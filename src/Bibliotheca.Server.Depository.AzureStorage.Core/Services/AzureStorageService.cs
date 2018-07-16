using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions;
using Bibliotheca.Server.Depository.AzureStorage.Core.Parameters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly ApplicationParameters _applicationParameters;

        private readonly ILogger<AzureStorageService> _logger;

        public AzureStorageService(IOptions<ApplicationParameters> applicationParameters, ILogger<AzureStorageService> logger)
        {
            _applicationParameters = applicationParameters.Value;
            _logger = logger;
        }

        public async Task<IList<string>> GetProjectsIdsAsync()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            BlobContinuationToken continuationToken = null;
            ContainerResultSegment resultSegment = null;

            List<string> containers = new List<string>();
            do
            {
                resultSegment = await blobClient.ListContainersSegmentedAsync(continuationToken);
                foreach (var blobItem in resultSegment.Results)
                {
                    var containerName = blobItem.StorageUri.PrimaryUri.Segments.Last();
                    if(!containerName.Equals(GroupsService.SettingsContainerName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        containers.Add(containerName);
                    }
                }

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            return containers.ToArray();
        }

        public async Task<IList<string>> GetBranchesNamesAsync(string projectId)
        {
            return await GetFoldersAsync(projectId, string.Empty, string.Empty);
        }

        public async Task<bool> IsFileExistsAsync(string projectId, string branchName, string fileUri)
        {
            CloudBlobContainer container = GetContainerReference(projectId);

            var path = Path.Combine(branchName, fileUri);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);

            bool exists = await blockBlob.ExistsAsync();
            return exists;
        }

        public async Task DeleteFileAsync(string projectId, string fileUri)
        {
            await DeleteFileAsync(projectId, string.Empty, fileUri);
        }

        public async Task DeleteFileAsync(string projectId, string branchName, string fileUri)
        {
            CloudBlobContainer container = GetContainerReference(projectId);

            var path = Path.Combine(branchName, fileUri);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);

            await blockBlob.DeleteIfExistsAsync();
        }

        public async Task<IList<string>> GetFilesAsync(string projectId, string branchName)
        {
            CloudBlobContainer container = GetContainerReference(projectId);
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            List<string> blobs = new List<string>();
            int prefixLength = $"/{projectId}/{branchName}/".Length;

            do
            {
                resultSegment = await container.ListBlobsSegmentedAsync(branchName, true, BlobListingDetails.Metadata, 10, continuationToken, null, null);
                foreach (var blobItem in resultSegment.Results)
                {
                    var fileUri = blobItem.StorageUri.PrimaryUri.LocalPath;
                    fileUri = fileUri.Substring(prefixLength, fileUri.Length - prefixLength);
                    blobs.Add(fileUri);
                }

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            return blobs.ToArray();
        }

        public async Task<IList<string>> GetFoldersAsync(string projectId, string branchName)
        {
            return await GetFoldersAsync(projectId, branchName, string.Empty);
        }

        public async Task<IList<string>> GetFoldersAsync(string projectId, string branchName, string path)
        {
            CloudBlobContainer container = GetContainerReference(projectId);
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            var folderPath = Path.Combine(branchName, path);
            List<string> blobs = new List<string>();
            do
            {
                resultSegment = await container.ListBlobsSegmentedAsync(folderPath, false, BlobListingDetails.Metadata, 10, continuationToken, null, null);
                foreach (var blobItem in resultSegment.Results)
                {
                    var url = blobItem.StorageUri.PrimaryUri.Segments.Last();
                    if (url.EndsWith("/"))
                    {
                        var folderName = url.Trim('/');
                        folderName = WebUtility.UrlDecode(folderName);
                        blobs.Add(folderName);
                    }
                }

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            return blobs.ToArray();
        }

        public async Task<string> ReadTextAsync(string projectId, string fileUri)
        {
            return await ReadTextAsync(projectId, string.Empty, fileUri);
        }

        public async Task<string> ReadTextAsync(string projectId, string branchName, string fileUri)
        {
            CloudBlobContainer container = GetContainerReference(projectId);

            var path = Path.Combine(branchName, fileUri);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);

            bool exists = await blockBlob.ExistsAsync();
            if(!exists)
            {
                throw new DocumentNotFoundException($"File '{path}' not found.");
            }

            string text = await blockBlob.DownloadTextAsync();
            return text;
        }

        public async Task<string> ReadAppendTextAsync(string projectId, string fileUri)
        {
            return await ReadAppendTextAsync(projectId, string.Empty, fileUri);
        }

        public async Task<string> ReadAppendTextAsync(string projectId, string branchName, string fileUri)
        {
            CloudBlobContainer container = GetContainerReference(projectId);

            var path = Path.Combine(branchName, fileUri);
            var blockBlob = container.GetAppendBlobReference(path);

            bool exists = await blockBlob.ExistsAsync();
            if(!exists)
            {
                throw new DocumentNotFoundException($"File '{path}' not found.");
            }

            string text = await blockBlob.DownloadTextAsync();
            return text;
        }

        public async Task<byte[]> ReadBinaryAsync(string projectId, string branchName, string fileUri)
        {
            CloudBlobContainer container = GetContainerReference(projectId);
            var path = Path.Combine(branchName, fileUri);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);

            bool exists = await blockBlob.ExistsAsync();
            if(!exists)
            {
                throw new DocumentNotFoundException($"File '{path}' not found.");
            }

            using (var ms = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task WriteTextAsync(string projectId, string fileUri, string contents)
        {
            await WriteTextAsync(projectId, string.Empty, fileUri, contents);
        }

        public async Task WriteTextAsync(string projectId, string branchName, string fileUri, string contents)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(projectId);

            var path = Path.Combine(branchName, fileUri);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);
            await blockBlob.UploadTextAsync(contents);
        }

        public async Task AppendTextAsync(string projectId, string fileUri, string contents)
        {
            await AppendTextAsync(projectId, string.Empty, fileUri, contents);
        }

        public async Task AppendTextAsync(string projectId, string branchName, string fileUri, string contents)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(projectId);

            var path = Path.Combine(branchName, fileUri);
            var blockBlob = container.GetAppendBlobReference(path);
            if (!await blockBlob.ExistsAsync())
            {
                await blockBlob.CreateOrReplaceAsync();
            }

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                await blockBlob.AppendBlockAsync(stream);
            }
        }

        public async Task WriteBinaryAsync(string projectId, string branchName, string fileUri, byte[] contents)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(projectId);

            var path = Path.Combine(branchName, fileUri);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);
            await blockBlob.UploadFromByteArrayAsync(contents, 0, contents.Length);
        }

        public async Task CreateFolderAsync(string projectId)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(projectId);

            bool exists = await container.ExistsAsync();
            if(exists)
            {
               throw new ProjectAlreadyExistsException($"Project '{projectId}' already exists");
            }

            await container.CreateAsync();
        }

        public async Task DeleteFolderAsync(string projectId)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(projectId);

            bool exists = await container.ExistsAsync();
            if (!exists)
            {
                throw new ProjectNotFoundException($"Project '{projectId}' not found.");
            }

            await container.DeleteAsync();
        }

        public async Task DeleteFolderAsync(string projectId, string branchName)
        {
            await DeleteFolderAsync(projectId, branchName, string.Empty);
        }

        public async Task DeleteFolderAsync(string projectId, string branchName, string path)
        {
            _logger.LogInformation($"Deleting folder: '{path}' from branch: '{branchName}' and project: '{projectId}'.");

            CloudBlobContainer container = GetContainerReference(projectId);
            var folderPath = Path.Combine(branchName, path);

            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;
            do
            {
                resultSegment = await container.ListBlobsSegmentedAsync(folderPath, true, BlobListingDetails.Metadata, 10, continuationToken, null, null);
                foreach (var blobItem in resultSegment.Results)
                {
                    _logger.LogInformation($"Deleting '{blobItem.StorageUri.PrimaryUri}' file...");

                    var blob = blobItem as CloudBlockBlob;
                    if(blob != null)
                    {
                        var blobReference = container.GetBlockBlobReference(blob.Name);
                        if(blobReference != null)
                        {
                            await blobReference.DeleteIfExistsAsync();
                            _logger.LogInformation($"File '{blobItem.StorageUri.PrimaryUri}' deleted.");
                        }
                        else
                        {
                            _logger.LogWarning($"There is an issue with deleting '{blobItem.StorageUri.PrimaryUri}' file. Blobk blob refenrece not exists.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"There is an issue with deleting '{blobItem.StorageUri.PrimaryUri}' file. Blob is not a cloud block blob.");
                    }
                }

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            _logger.LogInformation($"Deleting process finished.");
        }

        private CloudBlobContainer GetContainerReference(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            if (container == null)
            {
                throw new ProjectNotFoundException($"Azure cloud blob container '{containerName}' not exists");
            }

            return container;
        }
    }
}
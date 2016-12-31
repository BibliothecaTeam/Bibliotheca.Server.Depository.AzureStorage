using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.Parameters;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly ApplicationParameters _applicationParameters;

        public FileSystemService(IOptions<ApplicationParameters> applicationParameters)
        {
            _applicationParameters = applicationParameters.Value;
        }

        public async Task<IList<string>> GetProjectsIdsAsync()
        {
            return await GetFoldersAsync(string.Empty);
        }

        public async Task<IList<string>> GetBranchesNamesAsync(string projectId)
        {
            return await GetFoldersAsync(projectId);
        }

        public Task<bool> IsFileExistsAsync(string projectId, string branchName, string fileUri)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(string projectId, string branchName, string fileUri)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetFoldersAsync(string projectId)
        {
            return await GetFoldersAsync(projectId, string.Empty, string.Empty);
        }

        public async Task<IList<string>> GetFoldersAsync(string projectId, string branchName)
        {
            return await GetFoldersAsync(projectId, branchName, string.Empty);
        }

        public Task<IList<string>> GetFoldersAsync(string projectId, string branchName, string path)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ReadTextAsync(string projectId, string fileUri)
        {
            return await ReadTextAsync(projectId, string.Empty, fileUri);
        }

        public Task<string> ReadTextAsync(string projectId, string branchName, string fileUri)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadBinaryAsync(string projectId, string branchName, string fileUri)
        {
            throw new NotImplementedException();
        }

        public async Task WriteTextAsync(string projectId, string fileUri, string contents)
        {
            await WriteTextAsync(projectId, string.Empty, fileUri, contents);
        }

        public Task WriteTextAsync(string projectId, string branchName, string fileUri, string contents)
        {
            throw new NotImplementedException();
        }

        public Task WriteBinaryAsync(string projectId, string branchName, string fileUri, byte[] contents)
        {
            throw new NotImplementedException();
        }

        public async Task CreateFolderAsync(string projectId)
        {
            await CreateFolderAsync(projectId, string.Empty, string.Empty);
        }

        public async Task CreateFolderAsync(string projectId, string branchName)
        {
            await CreateFolderAsync(projectId, branchName, string.Empty);
        }

        public Task CreateFolderAsync(string projectId, string branchName, string path)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteFolderAsync(string projectId)
        {
            await DeleteFolderAsync(projectId, string.Empty, string.Empty);
        }

        public async Task DeleteFolderAsync(string projectId, string branchName)
        {
            await DeleteFolderAsync(projectId, branchName, string.Empty);
        }

        public Task DeleteFolderAsync(string projectId, string branchName, string path)
        {
            throw new NotImplementedException();
        }
    }
}
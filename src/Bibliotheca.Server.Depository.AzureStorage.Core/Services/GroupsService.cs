using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;
using Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions;
using Bibliotheca.Server.Depository.AzureStorage.Core.Parameters;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly ApplicationParameters _applicationParameters;

        public const string SettingsContainerName = "bibliotheca-settings";
        private const string GroupFileName = "groups.json";

        public GroupsService(IOptions<ApplicationParameters> applicationParameters)
        {
            _applicationParameters = applicationParameters.Value;
        }

        public async Task<IList<GroupDto>> GetGroupsAsync()
        {
            CloudBlockBlob fileBlob = GetFileBlob();
            var groups = await ReadGroupsAsync(fileBlob);
            return groups;
        }

        public async Task<GroupDto> GetGroupAsync(string groupName)
        {
            CloudBlockBlob fileBlob = GetFileBlob();
            var groups = await ReadGroupsAsync(fileBlob);
            return groups.FirstOrDefault(x => x.Name == groupName);
        }

        public async Task CreateGroupAsync(GroupDto group)
        {
            CloudBlockBlob fileBlob = GetFileBlob();
            var groups = await ReadGroupsAsync(fileBlob);

            if (groups.Any(x => x.Name == group.Name))
            {
                throw new GroupAlreadyExistsException($"Group {group.Name} already exists.");
            }

            groups.Add(group);
            await UploadGroupsAsync(fileBlob, groups);
        }

        public async Task UpdateGroupAsync(GroupDto group)
        {
            CloudBlockBlob fileBlob = GetFileBlob();
            var groups = await ReadGroupsAsync(fileBlob);

            var groupToDelete = groups.FirstOrDefault(x => x.Name == group.Name);
            if(groupToDelete == null)
            {
                throw new GroupNotExistsException($"Group {group.Name} not exists.");
            }

            groups.Remove(groupToDelete);
            groups.Add(group);
            await UploadGroupsAsync(fileBlob, groups);
        }

        public async Task DeleteGroupAsync(string groupName)
        {
            CloudBlockBlob fileBlob = GetFileBlob();
            var groups = await ReadGroupsAsync(fileBlob);

            var groupToDelete = groups.FirstOrDefault(x => x.Name == groupName);
            if(groupToDelete == null)
            {
                throw new GroupNotExistsException($"Group {groupName} not exists.");
            }

            groups.Remove(groupToDelete);
            await UploadGroupsAsync(fileBlob, groups);
        }

        private CloudBlockBlob GetFileBlob()
        {
            var container = GetContainerReference();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(GroupFileName);
            return blockBlob;
        }

        private async Task<List<GroupDto>> ReadGroupsAsync(CloudBlockBlob fileBlob)
        {
            var groups = new List<GroupDto>();
            var fileExist = await fileBlob.ExistsAsync();
            if(fileExist)
            {
                var groupsString = await fileBlob.DownloadTextAsync();
                groups = JsonConvert.DeserializeObject<List<GroupDto>>(groupsString);
            }

            return groups;
        }

        private async Task UploadGroupsAsync(CloudBlockBlob fileBlob, List<GroupDto> groups)
        {
            var sortedGroups = groups.OrderBy(x => x.Name);
            var serializedGroups = JsonConvert.SerializeObject(sortedGroups);
            await fileBlob.UploadTextAsync(serializedGroups);
        }

        private CloudBlobContainer GetContainerReference()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(SettingsContainerName);

            container.CreateIfNotExistsAsync();
            return container;
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public interface IGroupsService
    {
        Task<IList<GroupDto>> GetGroupsAsync();

        Task<GroupDto> GetGroupAsync(string groupName);

        Task CreateGroupAsync(GroupDto group);

        Task UpdateGroupAsync(GroupDto group);

        Task DeleteGroupAsync(string groupName);
    }
}
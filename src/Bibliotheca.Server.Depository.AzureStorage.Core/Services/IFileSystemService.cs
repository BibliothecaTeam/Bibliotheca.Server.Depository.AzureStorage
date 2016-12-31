using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public interface IFileSystemService
    {
        Task<IList<string>> GetProjectsIdsAsync();

        Task<IList<string>> GetBranchesNamesAsync(string projectId);

        Task<bool> IsFileExistsAsync(string projectId, string branchName, string fileUri);

        Task DeleteFileAsync(string projectId, string branchName, string fileUri);

        Task<IList<string>> GetFoldersAsync(string projectId);

        Task<IList<string>> GetFoldersAsync(string projectId, string branchName);

        Task<IList<string>> GetFoldersAsync(string projectId, string branchName, string path);

        Task CreateFolderAsync(string projectId);

        Task CreateFolderAsync(string projectId, string branchName);

        Task CreateFolderAsync(string projectId, string branchName, string path);

        Task DeleteFolderAsync(string projectId);

        Task DeleteFolderAsync(string projectId, string branchName);

        Task DeleteFolderAsync(string projectId, string branchName, string path);

        Task<string> ReadTextAsync(string projectId, string fileUri);

        Task<string> ReadTextAsync(string projectId, string branchName, string fileUri);

        Task<byte[]> ReadBinaryAsync(string projectId, string branchName, string fileUri);

        Task WriteTextAsync(string projectId, string fileUri, string contents);

        Task WriteTextAsync(string projectId, string branchName, string fileUri, string contents);

        Task WriteBinaryAsync(string projectId, string branchName, string fileUri, byte[] contents);
    }
}
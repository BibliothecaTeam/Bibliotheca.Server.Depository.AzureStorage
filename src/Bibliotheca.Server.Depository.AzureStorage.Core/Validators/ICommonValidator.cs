using System.Threading.Tasks;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Validators
{
    public interface ICommonValidator
    {
        void ProjectIdShouldBeSpecified(string projectId);

        void BranchNameShouldBeSpecified(string branchName);

        void DocumentUriShouldBeSpecified(string fileUri);

        Task BranchHaveToExists(string projectId, string branchName);

        Task ProjectHaveToExists(string projectId);

        Task DocumentHaveToExists(string projectId, string branchName, string fileUri);

        Task DocumentShouldNotExists(string projectId, string branchName, string fileUri);

        Task BranchShouldNotExists(string projectId, string branchName);

        Task ProjectShouldNotExists(string projectId);

    }
}
using System.Threading.Tasks;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public interface ILogsService
    {
        Task<string> GetLogsAsync(string projectId);

        Task AppendLogsAsync(string projectId, string logs);
    }
}
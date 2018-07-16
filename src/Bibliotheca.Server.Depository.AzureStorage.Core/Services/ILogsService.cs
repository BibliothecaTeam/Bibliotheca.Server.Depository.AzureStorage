using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public interface ILogsService
    {
        Task<LogsDto> GetLogsAsync(string projectId);

        Task AppendLogsAsync(string projectId, LogsDto logs);

        Task DeleteLogsAsync(string projectId);
    }
}
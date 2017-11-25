using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public class LogsService : ILogsService
    {
        private readonly IAzureStorageService _azureStorageService;

        public LogsService(IAzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public async Task<LogsDto> GetLogsAsync(string projectId)
        {
            var logs = await _azureStorageService.ReadTextAsync(projectId, "logs.txt");
            return new LogsDto { Message = logs };
        }

        public async Task AppendLogsAsync(string projectId, LogsDto logs)
        {
            await _azureStorageService.WriteTextAsync(projectId, "logs.txt", logs.Message);
        }
    }
}
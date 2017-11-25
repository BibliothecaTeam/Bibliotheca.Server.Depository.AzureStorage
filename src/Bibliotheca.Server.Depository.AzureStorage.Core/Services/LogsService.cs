using System.Threading.Tasks;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public class LogsService : ILogsService
    {
        private readonly IAzureStorageService _azureStorageService;

        public LogsService(IAzureStorageService azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public async Task<string> GetLogsAsync(string projectId)
        {
            var logs = await _azureStorageService.ReadTextAsync(projectId, "logs.txt");
            return logs;
        }

        public async Task AppendLogsAsync(string projectId, string logs)
        {
            await _azureStorageService.WriteTextAsync(projectId, "logs.txt", logs);
        }
    }
}
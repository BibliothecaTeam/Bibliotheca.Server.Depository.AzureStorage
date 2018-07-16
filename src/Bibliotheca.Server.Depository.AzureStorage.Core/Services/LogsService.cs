using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;
using Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions;

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
            var logs = string.Empty;
            try
            {
                logs = await _azureStorageService.ReadAppendTextAsync(projectId, "logs.txt");
            }
            catch(DocumentNotFoundException) 
            { 
            }
            catch(Exception exception) 
            {
                throw exception;
            }

            return new LogsDto { Message = logs };
        }

        public async Task AppendLogsAsync(string projectId, LogsDto logs)
        {
            await _azureStorageService.AppendTextAsync(projectId, "logs.txt", logs.Message);
        }

        public async Task DeleteLogsAsync(string projectId)
        {
            await _azureStorageService.DeleteFileAsync(projectId, "logs.txt");
        }
    }
}
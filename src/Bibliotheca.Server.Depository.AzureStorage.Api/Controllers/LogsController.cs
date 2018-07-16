using System.Reflection;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;
using Bibliotheca.Server.Depository.AzureStorage.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Depository.AzureStorage.Api.Controllers
{
    /// <summary>
    /// Logs controller.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/logs/{projectId}")]
    public class LogsController : Controller
    {
        private readonly ILogsService _logsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logsService">Logs service.</param>
        public LogsController(ILogsService logsService)
        {
            _logsService = logsService;
        }

        /// <summary>
        /// Get logs for project.
        /// </summary>
        /// <remarks>
        /// Endpoint returns logs messages associated with specific project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <returns>Logs dor specific project.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(LogsDto))]
        public async Task<LogsDto> Get(string projectId)
        {
            return await _logsService.GetLogsAsync(projectId);
        }

        /// <summary>
        /// Append logs to project.
        /// </summary>
        /// <remarks>
        /// Endpoint appends logs messages associated with specific project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="logsDto">Message.</param>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task Put(string projectId, [FromBody] LogsDto logsDto)
        {
            await _logsService.AppendLogsAsync(projectId, logsDto);
        }

        /// <summary>
        /// Delete logs from project.
        /// </summary>
        /// <remarks>
        /// Endpoint delete logs messages associated to specific project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        [HttpDelete]
        [ProducesResponseType(200)]
        public async Task Delete(string projectId)
        {
            await _logsService.DeleteLogsAsync(projectId);
        }
    }
}
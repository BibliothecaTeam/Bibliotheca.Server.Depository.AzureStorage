using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;
using Bibliotheca.Server.Depository.AzureStorage.Core.Services;
using Bibliotheca.Server.Mvc.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Depository.AzureStorage.Api.Controllers
{
    /// <summary>
    /// Controller which manages branches in documentation project.
    /// </summary>
    [UserAuthorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches")]
    public class BranchesController : Controller
    {
        private readonly IBranchesService _branchesService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="branchesService">Branches service.</param>
        public BranchesController(IBranchesService branchesService)
        {
            _branchesService = branchesService;
        }

        /// <summary>
        /// Get all branches for specific project.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all branches for specific project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <returns>List of branches.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<BranchDto>))]
        public async Task<IList<BranchDto>> Get(string projectId)
        {
            var branches = await _branchesService.GetBranchesAsync(projectId);
            return branches;
        }

        /// <summary>
        /// Get information about specific branch in project.
        /// </summary>
        /// <remarks>
        /// Endpoint returns information about one specific project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>Information about specific branch.</returns>
        [HttpGet("{branchName}")]
        [ProducesResponseType(200, Type = typeof(BranchDto))]
        public async Task<BranchDto> Get(string projectId, string branchName)
        {
            var branch = await _branchesService.GetBranchAsync(projectId, branchName);
            return branch;
        }

        /// <summary>
        /// Create a new branch.
        /// </summary>
        /// <remarks>
        /// Endpoint for creating a new branch in project. Information about branch should be send as a JSON in body request.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branch">Information about branch.</param>
        /// <returns>If created successfully endpoint returns 201 (Created).</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Post(string projectId, [FromBody] BranchDto branch)
        {
            await _branchesService.CreateBranchAsync(projectId, branch);
            return Created($"/projects/{projectId}/branches/{branch.Name}", branch);
        }

        /// <summary>
        /// Update information about branch.
        /// </summary>
        /// <remarks>
        /// Endpoint for updating information about branch. Information about branch should be send as a JSON in body request.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="branch">Information about branch.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{branchName}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put(string projectId, string branchName, [FromBody] BranchDto branch)
        {
            await _branchesService.UpdateBranchAsync(projectId, branchName, branch);
            return Ok();
        }

        /// <summary>
        /// Delete specific branch.
        /// </summary>
        /// <remarks>
        /// Endpoint for deleting specific branch.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>If deleted successfully endpoint returns 200 (Ok).</returns>
        [HttpDelete("{branchName}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(string projectId, string branchName)
        {
            await _branchesService.DeleteBranchAsync(projectId, branchName);
            return Ok();
        }
    }
}
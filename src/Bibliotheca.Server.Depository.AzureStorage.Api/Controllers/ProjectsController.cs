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
    /// Controller which manages projects infrmation.
    /// </summary>
    [UserAuthorize]
    [ApiVersion("1.0")]
    [Route("api/projects")]
    public class ProjectsController : Controller
    {
        private readonly IProjectsService _projectsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="projectsService">Projects service.</param>
        public ProjectsController(IProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        /// <summary>
        /// Get list of projects.
        /// </summary>
        /// <remarks>
        /// Endpoint returns projects stored in the system.
        /// </remarks>
        /// <returns>List of projects.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<ProjectDto>))]
        public async Task<IList<ProjectDto>> Get()
        {
            var projects = await _projectsService.GetProjectsAsync();
            return projects;
        }

        /// <summary>
        /// Get specific project.
        /// </summary>
        /// <remarks>
        /// Endpoint returns detailed information about specific project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <returns>Information about specific project.</returns>
        [HttpGet("{projectId}")]
        [ProducesResponseType(200, Type = typeof(ProjectDto))]
        public async Task<ProjectDto> Get(string projectId)
        {
            var project = await _projectsService.GetProjectAsync(projectId);
            return project;
        }

        /// <summary>
        /// Create a new project.
        /// </summary>
        /// <remarks>
        /// Endpoint for creating a new project. Project is automatically assigned to the user (if authorization service is enabled).
        /// </remarks>
        /// <param name="project">Project information.</param>
        /// <returns>If created successfully endpoint returns 201 (Created).</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Post([FromBody] ProjectDto project)
        {
            await _projectsService.CreateProjectAsync(project);
            return Created($"/projects/{project.Id}", project);
        }

        /// <summary>
        /// Update information about project.
        /// </summary>
        /// <remarks>
        /// Endpoint for updating information about project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="project">Project information.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{projectId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put(string projectId, [FromBody] ProjectDto project)
        {
            await _projectsService.UpdateProjectAsync(projectId, project);
            return Ok();
        }

        /// <summary>
        /// Delete specific project.
        /// </summary>
        /// <remarks>
        /// Endpoint for deleting specific project. Besides project information all branches and documentation files are deleting. 
        /// Also documentation is deleted from search index (if search service is enabled). 
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <returns>If deleted successfully endpoint returns 200 (Ok).</returns>
        [HttpDelete("{projectId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(string projectId)
        {
            await _projectsService.DeleteProjectAsync(projectId);
            return Ok();
        }
    }
}

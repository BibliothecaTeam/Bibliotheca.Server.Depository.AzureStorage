using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;
using Bibliotheca.Server.Depository.AzureStorage.Core.Services;
using Bibliotheca.Server.Mvc.Middleware.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Depository.AzureStorage.Api.Controllers
{
    /// <summary>
    /// Controller which manages groups which we can set on project.
    /// </summary>
    [UserAuthorize]
    [ApiVersion("1.0")]
    [Route("api/groups")]
    public class GroupsController : Controller
    {
        private readonly IGroupsService _groupsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupsService">Groups service.</param>
        public GroupsController(IGroupsService groupsService)
        {
            _groupsService = groupsService;
        }

        /// <summary>
        /// Get all groups.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all groups.
        /// </remarks>
        /// <returns>List of groups.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<GroupDto>))]
        public async Task<IList<GroupDto>> Get()
        {
            var groups = await _groupsService.GetGroupsAsync();
            return groups;
        }

        /// <summary>
        /// Get group information.
        /// </summary>
        /// <remarks>
        /// Endpoint returns information about specific group.
        /// </remarks>
        /// <param name="groupName">Group name.</param>
        /// <returns>Group data.</returns>
        [HttpGet("{groupName}")]
        [ProducesResponseType(200, Type = typeof(GroupDto))]
        public async Task<GroupDto> Get(string groupName)
        {
            var group = await _groupsService.GetGroupAsync(groupName);
            return group;
        }

        /// <summary>
        /// Create a new group.
        /// </summary>
        /// <remarks>
        /// Endpoint for creating a new group. Information about group (name, svg icon) should be send as a JSON in body request.
        /// </remarks>
        /// <param name="group">Group data.</param>
        /// <returns>If created successfully endpoint returns 201 (Created).</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Post([FromBody] GroupDto group)
        {
            await _groupsService.CreateGroupAsync(group);
            return Created($"/groups/{group.Name}", group);
        }

        /// <summary>
        /// Update group information.
        /// </summary>
        /// <remarks>
        /// Endpoint for updating group information. Information about group (name, svg icon) should be send as a JSON in body request.
        /// </remarks>
        /// <param name="groupName">Group name.</param>
        /// <param name="group">Group data.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{groupName}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put(string groupName, [FromBody] GroupDto group)
        {
            group.Name = groupName;
            await _groupsService.UpdateGroupAsync(group);
            return Ok();
        }

        /// <summary>
        /// Delete group.
        /// </summary>
        /// <remarks>
        /// Endpoint for deleting group.
        /// </remarks>
        /// <param name="groupName">Group name.</param>
        /// <returns>If deleted successfully endpoint returns 200 (Ok).</returns>
        [HttpDelete("{groupName}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(string groupName)
        {
            await _groupsService.DeleteGroupAsync(groupName);
            return Ok();
        }
    }
}
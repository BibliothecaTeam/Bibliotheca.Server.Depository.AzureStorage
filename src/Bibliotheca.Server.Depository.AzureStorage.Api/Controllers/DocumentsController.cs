using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;
using Bibliotheca.Server.Depository.AzureStorage.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Depository.AzureStorage.Api.Controllers
{
    /// <summary>
    /// Controller which manages documents stored in documentation branch in project.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/{branchName}/documents")]
    public class DocumentsController : Controller
    {
        private readonly IDocumentsService _documentsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="documentsService">Documents service.</param>
        public DocumentsController(IDocumentsService documentsService)
        {
            _documentsService = documentsService;
        }

        /// <summary>
        /// Get list of documents from branch in project.
        /// </summary>
        /// <remarks>
        /// Endpoint which returns list of documents from specific branch in project. 
        /// Each object contains base information about document.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>List of documents.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<BaseDocumentDto>))]
        public async Task<IList<BaseDocumentDto>> Get(string projectId, string branchName)
        {
            var documents = await _documentsService.GetDocumentsAsync(projectId, branchName);
            return documents;
        }

        /// <summary>
        /// Get information about specific document.
        /// </summary>
        /// <remarks>
        /// Endpoint returns detailed information about document. Contains also document body as a JSON property.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="fileUri">File uri. As a path separator to file is used ":". For example path "docs/folder/index.md"
        /// should be converted to "docs:folder:index.md".</param>
        /// <returns>Detailed information about document (with document body).</returns>
        [HttpGet("{fileUri}")]
        [ProducesResponseType(200, Type = typeof(DocumentDto))]
        public async Task<DocumentDto> Get(string projectId, string branchName, string fileUri)
        {
            fileUri = DecodeUrl(fileUri);
            var document = await _documentsService.GetDocumentAsync(projectId, branchName, fileUri);
            return document;
        }

        /// <summary>
        /// Create new document information.
        /// </summary>
        /// <remarks>
        /// Endpoint which is used to add a new documentation file to branch in project. Documentation files can be send one by one
        /// using this endpoint.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="document">Document data.</param>
        /// <returns>If created successfully endpoint returns 201 (Created).</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Post(string projectId, string branchName, [FromBody] DocumentDto document)
        {
            document.Uri = DecodeUrl(document.Uri);
            await _documentsService.CreateDocumentAsync(projectId, branchName, document);

            document.Content = null;
            return Created($"/projects/{projectId}/branches/{branchName}/documents/{document.Uri}", document);
        }

        /// <summary>
        /// Update documentation information.
        /// </summary>
        /// <remarks>
        /// Endpoint which is used to update documentation file in branch in project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="fileUri">File uri. As a path separator to file is used ":". For example path "docs/folder/index.md"
        /// should be converted to "docs:folder:index.md".</param>
        /// <param name="document">Document data.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{fileUri}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put(string projectId, string branchName, string fileUri, [FromBody] DocumentDto document)
        {
            fileUri = DecodeUrl(fileUri);
            await _documentsService.UpdateDocumentAsync(projectId, branchName, fileUri, document);
            return Ok();
        }

        /// <summary>
        /// Delete documentation file.
        /// </summary>
        /// <remarks>
        /// Endpoint which is used to delete specific documentation file.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="fileUri">File uri. As a path separator to file is used ":". For example path "docs/folder/index.md"
        /// should be converted to "docs:folder:index.md".</param>
        /// <returns>If deleted successfully endpoint returns 200 (Ok).</returns>
        [HttpDelete("{fileUri}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(string projectId, string branchName, string fileUri)
        {
            fileUri = DecodeUrl(fileUri);
            await _documentsService.DeleteDocumentAsync(projectId, branchName, fileUri);
            return Ok();
        }

        private string DecodeUrl(string url)
        {
            return url.Replace(":", "/");
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects;
using Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions;
using Bibliotheca.Server.Depository.AzureStorage.Core.MimeTypes;
using Bibliotheca.Server.Depository.AzureStorage.Core.Validators;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IAzureStorageService _azureStorageService;
        private readonly ICommonValidator _commonValidator;

        public DocumentsService(IAzureStorageService azureStorageService, ICommonValidator commonValidator)
        {
            _azureStorageService = azureStorageService;
            _commonValidator = commonValidator;
        }

        public async Task<IList<BaseDocumentDto>> GetDocumentsAsync(string projectId, string branchName)
        {
            await _commonValidator.ProjectHaveToExists(projectId);
            await _commonValidator.BranchHaveToExists(projectId, branchName);

            var baseDocuments = new List<BaseDocumentDto>();
            var files = await _azureStorageService.GetFilesAsync(projectId, branchName);
            foreach(var file in files)
            {
                var extension = Path.GetExtension(file);
                var mimeType = MimeTypeMap.GetMimeType(extension);

                var baseDocument = new BaseDocumentDto
                {
                    Uri = file,
                    Name = Path.GetFileName(file),
                    ConentType = mimeType
                };

                baseDocuments.Add(baseDocument);
            }

            return baseDocuments;
        }

        public async Task<DocumentDto> GetDocumentAsync(string projectId, string branchName, string fileUri)
        {
            await _commonValidator.ProjectHaveToExists(projectId);
            await _commonValidator.BranchHaveToExists(projectId, branchName);
            await _commonValidator.DocumentHaveToExists(projectId, branchName, fileUri);

            byte[] content;
            try
            {
                content = await _azureStorageService.ReadBinaryAsync(projectId, branchName, fileUri);
            }
            catch (FileNotFoundException)
            {
                throw new DocumentNotFoundException($"Document '{fileUri}' not exists in branch '{branchName}' in project '{projectId}'.");
            }

            var extension = Path.GetExtension(fileUri);
            var mimeType = MimeTypeMap.GetMimeType(extension);
            
            var document = new DocumentDto
            {
                ConentType = mimeType,
                Name = Path.GetFileName(fileUri),
                Uri = fileUri,
                Content = content
            };

            return document;
        }

        public async Task CreateDocumentAsync(string projectId, string branchName, DocumentDto document)
        {
            await _commonValidator.ProjectHaveToExists(projectId);
            await _commonValidator.BranchHaveToExists(projectId, branchName);

            _commonValidator.DocumentUriShouldBeSpecified(document.Uri);
            await _commonValidator.DocumentShouldNotExists(projectId, branchName, document.Uri);

            await _azureStorageService.WriteBinaryAsync(projectId, branchName, document.Uri, document.Content);
        }

        public async Task UpdateDocumentAsync(string projectId, string branchName, string fileUri, DocumentDto document)
        {
            await _commonValidator.ProjectHaveToExists(projectId);
            await _commonValidator.BranchHaveToExists(projectId, branchName);
            await _commonValidator.DocumentHaveToExists(projectId, branchName, fileUri);

            await _azureStorageService.WriteBinaryAsync(projectId, branchName, fileUri, document.Content);
        }

        public async Task DeleteDocumentAsync(string projectId, string branchName, string fileUri)
        {
            await _commonValidator.ProjectHaveToExists(projectId);
            await _commonValidator.BranchHaveToExists(projectId, branchName);
            await _commonValidator.DocumentHaveToExists(projectId, branchName, fileUri);

            await _azureStorageService.DeleteFileAsync(projectId, branchName, fileUri);
        }
    }
}
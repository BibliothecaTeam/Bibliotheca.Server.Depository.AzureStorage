namespace Bibliotheca.Server.Depository.AzureStorage.Core.DataTransferObjects
{
    public class DocumentDto : BaseDocumentDto
    {
        public byte[] Content { get; set; }
    }
}
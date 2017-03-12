using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class DocumentNotFoundException : NotFoundException
    {
        public DocumentNotFoundException(string message) : base(message)
        {
        }
    }
}
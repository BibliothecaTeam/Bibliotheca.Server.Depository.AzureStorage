using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class MkDocsFileNotFoundException : NotFoundException
    {
        public MkDocsFileNotFoundException(string message) : base(message)
        {
        }
    }
}
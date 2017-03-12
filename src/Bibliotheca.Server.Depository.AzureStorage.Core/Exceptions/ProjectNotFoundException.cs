using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class ProjectNotFoundException : NotFoundException
    {
        public ProjectNotFoundException(string message) : base(message)
        {
        }
    }
}
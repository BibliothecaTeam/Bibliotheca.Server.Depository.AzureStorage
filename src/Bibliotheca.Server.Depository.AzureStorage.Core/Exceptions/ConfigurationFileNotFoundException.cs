using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class ConfigurationFileNotFoundException : NotFoundException
    {
        public ConfigurationFileNotFoundException(string message) : base(message)
        {
        }
    }
}
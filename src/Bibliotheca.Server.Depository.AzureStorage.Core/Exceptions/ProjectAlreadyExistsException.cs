using System;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class ProjectAlreadyExistsException : Exception
    {
        public ProjectAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
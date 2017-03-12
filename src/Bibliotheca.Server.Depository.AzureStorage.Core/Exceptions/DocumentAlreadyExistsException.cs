using System;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class DocumentAlreadyExistsException : Exception
    {
        public DocumentAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
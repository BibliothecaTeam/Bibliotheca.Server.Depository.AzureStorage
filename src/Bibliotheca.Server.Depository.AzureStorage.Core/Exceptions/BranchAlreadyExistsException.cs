using System;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class BranchAlreadyExistsException : Exception
    {
        public BranchAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
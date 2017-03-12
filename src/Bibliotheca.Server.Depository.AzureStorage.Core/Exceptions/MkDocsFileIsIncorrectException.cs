using System;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class MkDocsFileIsIncorrectException : Exception
    {
        public MkDocsFileIsIncorrectException(string message) : base(message)
        {
        }
    }
}
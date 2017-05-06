using System;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class MkDocsFileIsIncorrectException : BibliothecaException
    {
        public MkDocsFileIsIncorrectException(string message) : base(message)
        {
        }
    }
}
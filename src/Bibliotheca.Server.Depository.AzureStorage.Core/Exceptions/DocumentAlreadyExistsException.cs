using System;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class DocumentAlreadyExistsException : BibliothecaException
    {
        public DocumentAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
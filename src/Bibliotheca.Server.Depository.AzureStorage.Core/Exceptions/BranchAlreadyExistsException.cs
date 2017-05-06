using System;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class BranchAlreadyExistsException : BibliothecaException
    {
        public BranchAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
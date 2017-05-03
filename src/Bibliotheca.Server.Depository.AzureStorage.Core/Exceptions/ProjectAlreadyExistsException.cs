using System;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class ProjectAlreadyExistsException : BibliothecaException
    {
        public ProjectAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
using System;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class GroupAlreadyExistsException : BibliothecaException
    {
        public GroupAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
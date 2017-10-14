using System;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Exceptions
{
    public class GroupNotExistsException : BibliothecaException
    {
        public GroupNotExistsException(string message) : base(message)
        {
        }
    }
}
using System;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Exceptions
{
    public class APIModelException : Exception
    {
        public ErrorType Code { get; set; }
        public APIModelException(ErrorType code, string message) : base(message)
        {
            Code = code;
        }
    }
}

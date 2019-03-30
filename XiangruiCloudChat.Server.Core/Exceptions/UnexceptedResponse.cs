using System;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Exceptions
{
    public class UnexceptedResponse : Exception
    {
        public AiurProtocol Response { get; set; }
        public ErrorType Code => Response.Code;
        public UnexceptedResponse(AiurProtocol response) : base(response.Message)
        {
            Response = response;
        }
    }
}

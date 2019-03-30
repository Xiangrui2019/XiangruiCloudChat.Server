using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Exceptions;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Attributes
{
    public class APIExpHandler : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            switch (context.Exception)
            {
                case UnexceptedResponse exp:
                    context.ExceptionHandled = true;
                    context.Result = new JsonResult(new AiurProtocol { Code = exp.Code, Message = exp.Message });
                    break;
                case APIModelException exp:
                    context.ExceptionHandled = true;
                    context.Result = new JsonResult(new AiurProtocol { Code = exp.Code, Message = exp.Message });
                    break;
            }
        }
    }
}

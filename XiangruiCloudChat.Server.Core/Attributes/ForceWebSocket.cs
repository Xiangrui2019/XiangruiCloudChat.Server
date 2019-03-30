using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Attributes
{
    public class ForceWebSocket : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (context.HttpContext.WebSockets.IsWebSocketRequest) return;
            var arg = new AiurProtocol
            {
                Code = ErrorType.InvalidInput,
                Message = "不可以使用HTTP协议访问Websocket接口!"
            };
            context.Result = new JsonResult(arg);
        }
    }
}

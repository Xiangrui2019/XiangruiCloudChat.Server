using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace XiangruiCloudChat.Server.Core.Attributes
{
    public class FileChecker : ActionFilterAttribute
    {
        public long MaxSize { get; set; } = -1;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            try
            {
                if (context.HttpContext.Request.Method.ToUpper().Trim() != "POST")
                {
                    context.ModelState.AddModelError("", "To upload your file, you have to submit the form!");
                    return;
                }

                if (context.HttpContext.Request.Form.Files.Count < 1)
                {
                    context.ModelState.AddModelError("", "Please provide a file!");
                    return;
                }

                var file = context.HttpContext.Request.Form.Files.First();
                if (file == null)
                {
                    context.ModelState.AddModelError("", "Please provide a file!");
                    return;
                }

                if (file.Length < 1)
                {
                    context.ModelState.AddModelError("", "Please provide a valid file!");
                    return;
                }

                if ((MaxSize != -1 && file.Length > MaxSize) || file.Length > Values.MaxFileSize)
                {
                    context.ModelState.AddModelError("", "Please provide a file which is smaller than 1GB!");
                }
            }
            catch (Exception e)
            {
                context.ModelState.AddModelError("", e.Message);
            }
        }
    }
}

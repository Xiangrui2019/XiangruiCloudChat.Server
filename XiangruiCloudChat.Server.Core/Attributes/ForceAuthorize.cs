using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using XiangruiCloudChat.Server.Core.Models;
using XiangruiCloudChat.Server.Core.Services;

namespace XiangruiCloudChat.Server.Core.Attributes
{
    public class ForceAuthorize : ActionFilterAttribute
    {
        private string PreferController { get; }
        private string PreferAction { get; }
        private bool? JustTry { get; } = false;
        private bool PreferPageSet { get; }
        private bool Register { get; }
        private bool DirectlyReject { get; }

        private bool HasAPreferPage => (!string.IsNullOrEmpty(PreferController)
            && !string.IsNullOrEmpty(PreferAction))
            || PreferPageSet;

        private string PreferPage
        {
            get
            {
                if (string.IsNullOrEmpty(PreferController) && string.IsNullOrEmpty(PreferAction))
                {
                    return "/";
                }
                return new AiurUrl(string.Empty, PreferController, PreferAction, new { }).ToString();
            }
        }

        public ForceAuthorize(bool directlyReject = false)
        {
            DirectlyReject = directlyReject;
        }

        public ForceAuthorize(string preferController, string preferAction, bool justTry, bool register = false)
        {
            PreferController = preferController;
            PreferAction = preferAction;
            JustTry = justTry ? true : (bool?) null;
            PreferPageSet = true;
            Register = register;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!(context.Controller is Controller controller))
            {
                throw new InvalidOperationException();
            }
            var show = context.HttpContext.Request.Query[Values.DirectShowString.Key];

            if (!controller.User.Identity.IsAuthenticated)
            {
                if (HasAPreferPage)
                {
                    if (show == Values.DirectShowString.Value && JustTry == true)
                    {
                        return;
                    }

                    context.Result = Redirect(context, PreferPage, JustTry, Register);
                }
                else if (DirectlyReject)
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    context.Result = Redirect(context, controller.Request.Path.Value, justTry: null, register: Register);
                }
            }
            else if (HasAPreferPage && !controller.Request.Path.Value.ToLower().StartsWith(PreferPage.ToLower()))
            {
                context.HttpContext.Response.Redirect(PreferPage);
            }
            else
            {
                return;
            }
        }

        private RedirectResult Redirect(ActionExecutingContext context, string page, bool? justTry, bool register)
        {
            var urlConverter = context.HttpContext.RequestServices.GetService<UrlConverter>();
            string serverPosition = $"{context.HttpContext.Request.Scheme}://{context.HttpContext.Request.Host}";
            string url = urlConverter.UrlWithAuth(serverPosition, page, justTry, register);
            return new RedirectResult(url);
        }
    }
}

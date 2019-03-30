﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Attributes
{
    public class APIModelStateChecker : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller && !controller.ModelState.IsValid)
            {
                context.Result = ResultGenerator.GetInvalidModelStateErrorResponse(controller.ModelState);
            }
            base.OnActionExecuting(context);
        }
    }

    public static class ResultGenerator
    {
        public static JsonResult GetInvalidModelStateErrorResponse(ModelStateDictionary modelstate)
        {
            var list = new List<string>();
            foreach (var value in modelstate)
            {
                foreach (var error in value.Value.Errors)
                {
                    list.Add(error.ErrorMessage);
                }
            }
            var arg = new AiurCollection<string>(list)
            {
                Code = ErrorType.InvalidInput,
                Message = "无法生成输出结果!"
            };
            return new JsonResult(arg);
        }
    }
}

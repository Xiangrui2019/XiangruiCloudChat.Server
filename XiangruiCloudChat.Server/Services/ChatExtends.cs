using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace XiangruiCloudChat.Server.Services
{
    public static class ChatExtends
    {
        public static JsonResult ChatJson(this Controller controller, object obj)
        {
            return controller.Json(obj, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
        }
    }
}
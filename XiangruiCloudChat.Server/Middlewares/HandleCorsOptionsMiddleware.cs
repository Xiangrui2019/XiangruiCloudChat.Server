using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace XiangruiCloudChat.Server.Middlewares
{
    public class HandleCorsOptionsMiddleware
    {
        private string ApplicationUrl { get; }
        private readonly RequestDelegate _next;

        public HandleCorsOptionsMiddleware(RequestDelegate next, IConfiguration configuration, IHostingEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                ApplicationUrl = configuration["ApplicationUrls:DevlopementApplicationUrl"];
            }
            else
            {
                ApplicationUrl = configuration["ApplicationUrls:ProductionApplicationUrl"];
            }
            
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Cache-Control", "no-cache");
            context.Response.Headers.Add("Expires", "-1");
            context.Response.Headers.Add("Access-Control-Allow-Origin", ApplicationUrl);
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Authorization");
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 204;
                return;
            }
            await _next.Invoke(context);
        }
    }
}
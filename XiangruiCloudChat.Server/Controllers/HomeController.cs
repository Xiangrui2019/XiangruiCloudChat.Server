using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XiangruiCloudChat.Server.Core.Models;
using XiangruiCloudChat.Server.Services;

namespace XiangruiCloudChat.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return this.ChatJson(new AiurProtocol
            {
                Code = ErrorType.Success,
                Message = $"欢迎访问祥瑞云易信{_configuration["CurrentVersion"]}的服务器端, 但是, 除了祥瑞云易信的客户端和SDK, 不可以使用浏览器直接访问."
            });
        }

        public IActionResult Ping()
        {
            return Ok();
        }
    }
}
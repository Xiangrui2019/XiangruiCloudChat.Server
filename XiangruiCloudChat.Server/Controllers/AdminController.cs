using System.Net.Mime;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Services.ToAPIServer;
using Microsoft.AspNetCore.Mvc;
using XiangruiCloudChat.Server.Data;

namespace XiangruiCloudChat.Server.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    [AiurForceAuth(directlyReject: true)]
    public class AdminController : Controller
    {
    }
}
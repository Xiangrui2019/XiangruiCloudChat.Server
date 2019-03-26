using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Aiursoft.Pylon.Attributes;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services.ToAPIServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XiangruiCloudChat.Server.Data;
using XiangruiCloudChat.Server.Services;

namespace XiangruiCloudChat.Server.Controllers
{
    [APIExpHandler]
    [APIModelStateChecker]
    [AiurForceAuth(directlyReject: true)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> GetOnlineUsersCount()
        {
            var onlineDevices = await _dbContext
                .OnlineDevices
                .AsNoTracking()
                .ToListAsync();

            return this.ChatJson(new AiurValue<int>(onlineDevices.Count)
            {
                Code = ErrorType.Success,
                Message = "成功获取了在线用户数量!",
                Value = onlineDevices.Count
            });
        }
    }
}
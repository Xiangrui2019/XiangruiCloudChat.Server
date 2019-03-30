using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebPush;
using XiangruiCloudChat.Server.Data;

namespace XiangruiCloudChat.Server.Services
{
    public class ThirdPartyPushService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly WebPushClient _webPushClient;
        private readonly ILogger _logger;

        public ThirdPartyPushService(
            ApplicationDbContext dbContext,
            IConfiguration configuration,
            WebPushClient webPushClient,
            ILogger<ThirdPartyPushService> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _webPushClient = webPushClient;
            _logger = logger;
        }

        public async Task PushAsync(string recieverId, string triggerEmail, string payload)
        {
            var devices = await _dbContext.Devices.Where(t => t.UserID == recieverId).ToListAsync();
            string vapidPublicKey = _configuration.GetSection("VapidKeys")["PublicKey"];
            string vapidPrivateKey = _configuration.GetSection("VapidKeys")["PrivateKey"];

            foreach (var device in devices)
            {
                try
                {
                    var pushSubscription = new PushSubscription(device.PushEndpoint, device.PushP256DH, device.PushAuth);
                    var vapidDetails = new VapidDetails("mailto:" + triggerEmail, vapidPublicKey, vapidPrivateKey);
                    _logger.LogInformation($"Trying to call WebPush API to push a new event to {recieverId}, Event content is '{payload}', Device ID is {device.Id}");
                    await _webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                }
                catch (WebPushException e)
                {
                    _logger.LogCritical(e, "A WebPush error occoured while calling WebPush API: " + e.Message);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "An error occoured while calling WebPush API: " + e.Message);
                }
            }
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Exceptions;
using XiangruiCloudChat.Server.Core.Models;
using XiangruiCloudChat.Server.Core.Models.Stargate.MessageAddressModels;

namespace XiangruiCloudChat.Server.Core.Services.ToStargateServer
{
    public class PushMessageService
    {
        private readonly HTTPService _httpService;
        private readonly ServiceLocation _serviceLocation;
        public PushMessageService(
            HTTPService httpService,
            ServiceLocation serviceLocation)
        {
            _httpService = httpService;
            _serviceLocation = serviceLocation;
        }

        public async Task<AiurProtocol> PushMessageAsync(string accessToken, int channelId, string messageContent, bool noexception = false)
        {
            var url = new AiurUrl(_serviceLocation.Stargate, "Message", "PushMessage", new { });
            var form = new AiurUrl(string.Empty, new PushMessageAddressModel
            {
                AccessToken = accessToken,
                ChannelId = channelId,
                MessageContent = messageContent
            });
            var result = await _httpService.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (!noexception && jResult.Code != ErrorType.Success)
            {
                throw new UnexceptedResponse(jResult);
            }
            return jResult;
        }
    }
}

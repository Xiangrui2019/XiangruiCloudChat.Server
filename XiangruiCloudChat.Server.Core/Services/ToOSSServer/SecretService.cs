using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Exceptions;
using XiangruiCloudChat.Server.Core.Models;
using XiangruiCloudChat.Server.Core.Models.OSS.SecretAddressModels;

namespace XiangruiCloudChat.Server.Core.Services.ToOSSServer
{
    public class SecretService
    {
        private readonly ServiceLocation _serviceLoation;
        private readonly HTTPService _http;

        public SecretService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLoation = serviceLocation;
            _http = http;
        }

        public async Task<AiurValue<string>> GenerateAsync(int id, string accessToken, int maxUseTimes)
        {
            var url = new AiurUrl(_serviceLoation.OSS, "Secret", "Generate", new GenerateAddressModel
            {
                Id = id,
                AccessToken = accessToken,
                MaxUseTimes = maxUseTimes
            });
            var result = await _http.Get(url, true);
            var jResult = JsonConvert.DeserializeObject<AiurValue<string>>(result);
            if (jResult.Code != ErrorType.Success)
                throw new UnexceptedResponse(jResult);
            return jResult;
        }
    }
}

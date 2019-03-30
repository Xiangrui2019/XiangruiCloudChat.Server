using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Developer.ApiAddressModels;
using XiangruiCloudChat.Server.Core.Exceptions;
using XiangruiCloudChat.Server.Core.Models;
using XiangruiCloudChat.Server.Core.Models.Developer.ApiAddressModels;
using XiangruiCloudChat.Server.Core.Models.Developer.ApiViewModels;

namespace XiangruiCloudChat.Server.Core.Services.ToDeveloperServer
{
    public class DeveloperApiService
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;
        public DeveloperApiService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<AiurProtocol> IsValidAppAsync(string appId, string appSecret)
        {
            var url = new AiurUrl(_serviceLocation.Developer, "api", "IsValidApp", new IsValidateAppAddressModel
            {
                AppId = appId,
                AppSecret = appSecret
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new UnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AppInfoViewModel> AppInfoAsync(string appId)
        {
            var url = new AiurUrl(_serviceLocation.Developer, "api", "AppInfo", new AppInfoAddressModel
            {
                AppId = appId
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AppInfoViewModel>(result);
            if (JResult.Code != ErrorType.Success)
                throw new UnexceptedResponse(JResult);
            return JResult;
        }
    }
}

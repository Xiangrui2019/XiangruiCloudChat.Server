﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Archon;
using XiangruiCloudChat.Server.Core.Exceptions;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Services.ToArchonServer
{
    public class ArchonApiService
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public ArchonApiService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<AccessTokenViewModel> AccessTokenAsync(string appId, string appSecret)
        {
            var url = new AiurUrl(_serviceLocation.Archon, "API", "AccessToken", new AccessTokenAddressModel
            {
                AppId = appId,
                AppSecret = appSecret
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AccessTokenViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new UnexceptedResponse(JResult);
            return JResult;
        }
    }
}

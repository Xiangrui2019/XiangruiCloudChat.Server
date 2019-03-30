﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.API.ApiViewModels;
using XiangruiCloudChat.Server.Core.Exceptions;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Services.ToAPIServer
{
    public class CoreApiService
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public CoreApiService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        [Obsolete(message: "Token was signed!", error: true)]
        public async Task<ValidateAccessTokenViewModel> ValidateAccessTokenAsync(string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.API, "api", "ValidateAccessToken", new
            {
                accessToken
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<ValidateAccessTokenViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new UnexceptedResponse(JResult);
            return JResult;
        }



        public async Task<AllUserGrantedViewModel> AllUserGrantedAsync(string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.API, "API", "AllUserGranted", new
            {
                accessToken
            });
            var result = await _http.Get(url, true);
            var JResult = JsonConvert.DeserializeObject<AllUserGrantedViewModel>(result);

            if (JResult.Code != ErrorType.Success)
                throw new UnexceptedResponse(JResult);
            return JResult;
        }

        public async Task<AiurProtocol> DropGrantsAsync(string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.API, "API", "DropGrants", new { });
            var form = new AiurUrl(string.Empty, new
            {
                AccessToken = accessToken
            });
            var result = await _http.Post(url, form, true);
            var jResult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jResult.Code != ErrorType.Success)
                throw new UnexceptedResponse(jResult);
            return jResult;
        }
    }
}

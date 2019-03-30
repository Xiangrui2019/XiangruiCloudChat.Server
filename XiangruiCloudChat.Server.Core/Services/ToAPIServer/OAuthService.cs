﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.API.OAuthAddressModels;
using XiangruiCloudChat.Server.Core.API.OAuthViewModels;
using XiangruiCloudChat.Server.Core.Exceptions;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Services.ToAPIServer
{
    public class OAuthService
    {
        private readonly ServiceLocation _serviceLocation;
        private readonly HTTPService _http;

        public OAuthService(
            ServiceLocation serviceLocation,
            HTTPService http)
        {
            _serviceLocation = serviceLocation;
            _http = http;
        }

        public async Task<AiurValue<int>> PasswordAuthAsync(string appid, string email, string password)
        {
            var url = new AiurUrl(_serviceLocation.API, "OAuth", "PasswordAuth", new { });
            var form = new AiurUrl(string.Empty, new PasswordAuthAddressModel
            {
                AppId = appid,
                Email = email,
                Password = password
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurValue<int>>(result);
            if (jresult.Code != ErrorType.Success)
                throw new UnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<AiurProtocol> AppRegisterAsync(string email, string password, string confirmPassword)
        {
            var url = new AiurUrl(_serviceLocation.API, "OAuth", "AppRegister", new { });
            var form = new AiurUrl(string.Empty, new AppRegisterAddressModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            });
            var result = await _http.Post(url, form, true);
            var jresult = JsonConvert.DeserializeObject<AiurProtocol>(result);
            if (jresult.Code != ErrorType.Success)
                throw new UnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<CodeToOpenIdViewModel> CodeToOpenIdAsync(int code, string accessToken)
        {
            var url = new AiurUrl(_serviceLocation.API, "OAuth", "CodeToOpenId", new CodeToOpenIdAddressModel
            {
                AccessToken = accessToken,
                Code = code,
                grant_type = "authorization_code"
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<CodeToOpenIdViewModel>(result);

            if (jresult.Code != ErrorType.Success)
                throw new UnexceptedResponse(jresult);
            return jresult;
        }

        public async Task<UserInfoViewModel> OpenIdToUserInfo(string accessToken, string openid)
        {
            var url = new AiurUrl(_serviceLocation.API, "oauth", "UserInfo", new UserInfoAddressModel
            {
                access_token = accessToken,
                openid = openid,
                lang = "en-US"
            });
            var result = await _http.Get(url, true);
            var jresult = JsonConvert.DeserializeObject<UserInfoViewModel>(result);
            if (jresult.Code != ErrorType.Success)
                throw new UnexceptedResponse(jresult);
            return jresult;
        }
    }
}

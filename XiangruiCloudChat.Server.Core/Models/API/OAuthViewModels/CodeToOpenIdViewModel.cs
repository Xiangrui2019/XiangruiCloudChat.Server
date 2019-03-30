using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.API.OAuthViewModels
{
    public class CodeToOpenIdViewModel : AiurProtocol
    {
        public string openid { get; set; }
        public string scope { get; set; }
    }
}

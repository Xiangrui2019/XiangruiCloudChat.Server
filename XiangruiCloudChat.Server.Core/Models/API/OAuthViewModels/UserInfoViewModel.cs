using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.API.OAuthViewModels
{
    public class UserInfoViewModel : AiurProtocol
    {
        public virtual AiurUserBase User { get; set; }
    }
}

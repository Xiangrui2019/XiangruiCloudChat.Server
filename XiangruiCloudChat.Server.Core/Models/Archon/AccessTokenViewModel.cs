using System;
using System.Collections.Generic;
using System.Text;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.Archon
{
    public class AccessTokenViewModel : AiurProtocol
    {
        public virtual string AccessToken { get; set; }
        public virtual DateTime DeadTime { get; set; }
    }
}

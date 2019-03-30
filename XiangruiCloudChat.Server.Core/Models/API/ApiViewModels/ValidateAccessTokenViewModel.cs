using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.API.ApiViewModels
{
    public class ValidateAccessTokenViewModel : AiurProtocol
    {
        public string AppId { get; set; } = null;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.API.ApiViewModels
{
    public class AllUserGrantedViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public List<Grant> Grants { get; set; }
    }
}

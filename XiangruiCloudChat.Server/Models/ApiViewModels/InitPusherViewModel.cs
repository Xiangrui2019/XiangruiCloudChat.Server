using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Models.Stargate.ChannelViewModels;

namespace XiangruiCloudChat.Server.Models.ApiViewModels
{
    public class InitPusherViewModel : CreateChannelViewModel
    {
        public string ServerPath { get; set; }
    }
}

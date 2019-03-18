using Aiursoft.Pylon.Models.Stargate.ChannelViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Models.ApiViewModels
{
    public class InitPusherViewModel : CreateChannelViewModel
    {
        public string ServerPath { get; set; }
    }
}

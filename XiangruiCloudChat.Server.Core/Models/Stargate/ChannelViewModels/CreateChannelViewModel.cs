using System;
using System.Collections.Generic;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Models.Stargate.ChannelViewModels
{
    public class CreateChannelViewModel : AiurProtocol
    {
        public int ChannelId { get; set; }
        public string ConnectKey { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Models.Stargate.ChannelViewModels
{
    public class ViewMyChannelsViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public IEnumerable<Channel> Channel { get; set; }
    }
}

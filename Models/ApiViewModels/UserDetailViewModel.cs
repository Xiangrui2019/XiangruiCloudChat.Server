using Aiursoft.Pylon.Models.Stargate.ChannelViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Pylon.Models;

namespace XiangruiCloudChat.Server.Models.ApiViewModels
{
    public class UserDetailViewModel : AiurProtocol
    {
        public ApplicationUser User { get; set; }
        public bool AreFriends { get; set; }
        public int? ConversationId { get; set; }
    }
}

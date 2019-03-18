using System.Threading.Tasks;
using Aiursoft.Pylon.Models.Stargate.ChannelViewModels;
using Aiursoft.Pylon.Services;
using Aiursoft.Pylon.Services.ToStargateServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XiangruiCloudChat.Server.Data;
using XiangruiCloudChat.Server.Events;
using XiangruiCloudChat.Server.Models;

namespace XiangruiCloudChat.Server.Services
{
    public class ChatPushService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PushMessageService _stargatePushService;
        private readonly AppsContainer _appsContainer;
        private readonly ChannelService _channelService;
        private readonly ThirdPartyPushService _thirdPartyPushService;

        public ChatPushService(
            ApplicationDbContext dbContext,
            PushMessageService stargatePushService,
            AppsContainer appsContainer,
            ChannelService channelService,
            ThirdPartyPushService thirdPartyPushService)
        {
            _dbContext = dbContext;
            _stargatePushService = stargatePushService;
            _appsContainer = appsContainer;
            _channelService = channelService;
            _thirdPartyPushService = thirdPartyPushService;
        }

        private static string _Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public async Task<CreateChannelViewModel> Init(string userId)
        {
            var token = await _appsContainer.AccessToken();
            var channel = await _channelService.CreateChannelAsync(token, $"Chat User Channel for Id: {userId}");
            return channel;
        }

        public async Task NewMessageEvent(ApplicationUser reciever, Conversation conversation, string content, ApplicationUser sender, bool alert)
        {
            var token = await _appsContainer.AccessToken();
            var channel = reciever.CurrentChannel;
            var newEvent = new NewMessageEvent
            {
                Type = EventType.NewMessage,
                ConversationId = conversation.Id,
                Sender = sender,
                Content = content,
                AESKey = conversation.AESKey,
                Muted = !alert,
                SentByMe = reciever.Id == sender.Id
            };
            if (channel != -1)
            {
                await _stargatePushService.PushMessageAsync(token, channel, _Serialize(newEvent), true);
            }
            if (alert)
            {
                await _thirdPartyPushService.PushAsync(reciever.Id, sender.Email, _Serialize(newEvent));
            }
        }

        public async Task NewFriendRequestEvent(string recieverId, string requesterId)
        {
            var token = await _appsContainer.AccessToken();
            var reciever = await _dbContext.Users.FindAsync(recieverId);
            var requester = await _dbContext.Users.FindAsync(requesterId);
            var channel = reciever.CurrentChannel;
            var nevent = new NewFriendRequest
            {
                Type = EventType.NewFriendRequest,
                RequesterId = requesterId
            };
            if (channel != -1)
                await _stargatePushService.PushMessageAsync(token, channel, _Serialize(nevent), true);
            await _thirdPartyPushService.PushAsync(reciever.Id, requester.Email, _Serialize(nevent));
        }

        public async Task WereDeletedEvent(string recieverId)
        {
            var token = await _appsContainer.AccessToken();
            var user = await _dbContext.Users.FindAsync(recieverId);
            var channel = user.CurrentChannel;
            var nevent = new WereDeletedEvent
            {
                Type = EventType.WereDeletedEvent
            };
            if (channel != -1)
                await _stargatePushService.PushMessageAsync(token, channel, _Serialize(nevent), true);
            await _thirdPartyPushService.PushAsync(user.Id, "xiangruikong2019@outlook.com", _Serialize(nevent));
        }

        public async Task FriendAcceptedEvent(string recieverId)
        {
            var token = await _appsContainer.AccessToken();
            var user = await _dbContext.Users.FindAsync(recieverId);
            var channel = user.CurrentChannel;
            var nevent = new FriendAcceptedEvent
            {
                Type = EventType.FriendAcceptedEvent
            };
            if (channel != -1)
                await _stargatePushService.PushMessageAsync(token, channel, _Serialize(nevent), true);
            await _thirdPartyPushService.PushAsync(user.Id, "xiangruikong2019@outlook.com", _Serialize(nevent));
        }
    }
}
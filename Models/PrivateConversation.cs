using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Models
{
    public class PrivateConversation : Conversation
    {
        public string RequesterId { get; set; }
        [ForeignKey(nameof(RequesterId))]
        public ApplicationUser RequestUser { get; set; }

        public string TargetId { get; set; }
        [ForeignKey(nameof(TargetId))]
        public ApplicationUser TargetUser { get; set; }
        [NotMapped]
        // Only a property for convenience.
        public string AnotherUserId { get; set; }

        public ApplicationUser AnotherUser(string myId) => myId == RequesterId ? TargetUser : RequestUser;
        public override int GetDisplayImage(string userId) => this.AnotherUser(userId).HeadImgFileKey;
        public override string GetDisplayName(string userId) => this.AnotherUser(userId).NickName;
        public override int GetUnReadAmount(string userId) => this.Messages.Count(p => !p.Read && p.SenderId != userId);
        public override Message GetLatestMessage()
        {
            try
            {
                return Messages.OrderByDescending(p => p.SendTime).First();
            }
            catch (InvalidOperationException)
            {
                return new Message
                {
                    Content = null,//"You are friends. Start chatting now!",
                    SendTime = this.ConversationCreateTime
                };
            }
        }

    }
}

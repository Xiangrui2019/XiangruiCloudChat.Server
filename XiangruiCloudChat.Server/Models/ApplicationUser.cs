﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Models
{
    public class ApplicationUser : AiurUserBase
    {
        [InverseProperty(nameof(PrivateConversation.RequestUser))]
        public IEnumerable<PrivateConversation> Friends { get; set; }

        [InverseProperty(nameof(PrivateConversation.TargetUser))]
        public IEnumerable<PrivateConversation> OfFriends { get; set; }

        [InverseProperty(nameof(UserGroupRelation.User))]
        public IEnumerable<UserGroupRelation> GroupsJoined { get; set; }

        [InverseProperty(nameof(GroupConversation.Owner))]
        public IEnumerable<GroupConversation> GroupsCreated { get; set; }

        [InverseProperty(nameof(Message.Sender))]
        public IEnumerable<Message> MessagesSent { get; set; }

        [InverseProperty(nameof(Report.Trigger))]
        public IEnumerable<Report> Reported { get; set; }

        [InverseProperty(nameof(Report.Target))]
        public IEnumerable<Report> ByReported { get; set; }

        [InverseProperty(nameof(Device.ApplicationUser))]
        public IEnumerable<Device> HisDevices { get; set; }

        public int CurrentChannel { get; set; } = -1;
        public string ConnectKey { get; set; }
        [JsonProperty]
        public bool MakeEmailPublic { get; set; } = true;
        [NotMapped]
        public bool IsMe { get; set; }

        public bool IsOnline { get; set; } = false;
        public override string Email { get; set; }
        public bool ShouldSerializeEmail() => MakeEmailPublic || IsMe;
    }
}
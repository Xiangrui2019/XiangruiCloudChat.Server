using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Models.Stargate
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        public int ChannelId { get; set; }
    }
}

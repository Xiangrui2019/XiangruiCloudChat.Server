using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Core.API
{
    public class Grant
    {
        public string AppID { get; set; }
        public DateTime GrantTime { get; set; } = DateTime.UtcNow;
        public string APIUserId { get; set; }
        [NotMapped]
        public virtual AiurUserBase UserInfo { get; set; }
    }
}

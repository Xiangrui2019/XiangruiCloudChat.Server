using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace XiangruiCloudChat.Server.Models
{
    public class Device
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string UserID { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(UserID))]
        public ApplicationUser ApplicationUser { get; set; }

        [JsonIgnore]
        public string PushEndpoint { get; set; }
        [JsonIgnore]
        public string PushP256DH { get; set; }
        [JsonIgnore]
        public string PushAuth { get; set; }
        public DateTime AddTime { get; set; } = DateTime.UtcNow;
    }
}

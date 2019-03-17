using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XiangruiCloudChat.Server.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        public string CreatorId { get; set; }
        [ForeignKey(nameof(CreatorId))]
        public ApplicationUser Creator { get; set; }

        public string TargetId { get; set; }
        [ForeignKey(nameof(TargetId))]
        [JsonIgnore]
        public ApplicationUser Target { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public bool Completed { get; set; }
    }
}

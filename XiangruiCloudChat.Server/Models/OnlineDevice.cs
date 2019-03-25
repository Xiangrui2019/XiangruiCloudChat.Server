using System.ComponentModel.DataAnnotations;

namespace XiangruiCloudChat.Server.Models
{
    public class OnlineDevice
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
    }
}
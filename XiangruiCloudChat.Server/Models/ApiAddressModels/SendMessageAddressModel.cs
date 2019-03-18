using System.ComponentModel.DataAnnotations;

namespace XiangruiCloudChat.Server.Models.ApiAddressModels
{
    public class SendMessageAddressModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace XiangruiCloudChat.Server.Models.ApiAddressModels
{
    public class UploadFileAddressModel
    {
        [Required]
        [FromQuery]
        public int ConversationId { get; set; }
    }
}

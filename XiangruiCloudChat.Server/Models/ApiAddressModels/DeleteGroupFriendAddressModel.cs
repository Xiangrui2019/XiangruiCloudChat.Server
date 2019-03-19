using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace XiangruiCloudChat.Server.Models.ApiAddressModels
{
    public class DeleteGroupFriendAddressModel
    {
        [Required]
        public int GroupId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string GroupRootId { get; set; }
    }
}
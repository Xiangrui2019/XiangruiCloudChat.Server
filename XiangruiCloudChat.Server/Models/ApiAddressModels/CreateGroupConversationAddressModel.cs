using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Attributes;

namespace XiangruiCloudChat.Server.Models.ApiAddressModels
{
    public class CreateGroupConversationAddressModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(25)]
        [NoSpace]
        [Display(Name ="新群聊名称")]
        public string GroupName { get; set; }

        [MaxLength(100, ErrorMessage = "您的密码太长了.")]
        [DataType(DataType.Password)]
        [NoSpace]
        public string JoinPassword { get; set; }
    }
}

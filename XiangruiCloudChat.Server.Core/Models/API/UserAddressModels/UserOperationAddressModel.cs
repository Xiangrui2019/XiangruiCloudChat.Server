using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XiangruiCloudChat.Server.Core.API.UserAddressModels
{
    public class UserOperationAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string OpenId { get; set; }

    }
}

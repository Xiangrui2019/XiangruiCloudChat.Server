using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XiangruiCloudChat.Server.Core.API.UserAddressModels
{
    public class DropGrantedAppsAddressModel : UserOperationAddressModel
    {
        [Required]
        public string AppIdToDrop { get; set; }
    }
}

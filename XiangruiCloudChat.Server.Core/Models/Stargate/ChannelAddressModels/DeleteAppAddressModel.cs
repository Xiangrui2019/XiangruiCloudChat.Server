using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Models.Stargate.ChannelAddressModels
{
    public class DeleteAppAddressModel
    {
        [Required]
        public string AppId { get; set; }
        [Required]
        public string AccessToken { get; set; }
    }
}

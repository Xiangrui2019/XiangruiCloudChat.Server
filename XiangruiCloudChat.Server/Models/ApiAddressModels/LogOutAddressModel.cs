using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Models.ApiAddressModels
{
    public class LogOutAddressModel
    {
        [Required]
        public int DeviceId { get; set; }
    }
}

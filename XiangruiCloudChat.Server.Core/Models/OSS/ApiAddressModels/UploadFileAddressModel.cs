using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Models.OSS.ApiAddressModels
{
    public class UploadFileAddressModel : CommonAddressModel
    {
        [Required]
        [Range(1, 18250)]
        public int AliveDays { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Attributes;

namespace XiangruiCloudChat.Server.Core.Models.OSS.ApiAddressModels
{
    public class CreateBucketAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        [MaxLength(25)]
        [MinLength(5)]
        [NoSpace]
        [NoDot]
        public string BucketName { get; set; }
        public bool OpenToRead { get; set; }
        public bool OpenToUpload { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Models.OSS.SecretAddressModels
{
    public class GenerateAddressModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string AccessToken { get; set; }

        [Range(1, 1000000)]
        public int MaxUseTimes { get; set; } = 1;
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Core.API.OAuthAddressModels
{
    public class CodeToOpenIdAddressModel
    {
        [Required]
        public virtual string AccessToken { get; set; }
        [Required]
        public virtual int Code { get; set; }
        [Required]
        public virtual string grant_type { get; set; }
    }
}

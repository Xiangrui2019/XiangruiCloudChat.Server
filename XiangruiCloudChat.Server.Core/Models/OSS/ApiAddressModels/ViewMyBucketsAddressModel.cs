﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Core.Models.OSS.ApiAddressModels
{
    public class ViewMyBucketsAddressModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}

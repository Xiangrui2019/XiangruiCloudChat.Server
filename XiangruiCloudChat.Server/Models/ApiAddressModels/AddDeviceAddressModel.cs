﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Models.ApiAddressModels
{
    public class AddDeviceAddressModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string PushEndpoint { get; set; }
        [Required]
        public string PushP256DH { get; set; }
        [Required]
        public string PushAuth { get; set; }
    }
}

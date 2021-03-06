﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Models.ApiAddressModels
{
    public class SearchFriendsAddressModel
    {
        [MinLength(1)]
        [Required]
        public string NickName { get; set; }

        public int Take { get; set; } = 20;
    }
}

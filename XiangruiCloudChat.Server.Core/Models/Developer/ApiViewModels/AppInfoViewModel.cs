﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Core.Models.Developer.ApiViewModels
{
    public class AppInfoViewModel : AiurProtocol
    {
        public App App { get; set; }
    }
}

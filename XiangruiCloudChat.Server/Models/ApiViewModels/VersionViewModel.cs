﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Models.ApiViewModels
{
    public class VersionViewModel : AiurProtocol
    {
        public string LatestVersion { get; set; }
        public string OldestSupportedVersion { get; set; }
        public string DownloadAddress { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Core.Models.OSS.ApiViewModels
{
    public class ViewMyBucketsViewModel : AiurProtocol
    {
        public string AppId { get; set; }
        public IEnumerable<Bucket> Buckets { get; set; }
    }
}

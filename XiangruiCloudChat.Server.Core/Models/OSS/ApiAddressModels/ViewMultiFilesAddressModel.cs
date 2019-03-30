using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Models.OSS.ApiAddressModels
{
    public class ViewMultiFilesAddressModel
    {
        [Required]
        public string Ids { get; set; }
    }
}

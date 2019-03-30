using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XiangruiCloudChat.Server.Core.Archon
{
    public class AccessTokenAddressModel
    {
        [Required]
        public virtual string AppId { get; set; }
        [Required]
        public virtual string AppSecret { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Core.Models.OSS.ApiViewModels
{
    public class ViewAllFilesViewModel : AiurProtocol
    {
        public virtual int BucketId { get; set; }
        public IEnumerable<OSSFile> AllFiles { get; set; }
    }
}

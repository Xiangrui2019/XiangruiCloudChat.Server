using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Core.Models.OSS.ApiViewModels
{
    public class UploadFileViewModel : AiurProtocol
    {
        public int FileKey { get; set; }
        public string Path { get; set; }
    }
}

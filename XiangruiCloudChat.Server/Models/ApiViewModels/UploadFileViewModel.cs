using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiangruiCloudChat.Server.Core.Models;

namespace XiangruiCloudChat.Server.Models.ApiViewModels
{
    public class UploadFileViewModel : AiurProtocol
    {
        public string SavedFileName { get; set; }
        public int FileKey { get; set; }
        public long FileSize { get; set; }
    }

    public class UploadImageViewModel : AiurProtocol
    {
        public int FileKey { get; set; }
        public string DownloadPath { get; set; }
    }
}

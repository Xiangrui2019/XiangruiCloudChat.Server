using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XiangruiCloudChat.Server.Core.Models
{
    public interface IPusher<T>
    {
        bool Connected { get; }
        Task Accept(HttpContext context);
        Task SendMessage(string Message);
    }
}

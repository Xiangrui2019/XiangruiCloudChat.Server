using System.Threading.Tasks;
using Aiursoft.Pylon.Exceptions;
using Aiursoft.Pylon.Models;
using Aiursoft.Pylon.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace XiangruiCloudChat.Server.Services
{
    public class VersionChecker
    {
        private IConfiguration Configuration { get; }

        public VersionChecker(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string CheckVersion()
        {
            return Configuration["CurrentVersion"].ToString();
        }
    }
}
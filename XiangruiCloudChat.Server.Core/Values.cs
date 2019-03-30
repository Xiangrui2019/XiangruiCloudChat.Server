using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace XiangruiCloudChat.Server.Core
{
    public static class Values
    {
        public static string ProjectName = "XiangruiCloudChat";
        public static string Schema = "https";
        public static string WsSchema = "wss";
        public static long MaxFileSize = 1000 * 1024 * 1024;

        public static KeyValuePair<string, string> DirectShowString => new KeyValuePair<string, string>("show", "direct");
        public static PasswordOptions PasswordOptions => new PasswordOptions
        {
            RequireDigit = false,
            RequiredLength = 6,
            RequireLowercase = false,
            RequireUppercase = false,
            RequireNonAlphanumeric = false
        };
    }
}

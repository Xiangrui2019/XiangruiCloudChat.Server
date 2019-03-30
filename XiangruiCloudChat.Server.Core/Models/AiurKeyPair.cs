﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using XiangruiCloudChat.Server.Core.Services;

namespace XiangruiCloudChat.Server.Core.Models
{
    public class AiurKeyPair
    {
        public RSAParameters PublicKey { get; set; }
        public RSAParameters PrivateKey { get; set; }

        public AiurKeyPair(IConfiguration configuration)
        {
            configuration = configuration.GetSection("Key");
            PublicKey = new RSAParameters
            {
                Exponent = configuration["Exponent"].Base64ToBytes(),
                Modulus = configuration["Modulus"].Base64ToBytes()
            };

            PrivateKey = new RSAParameters
            {
                D = configuration["D"].Base64ToBytes(),
                DP = configuration["DP"].Base64ToBytes(),
                DQ = configuration["DQ"].Base64ToBytes(),
                Exponent = configuration["Exponent"].Base64ToBytes(),
                InverseQ = configuration["InverseQ"].Base64ToBytes(),
                Modulus = configuration["Modulus"].Base64ToBytes(),
                P = configuration["P"].Base64ToBytes(),
                Q = configuration["Q"].Base64ToBytes()
            };
        }
    }
}

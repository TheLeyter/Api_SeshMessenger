using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApiSesh.Model.ServerModel
{
    public class ConfirmToken
    {
        public string Token { get; set; }

        public long expiryTime { get; set; }

        public ConfirmToken(string Token,int liveTime)
        {
            this.Token = Token;
            this.expiryTime = new DateTimeOffset(DateTime.Now.AddMinutes(liveTime)).ToUnixTimeSeconds();
        }
    }
}

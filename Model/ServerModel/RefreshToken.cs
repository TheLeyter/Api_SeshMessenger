using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AuthApiSesh.Model;

namespace AuthApiSesh.Model
{
    public class RefreshToken
    {
        [Key]
        public long id { get; set; }
        [Required]
        public virtual User user { get; set; }
        [Required]
        public string token { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime timeCreated { get; set; }

        public string deviceIp { get; set; }

        public string deviceOs { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime lastActivity { get; set; }

        public RefreshToken()
        {
            timeCreated = DateTime.UtcNow;
            lastActivity = timeCreated;
        }

        public RefreshToken(User user, string token, string deviceIp, string deviceOs)
        {
            timeCreated = DateTime.UtcNow;
            lastActivity = timeCreated;
            this.user = user;
            this.token = token;
            this.deviceIp = deviceIp;
            this.deviceOs = deviceOs;
        }
    }
}
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace AuthApiSesh.Model{

    public class User {
        [Key]
        public long id { get; set; }

        [Range(2,30)]
        public string username { get; set; }

        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Range(3,30)]
        public string firstName { get; set; }

        [Range(3,30)]
        public string lastName { get; set; }

        [DataType(DataType.Password)]
        public string password { get; set; }

        public string avatar { get; set; }

        [DataType(DataType.Date)]
        public DateTime registerDate { get; private set; }

        [DefaultValue(false)]
        public bool confirm { get; set; }

        public User(){
            this.registerDate = DateTime.UtcNow;
        }

        public User(string username, string email, string firstName, string lastName, string password)
        {
            this.username = username;
            this.email = email;
            this.firstName = firstName;
            this.lastName = lastName;
            this.password = password;
            this.registerDate = DateTime.UtcNow;
        }

        public string ToRefreshToken()
        {
            string payload = "{";
            payload += "\"" + nameof(this.id) + "\"" + ":" + this.id.ToString() + ",";
            payload += "\"" + nameof(this.username) + "\"" + ":" + "\"" + this.username + "\""+",";
            payload += "\"" + nameof(this.email) + "\"" + ":" + "\"" + this.email + "\"";
            payload += "}";

            return payload;
        }

    }

}
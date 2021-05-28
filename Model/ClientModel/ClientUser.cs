using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace AuthApiSesh.ClientModel{

    public class ClientUser {

        public string username { get; set; }

        public string? email { get; set; }

        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public string password { get; set; }


        public ClientUser(){ }

        public ClientUser(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public ClientUser(string username, string email, string firstName, string lastName, string password)
        {
            this.username = username;
            this.email = email;
            this.firstName = firstName;
            this.lastName = lastName;
            this.password = password;
        }

    }

}
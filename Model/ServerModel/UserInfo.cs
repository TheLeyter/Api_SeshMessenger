using System;

namespace AuthApiSesh.Model.ServerModel
{
    public class UserInfo
    {
        public long id { get; set; }
        public string email { get; set; }

        public string username { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public UserInfo(User user)
        {
            this.id = user.id;
            this.email = user.email;
            this.username = user.username;
            this.firstName = user.firstName;
            this.lastName = user.lastName;
        }

        

    }
}

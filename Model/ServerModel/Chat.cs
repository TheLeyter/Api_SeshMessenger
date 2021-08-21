using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApiSesh.Model.ServerModel
{
    public class Chat
    {
        [Key]
        public long id { get; set; }

        public virtual User userCreator { get; set; }
        public virtual User user { get; set; }
        public virtual List<Message> messages { get; set; }
        public DateTime createdTime { get; set; }

        public Chat(){}

        public Chat(User userCreator, User user)
        {
            createdTime = DateTime.UtcNow;

            this.userCreator = userCreator;
            this.user = user;
        }
    }
}

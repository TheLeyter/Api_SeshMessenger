using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AuthApiSesh.Model;

namespace AuthApiSesh.Model{
    public class Friend{

        [Key]
        public long id { get; set; }
        [Required]
        public User user1 { get; set; }
        [Required]
        public User user2 { get; set; }
        [DefaultValue(false)]
        public bool confirm { get; set; }

        public Friend(){}

        public Friend(User User1, User User2){
            this.user1 = User1;
            this.user2 = User2;
        }
    }
}
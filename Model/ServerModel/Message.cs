using AuthApiSesh.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApiSesh.Model.ServerModel
{
    public class Message
    {
        [Key]
        public long id { get; set; }
        public long chatId { get; set; }
        public virtual Chat chat { get; set; }
        public long userId { get; set; }
        public virtual User user { get; set; }
        public MessageType type { get; set; }
        public string payload { get; set; }
        public DateTime timeSend { get; set; }
        public MessageStatus status { get; set; }

    }
}

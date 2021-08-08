using System;
using System.Collections.Generic;
using System.Text;

namespace PowerBot.Core.Models
{
    public class PowerbotUser
    {
        public long Id { get; set; }
        public long TelegramId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime ActiveAt { get; set; }
        public UserAccess UserAccess { get; set; }

        public string FullName => FirstName + " " + LastName;
        public bool IsBanned => UserAccess == UserAccess.Ban;
    }
}

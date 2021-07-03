using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Core.Models
{
    public class Chat
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime ActiveAt { get; set; }
    }
}

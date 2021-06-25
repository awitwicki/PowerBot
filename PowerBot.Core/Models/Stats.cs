using System;
using System.Collections.Generic;
using System.Text;

namespace PowerBot.Core.Models
{
    public class Stats
    {
        public long Id { get; set; }
        public DateTime DateTime { get; set; }
        public ActionType ActionType { get; set; }
    }

    public enum ActionType
    {
        Message,
        Error
    }
}

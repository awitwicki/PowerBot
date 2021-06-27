using System;
using System.Collections.Generic;
using System.Text;

namespace PowerBot.Core.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string MessageText{ get; set; }
        public LogLevel LogLevel { get; set; }
    }
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Critical
    }
}

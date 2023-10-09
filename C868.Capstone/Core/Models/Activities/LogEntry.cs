using System;
using C868.Capstone.Core.Models.Data;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace C868.Capstone.Core.Models.Activities
{
    public class LogEntry
    {
        [PrimaryKey, AutoIncrement]
        public int LogEntryId { get; set; }
        
        public LogMessageType Type { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }

        [ForeignKey(typeof(User))]
        public int UserId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public User User { get; set; }

        public LogEntry()
        {
            LogEntryId = 0;
            UserId = 0;

            Type = LogMessageType.None;
            Message = string.Empty;
            Created = DateTime.Now;
            
            User = null;
        }
    }
}
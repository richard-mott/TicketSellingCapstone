using System;
using SQLite;

namespace C868.Capstone.Core.Models.Data
{
    public class Auditorium
    {
        [PrimaryKey, AutoIncrement]
        public int AuditoriumId { get; set; }
        
        public string Name { get; set; }
        public int Capacity { get; set; }
        public DateTime Created { get; set; }

        public Auditorium()
        {
            AuditoriumId = 0;
            
            Name = string.Empty;
            Capacity = 0;
            Created = DateTime.Now;
        }

        public Auditorium Clone()
        {
            return new Auditorium
            {
                AuditoriumId = AuditoriumId,
                
                Name = Name,
                Capacity = Capacity,
                Created = Created
            };
        }
    }
}

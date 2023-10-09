using System;
using SQLite;

namespace C868.Capstone.Core.Models.Data
{
    public class Movie
    {
        [PrimaryKey, AutoIncrement]
        public int MovieId { get; set; }

        public string Name { get; set; }
        public Rating Rating { get; set; }
        public int RunTime { get; set; }
        public string Cast { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set;  }
        
        public Movie()
        {
            MovieId = 0;
            
            Name = string.Empty;
            Rating = Rating.None;
            RunTime = 0;
            Cast = string.Empty;
            Description = string.Empty;
            Created = DateTime.Now;
        }

        public Movie Clone()
        {
            return new Movie
            {
                MovieId = MovieId,
                
                Name = Name,
                Rating = Rating,
                RunTime = RunTime,
                Cast = Cast,
                Description = Description,
                Created = Created
            };
        }
    }
}

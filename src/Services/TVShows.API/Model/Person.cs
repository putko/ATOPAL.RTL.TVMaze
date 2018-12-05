using System;
using System.Collections.Generic;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Model
{
    public class Person : EntityBase
    {
        public Person()
            : base()
        {
            Shows = new List<ShowPerson>();
        }

        public Person(int id)
            : base(id) 
        {
            Shows = new List<ShowPerson>();
        }

        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }
        public int TVMazeId { get; set; }

        public virtual IList<ShowPerson> Shows { get; set; }
    }
}
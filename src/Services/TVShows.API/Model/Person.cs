namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model
{
    using System;
    using System.Collections.Generic;

    public sealed class Person : EntityBase
    {
        public Person()
        {
            this.Shows = new List<ShowPerson>();
        }

        public Person(int id)
            : base(id: id)
        {
            this.Shows = new List<ShowPerson>();
        }

        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }
        public int TVMazeId { get; set; }

        public IList<ShowPerson> Shows { get; set; }
    }
}
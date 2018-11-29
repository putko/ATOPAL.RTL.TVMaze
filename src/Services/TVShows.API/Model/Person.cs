using System;
using System.Collections.Generic;

namespace TVShows.API.Model
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

        public DateTime BirthDate { get; set; }

        public virtual IList<ShowPerson> Shows { get; set; }


    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TVShows.API.Model
{
    public class Show : EntityBase
    {
        public Show()
           : base()
        {
            Persons = new List<ShowPerson>();
        }

        public Show(int id)
            : base(id)
        {
            Persons = new List<ShowPerson>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual IList<ShowPerson> Persons { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}

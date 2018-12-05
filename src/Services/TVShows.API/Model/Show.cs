using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AUTOPOAL.RTL.TVMaze.Services.TVShows.API.Model
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
        public int TVMazeId { get; set; }

        public long Timestamp { get; set; }
    }
}

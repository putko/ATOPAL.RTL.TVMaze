namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model
{
    using System.Collections.Generic;

    public sealed class Show : EntityBase
    {
        public Show()
        {
            this.Persons = new List<ShowPerson>();
        }

        public Show(int id)
            : base(id: id)
        {
            this.Persons = new List<ShowPerson>();
        }

        public string Name { get; set; }

        public IList<ShowPerson> Persons { get; set; }
        public int TVMazeId { get; set; }

        public long Timestamp { get; set; }
    }
}
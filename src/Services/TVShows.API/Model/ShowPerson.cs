namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model
{
    /// <summary>
    /// This class exists because many-to-many relation is not supported yet by ef .net core. instead of many-to-many we make double one-to-many relation over this class.
    /// </summary>
    public class ShowPerson
    {
        public int ShowId { get; set; }
        public Show Show { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
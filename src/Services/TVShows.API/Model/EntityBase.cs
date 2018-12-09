namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model
{
    public abstract class EntityBase
    {
        protected EntityBase(int id)
        {
            this.Id = id;
        }

        protected EntityBase()
        {
        }

        public int Id { get; set; }
    }
}
namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IGenericRepository<TEntity> where TEntity : EntityBase
    {
        IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        Task<TEntity> GetById(int id);
        void Insert(TEntity entity);
        void Delete(int id);
        void Delete(TEntity id);
        void Update(TEntity entity);
    }
}
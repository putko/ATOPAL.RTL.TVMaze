namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.Model
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : EntityBase
    {
        internal DbContext Context;
        internal DbSet<TEntity> DbSet;

        public GenericRepository(DbContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = this.DbSet;

            if (filter != null)
            {
                query = query.Where(predicate: filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (separator: new[] {','}, options: StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(navigationPropertyPath: includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(arg: query);
            }

            return query;
        }

        public virtual async Task<TEntity> GetById(int id)
        {
            return await this.DbSet.FindAsync(id);
        }

        public virtual void Insert(TEntity entity)
        {
            this.DbSet.Add(entity: entity);
            this.Context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var entityToDelete = this.DbSet.Find(id);
            this.Delete(entityToDelete: entityToDelete);
            this.Context.SaveChanges();
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (this.Context.Entry(entity: entityToDelete).State == EntityState.Detached)
            {
                this.DbSet.Attach(entity: entityToDelete);
            }

            this.DbSet.Remove(entity: entityToDelete);
            this.Context.SaveChanges();
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            this.DbSet.Attach(entity: entityToUpdate);
            this.Context.Entry(entity: entityToUpdate).State = EntityState.Modified;
            this.Context.SaveChanges();
        }
    }
}
namespace AUTOPAL.RTL.TVMaze.Services.TVShows.API.ViewModel
{
    using System.Collections.Generic;

    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Count = count;
            this.Data = data;
        }

        public int PageIndex { get; }

        public int PageSize { get; }

        public long Count { get; }

        public IEnumerable<TEntity> Data { get; }
    }
}
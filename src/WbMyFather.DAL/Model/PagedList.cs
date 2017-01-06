using System.Collections.Generic;
using System.Linq;

namespace WbMyFather.DAL.Model
{
    public class PagedList<T> : List<T>, IPagedList
    {
        public int TotalCount { get; }
        public int PageCount { get; }
        public int PageIndex { get; }
        public int PageSize { get; }

        public IEnumerable<T> Data => this.AsEnumerable();

        public PagedList(int pageIndex, int pageSize, int pageCount, int totalCount)
        {
            TotalCount = totalCount;
            PageCount = pageCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public PagedList(IQueryable<T> source, int page, int pageSize)
        {
            TotalCount = source.Count();
            PageCount = GetPageCount(pageSize, TotalCount);
            PageIndex = page < 1 ? 0 : page - 1;
            PageSize = pageSize;

            AddRange(source.Skip(PageIndex * PageSize).Take(PageSize).ToList());
        }

        private static int GetPageCount(int pageSize, int totalCount)
        {
            if (pageSize == 0)
                return 0;

            var remainder = totalCount % pageSize;
            return (totalCount / pageSize) + (remainder == 0 ? 0 : 1);
        }
    }
}

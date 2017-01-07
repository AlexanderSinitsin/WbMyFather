using System.Collections.Generic;

namespace WebSite.Models.Shared
{
    public class PagedList<T>
    {
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}

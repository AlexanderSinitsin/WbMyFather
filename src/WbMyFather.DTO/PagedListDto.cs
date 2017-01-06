using System.Collections.Generic;

namespace WbMyFather.DTO
{
    public class PagedListDto<T>
    {
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}

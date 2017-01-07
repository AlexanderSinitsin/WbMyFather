using System.Collections.Generic;

namespace WbMyFather.DTO.Models.Requests
{
    public class GetSortedFilteredPaging
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        /// <summary>
        /// Перечисление полей сортировки (поля разделяются запятой) именя полей должны совпадать с именем поля в модели
        /// </summary>
        public string SortField { get; set; }

        /// <summary> Порядок сортировки </summary>
        /// <remarks>0-Ascending / 1-Descending</remarks>
        public int SortOrder { get; set; }

        public IEnumerable<FilterRequest> Filters { get; set; }

        public GetSortedFilteredPaging()
        {
            PageSize = 10;
            PageNumber = 1;
        }
    }
}

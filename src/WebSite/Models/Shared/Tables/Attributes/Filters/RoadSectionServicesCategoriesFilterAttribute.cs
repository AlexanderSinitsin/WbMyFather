using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WbMyFather.DTO.Models.Requests;
using WebSite.Models.Shared.Tables.Attributes.Filters.Base;

namespace WebSite.Models.Shared.Tables.Attributes.Filters
{
    /// <summary>
    /// Для автодорог специализированный атрибут
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RoadSectionServicesCategoriesFilterAttribute : IntMatchFilterAttribute
    {
        public RoadSectionServicesCategoriesFilterAttribute() : base(null)
        {
        }

        public override FilterRequest GetFilter(string search)
        {
            if (string.IsNullOrEmpty(search)) return null;

            int filterValue;
            if (int.TryParse(search, out filterValue))
            {
                return new FilterRequest { Expression = "SectionServices.Any(SServiceCategories.Any(Value = @0))", Values = new object[] {filterValue} };
            }

            return null;
        }
    }
}

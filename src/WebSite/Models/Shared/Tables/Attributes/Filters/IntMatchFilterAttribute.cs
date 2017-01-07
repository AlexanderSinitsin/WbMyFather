using System;
using WbMyFather.DTO.Models.Requests;
using WebSite.Models.Shared.Tables.Attributes.Filters.Base;

namespace WebSite.Models.Shared.Tables.Attributes.Filters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IntMatchFilterAttribute : ColumnFilterAttributeBase
    {
        public override string Html => "<input type=\"number\" min=\"1\" class=\"form-control table-column-filter-id\"/>";

        public override FilterRequest GetFilter(string search)
        {
            if (string.IsNullOrEmpty(search)) return null;

            int filterValue;
            if (int.TryParse(search, out filterValue))
            {
                return new FilterRequest { Expression = $"{EntityPropertyName} = @0", Values = new object[] {filterValue} };
            }

            return null;
        }

        public IntMatchFilterAttribute(string entityPropertyName) : base(entityPropertyName)
        {
        }
    }
}

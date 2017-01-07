using System;
using System.Text;
using WbMyFather.DTO.Models.Requests;
using WebSite.Extensions;
using WebSite.Models.Shared.Tables.Attributes.Filters.Base;

namespace WebSite.Models.Shared.Tables.Attributes.Filters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateRangeFilterAttribute : ColumnFilterAttributeBase
    {
        public override string Html => "<div class=\"input-daterange input-group\"><span class=\"input-group-addon white-bg no-borders\">с</span><input class=\"form-control table-column-filter\" id=\"from\" type=\"date\"><span class=\"input-group-addon white-bg no-borders\">по</span><input class=\"form-control table-column-filter\" id=\"to\" type=\"date\"></div>";

        public string ChildsCollectionElementFieldForAny { get; set; }

        private bool IsByChildFilter => !string.IsNullOrEmpty(ChildsCollectionElementFieldForAny);

        public override FilterRequest GetFilter(string search)
        {
            if (string.IsNullOrEmpty(search)) return null;

            if (IsByChildFilter) return FilterByChilds(search);

            var dates = search.Split('|');
            var from = dates[0];
            var to = dates[1];

            var haveFrom = !string.IsNullOrEmpty(from);
            var haveTo = !string.IsNullOrEmpty(to);

            if (haveFrom && haveTo)
            {
                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName} >= @0 && {EntityPropertyName} <= @1",
                    Values = new object[] {DateTime.Parse(from).ToUtcFromUserLocal(), DateTime.Parse(to).AddDays(1).AddSeconds(-1).ToUtcFromUserLocal()}
                };
            }

            if (haveFrom)
            {
                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName} >= @0",
                    Values = new object[] {DateTime.Parse(from).ToUtcFromUserLocal()}
                };
            }

            if (haveTo)
            {
                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName} <= @0",
                    Values = new object[] { DateTime.Parse(to).AddDays(1).AddSeconds(-1).ToUtcFromUserLocal() }
                };
            }

            return null;
        }

        private FilterRequest FilterByChilds(string search)
        {
            var dates = search.Split('|');
            var from = dates[0];
            var to = dates[1];

            var haveFrom = !string.IsNullOrEmpty(from);
            var haveTo = !string.IsNullOrEmpty(to);

            if (haveFrom && haveTo)
            {
                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName}.Any({ChildsCollectionElementFieldForAny} >= @0 && {ChildsCollectionElementFieldForAny} <= @1)",
                    Values = new object[] { DateTime.Parse(from).ToUtcFromUserLocal(), DateTime.Parse(to).ToUtcFromUserLocal() }
                };
            }

            if (haveFrom)
            {
                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName}.Any({ChildsCollectionElementFieldForAny} >= @0)",
                    Values = new object[] { DateTime.Parse(from).ToUtcFromUserLocal() }
                };
            }

            if (haveTo)
            {
                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName}.Any({ChildsCollectionElementFieldForAny} <= @0)",
                    Values = new object[] { DateTime.Parse(to).ToUtcFromUserLocal() }
                };
            }

            return null;
        }

        public DateRangeFilterAttribute(string entityPropertyName) : base(entityPropertyName)
        {
        }
    }
}

using System.Globalization;
using WbMyFather.DTO.Models.Requests;
using WebSite.Models.Shared.Tables.Attributes.Filters.Base;
using System.Text;
using System;
using System.Linq;

namespace WebSite.Models.Shared.Tables.Attributes.Filters
{
    public class DecimalRangeFilterAttribute : ColumnFilterAttributeBase
    {
        public DecimalRangeFilterAttribute(string entityPropertyName) : base(entityPropertyName)
        {
        }

        public override string Html => "<div class=\"input-daterange input-group\"><span class=\"input-group-addon white-bg no-borders\">с</span><input class=\"form-control table-column-filter\" id=\"from\" type=\"number\" step=\"0.001\"><span class=\"input-group-addon white-bg no-borders\">по</span><input class=\"form-control table-column-filter\" id=\"to\" type=\"number\" step=\"0.001\"></div>";

        public string RelatedEntityPropertyNames { get; set; }

        public override FilterRequest GetFilter(string search)
        {
            if (string.IsNullOrEmpty(search)) return null;

            var range = search.Split('|');
            var from = range[0];
            var to = range[1];

            var haveFrom = !string.IsNullOrEmpty(from);
            var haveTo = !string.IsNullOrEmpty(to);

            var propNames = RelatedEntityPropertyNames?.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (haveFrom && haveTo)
            {
                if (propNames != null && propNames.Any())
                {
                    var anyExpression = new StringBuilder();

                    for (var i = 0; i < propNames.Length; i++)
                    {
                        if (i != 0)
                        {
                            anyExpression.Append(" OR ");
                        }

                        anyExpression.Append($"{propNames[i]} >= @0 && {propNames[i]} <= @1");
                    }

                    return new FilterRequest
                    {
                        Expression = $"{EntityPropertyName}.Any({anyExpression})",
                        Values = new object[] { decimal.Parse(from, CultureInfo.InvariantCulture), decimal.Parse(to, CultureInfo.InvariantCulture) }
                    };
                }

                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName} >= @0 && {EntityPropertyName} <= @1",
                    Values = new object[] { decimal.Parse(from, CultureInfo.InvariantCulture), decimal.Parse(to, CultureInfo.InvariantCulture) }
                };
            }

            if (haveFrom)
            {
                if (propNames != null && propNames.Any())
                {
                    var anyExpression = new StringBuilder();

                    for (var i = 0; i < propNames.Length; i++)
                    {
                        if (i != 0)
                        {
                            anyExpression.Append(" OR ");
                        }

                        anyExpression.Append($"{propNames[i]} >= @0");
                    }

                    return new FilterRequest
                    {
                        Expression = $"{EntityPropertyName}.Any({anyExpression})",
                        Values = new object[] { decimal.Parse(from, CultureInfo.InvariantCulture) }
                    };
                }

                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName} >= @0",
                    Values = new object[] { decimal.Parse(from, CultureInfo.InvariantCulture) }
                };
            }

            if (haveTo)
            {
                if (propNames != null && propNames.Any())
                {
                    var anyExpression = new StringBuilder();

                    for (var i = 0; i < propNames.Length; i++)
                    {
                        if (i != 0)
                        {
                            anyExpression.Append(" OR ");
                        }

                        anyExpression.Append($"{propNames[i]} >= @0");
                    }

                    return new FilterRequest
                    {
                        Expression = $"{EntityPropertyName}.Any({anyExpression})",
                        Values = new object[] { decimal.Parse(to, CultureInfo.InvariantCulture) }
                    };
                }

                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName} <= @0",
                    Values = new object[] { decimal.Parse(to, CultureInfo.InvariantCulture) }
                };
            }

            return null;
        }
    }
}

using System;
using System.Linq;
using System.Text;
using WbMyFather.DTO.Models.Requests;
using WebSite.Models.Shared.Tables.Attributes.Filters.Base;

namespace WebSite.Models.Shared.Tables.Attributes.Filters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StringContainsFilterAttribute : ColumnFilterAttributeBase
    {
        public override string Html => "<input type=\"text\" class=\"form-control table-column-filter\"/>";

        public string RelatedEntityPropertyNames { get; set; }

        public override FilterRequest GetFilter(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return null;
            }

            var propNames = RelatedEntityPropertyNames?.Split(new []{',', ' ', ';'}, StringSplitOptions.RemoveEmptyEntries);

            if (propNames != null && propNames.Any())
            {
                var anyExpression = new StringBuilder();

                for (var i = 0; i < propNames.Length; i++)
                {
                    if (i != 0)
                    {
                        anyExpression.Append(" OR ");
                    }

                    anyExpression.Append($"{propNames[i]}.Contains(\"{search}\")");
                }

                return new FilterRequest
                {
                    Expression = $"{EntityPropertyName}.Any({anyExpression})",
                    Values = new object[0]
                };
            }

            return new FilterRequest
            {
                Expression = $"{EntityPropertyName}.Contains(\"{search}\")",
                Values = new object[0]
            };
        }

        public StringContainsFilterAttribute(string entityPropertyName) : base(entityPropertyName)
        {
        }
    }
}

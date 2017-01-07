using System;
using WbMyFather.DTO.Models.Requests;

namespace WebSite.Models.Shared.Tables.Attributes.Filters.Base
{
    public abstract class ColumnFilterAttributeBase : Attribute, IColumnFilter
    {
        protected ColumnFilterAttributeBase(string entityPropertyName)
        {
            EntityPropertyName = entityPropertyName;
        }

        public string EntityPropertyName { get; }
        
        public abstract string Html { get; }

        public string InitJsMethod => $"{char.ToLower(GetType().Name[0]) + GetType().Name.Substring(1)}Init";

        public abstract FilterRequest GetFilter(string search);
    }
}

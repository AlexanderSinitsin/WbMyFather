using WbMyFather.DTO.Models.Requests;

namespace WebSite.Models.Shared.Tables.Attributes.Filters.Base
{
    public interface IColumnFilter
    {
        /// <summary>
        /// Имя поля в entity, по которому фильтруем
        /// </summary>
        string EntityPropertyName { get; }

        string Html { get; }

        string InitJsMethod { get; }

        FilterRequest GetFilter(string search);
    }
}

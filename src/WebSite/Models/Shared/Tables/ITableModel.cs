using System.Collections.Generic;

namespace WebSite.Models.Shared.Tables
{
    public interface ITableModel
    {
        /// <summary>
        /// Идентификатор таблицы
        /// </summary>
        string TableId { get; }

        /// <summary>
        /// Коллекция представлений колонок
        /// </summary>
        IList<ColumnMetadata> Columns { get; }

        /// <summary>
        /// Коллекция кнопок
        /// </summary>
        IList<TableButton> Buttons { get; }

        /// <summary>
        /// Имя функции обработчика клика по строке
        /// </summary>
        string OnClickFunction { get; }

        /// <summary>
        /// Определяет возможность множественного выбора строк
        /// </summary>
        bool IsMultipleSelect { get; }
    }
}

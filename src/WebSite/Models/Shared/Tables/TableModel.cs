using System.Collections.Generic;


namespace WebSite.Models.Shared.Tables
{
    public class TableModel<TModel> : ITableModel where TModel : class
    {
        public TableModel(IList<ColumnMetadata> columns, IList<TableButton> buttons, string onClickFunction, bool isMultipleSelect)
        {
            TableId = $"{typeof (TModel).Name}Table";
            Columns = columns;
            Buttons = buttons;
            OnClickFunction = onClickFunction;
            IsMultipleSelect = isMultipleSelect;
        }

        /// <summary>
        /// Идентификатор таблицы
        /// </summary>
        public string TableId { get; }

        /// <summary>
        /// Коллекция представлений колонок
        /// </summary>
        public IList<ColumnMetadata> Columns { get; }

        /// <summary>
        /// Коллекция кнопок
        /// </summary>
        public IList<TableButton> Buttons { get; }

        /// <summary>
        /// Имя функции обработчика клика по строке
        /// </summary>
        public string OnClickFunction { get; }

        /// <summary>
        /// Определяет возможность множественного выбора строк
        /// </summary>
        public bool IsMultipleSelect { get; }
    }
}

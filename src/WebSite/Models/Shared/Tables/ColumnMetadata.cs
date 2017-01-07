using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using WebSite.Models.Shared.Tables.Attributes;
using WebSite.Models.Shared.Tables.Attributes.Filters.Base;

namespace WebSite.Models.Shared.Tables
{
    public class ColumnMetadata
    {
        private readonly PropertyInfo propertyInfo;
        private readonly Func<object, object> formatFunc;


        public ColumnMetadata(PropertyInfo propertyInfo, Func<object, string> formatFunc = null)
        {
            this.propertyInfo = propertyInfo;
            Name = propertyInfo.Name;

            HeaderName = propertyInfo.GetCustomAttribute<DisplayAttribute>(true)?.Name ?? propertyInfo.Name;


            IsSortable = propertyInfo.IsDefined(typeof(SortableAttribute));
            IsHidden = propertyInfo.IsDefined(typeof(HiddenAttribute));
            IsVisible = !IsHidden;

            var filterAttributeType = propertyInfo.CustomAttributes.SingleOrDefault(a => typeof(IColumnFilter).IsAssignableFrom(a.AttributeType))?.AttributeType;
            if (filterAttributeType != null)
            {
                IsFilterable = true;
                ColumnFilter = (IColumnFilter)propertyInfo.GetCustomAttribute(filterAttributeType);
            }

            CssClass = propertyInfo.GetCustomAttribute<TdClassAttribute>(true)?.ClassName;

            //TODO по-хорошему бы атрибутом задавать или регать для типов свойств, если например для DateTime везде одинаково
            this.formatFunc = formatFunc;
        }

        /// <summary>
        /// Название колонки
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Заголовок колонки
        /// </summary>
        public string HeaderName { get; set; }

        /// <summary>
        /// Колонка сортируется, если true
        /// </summary>
        public bool IsSortable { get; set; }

        /// <summary>
        /// Если true, то колонка отображается
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Если true, колонка не отображается и нет возможности сделать ее видимой
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Колонка фильтруется, если true
        /// </summary>
        public bool IsFilterable { get; }

        /// <summary>
        /// Модель фильтра
        /// </summary>
        public IColumnFilter ColumnFilter { get; }

        /// <summary>
        /// CSS класс ячейки таблицы
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Вернет отформатированное значение для этого столбца из модели данных
        /// </summary>
        /// <param name="model">Модель данных</param>
        /// <returns></returns>
        public object Format(object model)
        {
            var value = propertyInfo.GetValue(model);
            return formatFunc != null ? formatFunc(value) : value;
        }
    }
}

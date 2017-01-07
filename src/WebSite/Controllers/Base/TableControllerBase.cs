using Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using WbMyFather.DTO.Models.Requests;
using WebSite.Extensions;
using WebSite.Models.Shared;
using WebSite.Models.Shared.Tables;
using WebSite.Models.Shared.Tables.Requests;

namespace WebSite.Controllers.Base
{
    public abstract class TableControllerBase<TModel> : BaseController where TModel : class
    {
        protected TableControllerBase(ILog log, ILog someService) : base(someService)
        {
            Log = log;
        }

        protected ILog Log { get; }

        protected virtual IList<ColumnMetadata> GetColumnsMetadata()
        {
            return typeof(TModel).GetProperties()
                .Where(p => p.IsDefined(typeof(DisplayAttribute), true))
                .Select(p =>
                {
                    if (p.PropertyType == typeof(DateTime))
                    {
                        return new ColumnMetadata(p, formatFunc: d => ((DateTime)d).ToUserLocalFromUtc().ToString(CultureInfo.CurrentCulture));
                    }
                    if (p.PropertyType == typeof(DateTime?))
                    {
                        return new ColumnMetadata(p, formatFunc: d => ((DateTime?)d).ToUserLocalFromUtc()?.ToString(CultureInfo.CurrentCulture));
                    }

                    return new ColumnMetadata(p);
                })
                .ToList();
        }


        protected abstract Task<PagedList<TModel>> GetModels(GetSortedFilteredPaging pagingRequest, IEnumerable<Column> columns);
        protected abstract string OnClickFunction { get; }
        protected abstract IList<TableButton> TableButtons { get; }

        protected virtual TableModel<TModel> CreateTableModel()
        {
            var columns = GetColumnsMetadata();
            var model = new TableModel<TModel>(columns, TableButtons, OnClickFunction, IsMultipleSelect);
            SetColumnsVisibilitySettings(model);
            return model;
        }

        [ChildActionOnly]
        public virtual ActionResult Index()
        {
            return PartialView("_TablePartial", CreateTableModel());
        }

        public async Task<JsonResult> TableData(int draw,
                                                IDictionary<string, string> columns,
                                                IDictionary<string, string> order,
                                                int start,
                                                int length,
                                                IDictionary<string, string> search
                                                )
        {
            try
            {
                var request = new TableDataRequest(draw, columns, order, start, length, search);
                var orderDirection = request.Order.Dir == "asc" ? 0 : 1;

                var properties = GetColumnsMetadata();

                var filters = new List<FilterRequest>();
                for (var i = 1; i < request.Columns.Count; i++)
                {
                    var column = request.Columns[i];
                    if (column.Searchable && !string.IsNullOrEmpty(column.Search.Value))
                    {
                        filters.Add(properties[i - 1].ColumnFilter.GetFilter(column.Search.Value));
                    }
                }

                filters = filters.Where(f => f != null).ToList();

                var pagingRequest = new GetSortedFilteredPaging
                {
                    PageNumber = request.Start / request.Length + 1,
                    PageSize = request.Length,
                    SortField = properties[request.Order.Column - 1].Name,
                    SortOrder = orderDirection,
                    Filters = filters
                };

                var pageModel = await GetModels(pagingRequest, request.Columns);

                var response = new
                {
                    //TODO в таблице можно биндить имена столбцов и биндить список моделей, вместо строк.
                    //В первой колонке нужен null, там чекбоксы
                    data = pageModel.Data.Select(m => properties.ToDictionary(p => p.Name, p => p.Format(m))),
                    recordsTotal = pageModel.TotalCount,
                    recordsFiltered = pageModel.TotalCount
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (SecurityException ex)
            {
                throw ex;
            }
            catch (Exception exception)
            {
                Log.Error($"Ошибка при загрузке данных для таблицы {typeof(TModel).Name}", exception);
                return Json(new { error = "Ошибка при загрузке данных", recordsTotal = 0, recordsFiltered = 0, data = new string[] { } }, JsonRequestBehavior.AllowGet);
            }
        }

        protected virtual bool IsMultipleSelect => true;

        [HttpPost]
        public async Task<JsonResult> SetColumnVisibility(string tableName, string columnName, bool isVisible)
        {
            var columnId = $"{tableName}.{columnName}";
            var sessionKey = $"{GlobalConsts.ColumnVisibility}_{columnId}";

            SessionManager.Set(sessionKey, isVisible);

            /*if (User.Identity.IsAuthenticated)
            {
                var setting = new UserSettingDto
                {
                    Group = GlobalConsts.ColumnVisibility,
                    Key = columnId,
                    Value = isVisible.ToString()
                };
                await UserService.UpdateSettings(User.Id, new[] { setting });
            }*/

            return Json(0);
        }

        /// <summary>
        /// Установит видимость столюцов исходя из настроек пользователя
        /// </summary>
        /// <param name="model">Модель таблицы</param>
        /// <returns></returns>
        private ITableModel SetColumnsVisibilitySettings(ITableModel model)
        {
            foreach (var column in model.Columns)
            {
                var columnId = $"{model.TableId}.{column.Name}";
                var sessionKey = $"{GlobalConsts.ColumnVisibility}_{columnId}";
                column.IsVisible = SessionManager.Get(sessionKey, column.IsVisible);
            }
            return model;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Security;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Logging;
using EntityFramework.Extensions;
using WbMyFather.DAL.Model.Base;
using WbMyFather.DAL;

namespace WbMyFather.BLL.Services.Base
{
    public abstract class ServiceBase<TEntity> where TEntity : class, IBaseTable<int>, new()
    {
        protected readonly IUnitOfWork Uow;
        protected readonly IMapper Mapper;
        protected readonly ILog Logger;
        protected readonly IRepository<TEntity> Repository;

        protected ServiceBase(IUnitOfWork uow, IRepository<TEntity> repository, IMapper mapper, ILog logger)
        {
            Mapper = mapper;
            Uow = uow;
            Logger = logger;
            Repository = repository;
        }

        /// <summary>
        /// Получить сущность по идентификатору
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="id"></param>
        /// <param name="project">Использовать проекцию данных в DTO</param>
        /// <returns></returns>
        protected async Task<TDto> GetById<TDto>(int id, bool project = true)
        {
            try
            {
                var query = Repository.Where(w => w.Id == id);
                if (typeof(IDeletedEntity).IsAssignableFrom(typeof(TEntity)))
                    query = query.Where("Deleted == null");

                var dto = project ? await query.ProjectTo<TDto>(Mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync() : Mapper.Map<TDto>(await query.SingleOrDefaultAsync());

                if (dto == null)
                {
                    throw new SecurityException("Нет прав для просмотра объекта");
                }

                return dto;
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка получения сущности {typeof(TEntity).Name} с идентификатором {id}: " + ex);
                throw;
            }

        }

        /// <summary>
        /// Получить сущности
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        protected async Task<IEnumerable<TDto>> Get<TDto>(Expression<Func<TEntity, bool>> searchQuery, bool project = true, string sortBy = null, int sortOrder = 0)
        {
            try
            {
                var query = searchQuery == null ? Repository.All() : Repository.Where(searchQuery);

                if (typeof(IDeletedEntity).IsAssignableFrom(typeof(TEntity)))
                    query = query.Where("Deleted == null");

                var orderExpression = (sortBy ?? "Id") + ' ' + (sortOrder == 0 ? "ASC" : "DESC");
                query = query.OrderBy(orderExpression);

                return project ? await query.ProjectTo<TDto>(Mapper.ConfigurationProvider).ToListAsync() : Mapper.Map<IEnumerable<TDto>>(await query.ToListAsync());
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка получения списка сущностей {typeof(TEntity).Name}: " + ex);
                throw;
            }
        }

        /// <summary>
        /// Получить все работы постранично
        /// </summary>
        /// <param name="param"></param>
        /// <param name="searchQuery"></param>
        /// <param name="project">Использовать проекцию данных в DTO</param>
        /// <returns></returns>
        /*protected async Task<PagedListDto<TDto>> GetPaged<TDto>(GetSortedFilteredPaging param, Expression<Func<TEntity, bool>> searchQuery, bool project = true)
        {
            try
            {
                var orderExpression = (param.SortField ?? "Id") + ' ' + (param.SortOrder == 0 ? "ASC" : "DESC");
                var query = searchQuery == null ? Repository.All() : Repository.Where(searchQuery);

                if (typeof(IDeletedEntity).IsAssignableFrom(typeof(TEntity)))
                    query = query.Where("Deleted == null");

                //TODO подумать как параметры передать в обобщенный запрос
                if (param.Filters != null && param.Filters.Any())
                {
                    foreach (var filter in param.Filters)
                    {
                        query = query.Where(filter.Expression, filter.Values);
                    }
                }

                query = query.OrderBy(orderExpression);
                var count = await query.CountAsync();
                var page = param.PageNumber > 1
                    ? query.Skip((param.PageNumber - 1) * param.PageSize).Take(param.PageSize)
                    : query.Take(param.PageSize);

                return new PagedListDto<TDto>
                {
                    Data = project ? await page.ProjectTo<TDto>(Mapper.ConfigurationProvider).ToListAsync() : Mapper.Map<IEnumerable<TDto>>(await page.ToListAsync()),
                    PageSize = param.PageSize,
                    PageIndex = param.PageNumber < 1 ? 0 : param.PageNumber - 1,
                    TotalCount = count,
                    PageCount = GetPageCount(param.PageSize, count)
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка получения страницы списка сущностей {typeof(TEntity).Name}: " + ex);
                //throw;
                return null;
            }

        }*/

        /// <summary>
        /// Удаление одной сущности
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task Delete(int id)
        {
            await Delete(new[] { id });
        }

        /// <summary>
        /// Удаление нескольких сущностй
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected async Task Delete(IEnumerable<int> ids)
        {
            try
            {
                var query = Repository
                .Where(w => ids.Contains(w.Id));

                if (typeof(IDeletedEntity).IsAssignableFrom(typeof(TEntity)))
                {
                    var now = DateTime.UtcNow;
                    var entities = await query.ToListAsync();
                    foreach (var entity in entities.OfType<IDeletedEntity>())
                        entity.Deleted = now;
                    await Uow.SaveChangesAsync();
                    return;
                }

                await query
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("Ошибка удаления сущности " + ex);
                throw;
            }

        }

        /// <summary>
        /// Создание новой сущности
        /// </summary>
        /// <param name="newEntity"></param>
        /// <returns></returns>
        protected async Task<int> Create(TEntity newEntity)
        {
            var addedEntity = Repository.Add(newEntity);
            await Uow.SaveChangesAsync();
            return addedEntity.Id;
        }

        protected int GetPageCount(int pageSize, int totalCount)
        {
            if (pageSize == 0)
                return 0;

            var remainder = totalCount % pageSize;
            return (totalCount / pageSize) + (remainder == 0 ? 0 : 1);
        }
    }
}

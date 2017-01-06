using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WbMyFather.DAL.Model;

namespace WbMyFather.DAL
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Получить количество сущностей
        /// </summary>
        int Count(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Наличие сущностей с данным фильтром
        /// </summary>
        bool Contains(Expression<Func<T, bool>> filter);

        /// <summary>
        /// Определяет наличие сущности в контексте
        /// </summary>
        bool Contains(T entity);

        /// <summary>
        /// Добависть сущность в контекст
        /// </summary>
        T Add(T entity);

        void AddRange(IEnumerable<T> entitys);

        /// <summary> Обновить </summary>
        void Update(T entity);

        /// <summary> Создать сущнсть с proxy </summary>
        T Create();

        /// <summary>
        /// Присоеденить сущность которая есть в базе но нет в контексте
        /// </summary>
        void Attach(T entity);

        /// <summary> Добавляет или обновляет сущность </summary>
        void InsertOrUpdate(T entity);

        /// <summary> Удалить сущность </summary>
        void Remove(T entity, bool force = false);

        /// <summary> Удалить сущность </summary>
        void Remove(Expression<Func<T, bool>> filter, bool force = false);

        /// <summary> Получить сущность </summary>
        T Get(Expression<Func<T, bool>> filter, string includeProperties = null);

        /// <summary> Загрузка связанных данных коллекции </summary>
        void LoadCollection(T entity, string navigationProperty);

        /// <summary> Загрузка связанных данных ссылки </summary>
        void LoadReference(T entity, string navigationProperty);

        /// <summary> Получить все сущности </summary>
        IEnumerable<T> GetAll(string includeProperties = null, bool noTracking = false, string sortBy = null, int sortOrder = 0);

        IEnumerable<T> Find(Expression<Func<T, bool>> filter, bool noTracking = false);
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, string sortBy, int sortOrder, string includeProperties, bool noTracking = false);
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, string sortBy, int sortOrder, int numDesired, int numSkipped, string includeProperties, bool noTracking = false);

        PagedList<T> FindPaging(int pageNumber, int pageSize, Expression<Func<T, bool>> filter, string sortBy, int sortOrder,
                        string includeProperties = null, bool noTracking = false, string filterStringExpression = null);

        /// <summary> Raw SQL Queries </summary>
        IEnumerable<T> FindByRawSql(string query, params object[] parameters);

        /// <summary>
        /// Для работы без UOW
        /// </summary>
        void SaveChanges();

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> filter);
        IQueryable<T> Where(Expression<Func<T, bool>> filter);
        IQueryable<T> All();
    }
}

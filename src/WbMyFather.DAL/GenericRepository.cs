using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WbMyFather.DAL.Extensions;
using WbMyFather.DAL.Model;
using WbMyFather.DAL.Model.Base;

namespace WbMyFather.DAL
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Entities;
        protected readonly IDbSet<T> Dbset;

        public GenericRepository(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context), "DbContext cannot be null.");

            Entities = context;
            Dbset = context.Set<T>();
        }

        public int Count(Expression<Func<T, bool>> filter = null)
        {
            return (filter == null) ? Dbset.Count() : Dbset.Count(filter);
        }

        public T Add(T entity)
        {
            return Dbset.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            var dt = Dbset as DbSet<T>;
            if (dt != null)
            {
                dt.AddRange(entities);
            }
            else
            {
                foreach (var e in entities)
                {
                    Dbset.Add(e);
                }
            }
        }

        public void Update(T entity)
        {
            Entities.Entry(entity).State = EntityState.Modified;

            var obj = entity as IDictionaryConcurrency;
            if (obj != null)
            {
                Entities.Entry(entity).Property("RowVersion").OriginalValue = obj.RowVersion;
                Entities.Entry(entity).Property("RowVersion").IsModified = true;
            }
        }

        public void Attach(T entity)
        {
            Dbset.Attach(entity);
            Entities.Entry(entity).State = EntityState.Modified;

        }

        public virtual void InsertOrUpdate(T entity)
        {
            throw new NotImplementedException();
        }

        public T Create()
        {
            return Dbset.Create();
        }

        public bool Contains(T entity)
        {
            return Dbset.FirstOrDefault(t => t == entity) != null;
        }

        public bool Contains(Expression<Func<T, bool>> filter)
        {
            return Dbset.FirstOrDefault(filter) != null;
        }

        public void Remove(T entity, bool force = false)
        {
            if (Entities.Entry(entity).State == EntityState.Detached)
            {
                Dbset.Attach(entity);
            }

            IDeletedEntity e = entity as IDeletedEntity;
            if (e != null && !force)
            {
                e.Deleted = DateTime.UtcNow;
            }
            else
            {
                Dbset.Remove(entity);
            }
        }

        public void Remove(Expression<Func<T, bool>> filter, bool force = false)
        {
            IEnumerable<T> objects = Dbset.Where(filter).AsEnumerable();
            foreach (T obj in objects) Remove(obj, force);
        }

        public T Get(Expression<Func<T, bool>> filter, string includeProperties = null)
        {
            IQueryable<T> query = Dbset;
            if (!string.IsNullOrEmpty(includeProperties))
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.ToList<T>().FirstOrDefault();
            //return Dbset.FirstOrDefault(filter);
        }


        public void LoadCollection(T entity, string navigationProperty)
        {
            Entities.Entry(entity).Collection(navigationProperty).Load();
        }

        public void LoadReference(T entity, string navigationProperty)
        {
            Entities.Entry(entity).Reference(navigationProperty).Load();
        }


        private IOrderedQueryable<T> OrderingHelper(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty);
            Expression property = param;
            //Expression.PropertyOrField(param, propertyName);

            foreach (var member in propertyName.Split('.'))
            {
                property = Expression.PropertyOrField(property, member);
            }

            LambdaExpression sort = Expression.Lambda(property, param);
            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));
            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }

        /// <summary>
        /// Сортировка набора по умолчанию по ключевым полям
        /// </summary>
        private IQueryable<T> OrderByKeys(IQueryable<T> source)
        {
            var keys = Entities.GetKeyNames<T>();
            return OrderByFields(source, keys, SortOrder.Ascending);
        }

        private IQueryable<T> OrderByFields(IQueryable<T> source, string[] fields, SortOrder order)
        {
            var first = true;
            var ret = source;
            foreach (var key in fields)
            {
                ret = OrderingHelper(ret, key, order == SortOrder.Descending, !first);
                first = false;
            }
            return ret;
        }

        public IEnumerable<T> GetAll(string includeProperties = null, bool noTracking = false, string sortBy = null, int sortOrder = 0)
        {
            IQueryable<T> query = Dbset;
            if (!string.IsNullOrEmpty(includeProperties))
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (!string.IsNullOrEmpty(sortBy))
            {
                string[] sortFields = sortBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                query = sortFields.Any()
                    ? OrderByFields(query, sortFields, sortOrder == 0 ? SortOrder.Ascending : SortOrder.Descending)
                    : OrderByKeys(query);
            }
            else query = OrderByKeys(query);

            return noTracking ? query.AsNoTracking().ToList<T>() : query.ToList<T>();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter, bool noTracking = false)
        {
            IQueryable<T> query = Dbset.Where(filter);

            return noTracking ? OrderByKeys(query).AsNoTracking().ToList<T>() : OrderByKeys(query).ToList<T>();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter,
                string sortBy, int sortOrder, string includeProperties = "", bool noTracking = false)
        {
            IQueryable<T> query = Dbset;
            if (!string.IsNullOrEmpty(includeProperties))
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                string[] sortFields = sortBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                query = sortFields.Any()
                    ? OrderByFields(query, sortFields, sortOrder == 0 ? SortOrder.Ascending : SortOrder.Descending)
                    : OrderByKeys(query);
            }
            else query = OrderByKeys(query);
            return noTracking ? query.AsNoTracking().ToList() : query.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter,
            string sortBy, int sortOrder, int numDesired, int numSkipped, string includeProperties, bool noTracking = false)
        {
            IQueryable<T> query = Dbset;
            if (!string.IsNullOrEmpty(includeProperties))
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                string[] sortFields = sortBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                query = sortFields.Any()
                    ? OrderByFields(query, sortFields, sortOrder == 0 ? SortOrder.Ascending : SortOrder.Descending)
                    : OrderByKeys(query);
            }
            else query = OrderByKeys(query);
            return noTracking ? query.Skip(numSkipped).Take(numDesired).AsNoTracking().ToList() : query.Skip(numSkipped).Take(numDesired).ToList();
        }

        public PagedList<T> FindPaging(int pageNumber, int pageSize, Expression<Func<T, bool>> filter, string sortBy, int sortOrder,
                        string includeProperties = null, bool noTracking = false, string filterStringExpression = null)
        {
            IQueryable<T> query = Dbset;
            if (!string.IsNullOrEmpty(includeProperties))
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(filterStringExpression))
            {
                query = query.Where(filterStringExpression);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                string[] sortFields = sortBy.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                query = sortFields.Any()
                    ? OrderByFields(query, sortFields, sortOrder == 0 ? SortOrder.Ascending : SortOrder.Descending)
                    : OrderByKeys(query);
            }
            else query = OrderByKeys(query);

            if (noTracking) query = query.AsNoTracking();

            var pagingModel = new PagedList<T>(query, pageNumber, pageSize);
            return pagingModel;
        }

        public IEnumerable<T> FindByRawSql(string query, params object[] parameters)
        {
            var db = Dbset as DbSet<T>;
            return db?.SqlQuery(query, parameters).ToList();
        }

        public void SaveChanges()
        {
            Entities.SaveChanges();
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return await Dbset.SingleOrDefaultAsync(filter);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> filter)
        {
            return Dbset.Where(filter);
        }

        public IQueryable<T> All()
        {
            return Dbset.AsQueryable();
        }
    }
}

using System.Data.Entity;
using System.Data.Entity.Migrations;
using WbMyFather.DAL.Model.Base;

namespace WbMyFather.DAL
{
    public class BaseTableRepository<T, TKey> : GenericRepository<T> where T : class, IBaseTable<TKey>
    {
        public BaseTableRepository(DbContext context)
           : base(context)
        {
        }

        public override void InsertOrUpdate(T entity)
        {
            Dbset.AddOrUpdate(e => e.Id, entity);
        }
    }
}

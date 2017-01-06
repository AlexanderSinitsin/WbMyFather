using System.Data.Entity;
using WbMyFather.DAL.Context;
using WbMyFather.DAL.Entities;
using WbMyFather.DAL.Model.Base;

namespace WbMyFather.DAL
{
    public class WbMyFatherUnitOfWork : UnitOfWorkBase, IWbMyFatherUnitOfWork
    {
        public WbMyFatherUnitOfWork() : base(GetDbContext())
        {
        }

        private static DbContext GetDbContext()
        {
            return new DataContext();
        }

        #region Repository

        private IRepository<Word> _wordRepository;

        public IRepository<Word> WordsRepository
            => _wordRepository ?? (_wordRepository = new BaseTableRepository<Word, int>(Context));

        public IRepository<T> GetRepository<T, TK>() where T : class, IBaseTable<TK>
        {
            var typeMdl = typeof(T);

            if (typeMdl == typeof(Word)) return (IRepository<T>)WordsRepository;

            return null;
        }

        #endregion Repository
    }
}

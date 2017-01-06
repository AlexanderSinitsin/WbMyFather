using WbMyFather.DAL.Entities;
using WbMyFather.DAL.Model.Base;

namespace WbMyFather.DAL
{
    public interface IWbMyFatherUnitOfWork : IUnitOfWork
    {
        IRepository<Word> WordsRepository { get; }

        IRepository<T> GetRepository<T, TK>() where T : class, IBaseTable<TK>;
    }
}

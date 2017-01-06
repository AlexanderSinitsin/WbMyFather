using System;
using System.Data;
using System.Threading.Tasks;

namespace WbMyFather.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Откат изменений
        /// </summary>
        void CancelChanges();


        /// <summary>
        /// Начать транзакцию
        /// </summary>
        IDisposable BeginTransaction();

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        IDisposable BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Подтверждение транзакции
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Отмена транзакциии
        /// </summary>
        void RollbackTransaction();
    }
}

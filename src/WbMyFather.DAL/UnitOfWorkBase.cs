using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace WbMyFather.DAL
{
    public class UnitOfWorkBase : IUnitOfWork
    {
        protected DbContext Context { get; }

        public DbContextTransaction Transaction { get; private set; }

        public UnitOfWorkBase(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            Transaction = null;
            Context = context;

            SetupContext();
        }

        private void SetupContext()
        {
            Context.Configuration.LazyLoadingEnabled = true;
            Context.Database.CommandTimeout = 1200;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void CancelChanges()
        {
            Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }

        public int SaveChanges()
        {
            Context.ChangeTracker.DetectChanges();
            return Context.SaveChanges();
        }

        public virtual IDisposable BeginTransaction(IsolationLevel isolationLevel)
        {
            Transaction = Context.Database.BeginTransaction(isolationLevel);

            return Transaction;
        }
        public virtual IDisposable BeginTransaction()
        {
            Transaction = Context.Database.BeginTransaction();

            return Transaction;
        }

        public virtual void CommitTransaction()
        {
            Transaction?.Commit();
        }

        public virtual void RollbackTransaction()
        {
            Transaction?.Rollback();
        }

        //public void AutoDetectChangesEnabled()
        //{
        //    Context.Configuration.AutoDetectChangesEnabled = true;
        //    Context.Configuration.ValidateOnSaveEnabled = true;
        //}

        //public void AutoDetectChangesDisabled()
        //{
        //    Context.Configuration.AutoDetectChangesEnabled = false;
        //    Context.Configuration.ValidateOnSaveEnabled = false;
        //}

        #region IDisposable

        protected bool Disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Transaction?.Dispose();
                    Context?.Dispose();
                }
            }
            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

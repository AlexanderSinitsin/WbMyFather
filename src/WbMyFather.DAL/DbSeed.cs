using NLog;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using System.Transactions;
using WbMyFather.DAL.Context;
using WbMyFather.DAL.Entities;

namespace WbMyFather.DAL
{
    internal static class DbSeed
    {
        // init version 20170113
        private const int CurrentVersion = 20170113;

        private static SpinLock _lock = new SpinLock();

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void SeedForProd(DataContext context)
        {
            var lockTaken = false;

            try
            {
                _lock.Enter(ref lockTaken);

                if (!SeedRequired(context))
                {
                    return;
                }

                using (var ts = new TransactionScope())
                {
                    SeedRows(context);

                    // Фиксируем версию данных
                    context.DbDataVersions.AddOrUpdate(v => v.Id, new DbDataVersion { Id = 1, Version = CurrentVersion });
                    context.SaveChanges();

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
            finally
            {
                if (lockTaken)
                {
                    _lock.Exit();
                }
            }
        }

        private static void SeedRows(DataContext context)
        {
            context.Rows.AddOrUpdate(r => r.Id,
                new Row { Id = 1, Name = "л." },
                new Row { Id = 2, Name = "п." },
                new Row { Id = 3, Name = "прим." });
            context.SaveChanges();
        }


        public static bool SeedRequired(DataContext context)
        {
            var version = context.DbDataVersions.FirstOrDefault();

            return version == null || version.Version < CurrentVersion;
        }
    }
}

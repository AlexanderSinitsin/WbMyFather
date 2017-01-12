using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WbMyFather.DAL.Context;
using WbMyFather.DAL.Migrations;

namespace WbMyFather.DAL
{
    public static class DataBaseInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
            using (var context = new DataContext())
            {
                context.Database.Initialize(false);
            }
        }
    }
}

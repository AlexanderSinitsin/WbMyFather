using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;


namespace WbMyFather.DAL.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Получить имена ключевых полей
        /// </summary>
        public static string[] GetKeyNames<TEntity>(this DbContext context)
                    where TEntity : class
        {
            return context.GetKeyNames(typeof(TEntity));
        }

        private static string[] GetKeyNames(this DbContext context, Type entityType)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the mapping between CLR types and metadata OSpace
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get metadata for given CLR type
            var entityMetadata = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == entityType);

            return entityMetadata.KeyProperties.Select(p => p.Name).ToArray(); ;
        }
    }
}

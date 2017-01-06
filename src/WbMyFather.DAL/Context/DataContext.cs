using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WbMyFather.DAL.Entities;

namespace WbMyFather.DAL.Context
{
    public class DataContext : DbContext
    {
        public DataContext() : base("name=DataContext")
        {
        }

        public DataContext(string connectionString) : base(connectionString)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public virtual DbSet<Word> Words { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<WordBook>()
                .HasMany(lo => lo.Pages)
                .WithRequired(lor => lor.WordBook)
                .WillCascadeOnDelete();
        }
    }
}

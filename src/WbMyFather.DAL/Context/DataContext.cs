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
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<DbDataVersion> DbDataVersions { get; set; }
        public virtual DbSet<Line> Lines { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Row> Rows { get; set; }
        public virtual DbSet<WordBook> WordBooks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<WordBook>()
                .HasMany(lo => lo.Pages)
                .WithRequired(lor => lor.WordBook)
                .WillCascadeOnDelete();
            modelBuilder.Entity<Page>()
               .HasMany(lo => lo.Lines)
               .WithRequired(lor => lor.Page)
               .WillCascadeOnDelete();
        }
    }
}

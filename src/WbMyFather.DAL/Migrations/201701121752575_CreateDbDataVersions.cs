namespace WbMyFather.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDbDataVersions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DbDataVersions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Version = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DbDataVersions");
        }
    }
}

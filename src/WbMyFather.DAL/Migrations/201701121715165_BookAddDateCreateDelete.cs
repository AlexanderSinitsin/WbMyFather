namespace WbMyFather.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookAddDateCreateDelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "DateCreate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Books", "Deleted", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "Deleted");
            DropColumn("dbo.Books", "DateCreate");
        }
    }
}

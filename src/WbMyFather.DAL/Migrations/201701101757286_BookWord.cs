namespace WbMyFather.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookWord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Words", "Book_Id", c => c.Int());
            CreateIndex("dbo.Words", "Book_Id");
            AddForeignKey("dbo.Words", "Book_Id", "dbo.Books", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Words", "Book_Id", "dbo.Books");
            DropIndex("dbo.Words", new[] { "Book_Id" });
            DropColumn("dbo.Words", "Book_Id");
        }
    }
}

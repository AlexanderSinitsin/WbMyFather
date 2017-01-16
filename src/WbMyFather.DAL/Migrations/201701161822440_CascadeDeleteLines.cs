namespace WbMyFather.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDeleteLines : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lines", "PageId", "dbo.Pages");
            DropIndex("dbo.Lines", new[] { "PageId" });
            AlterColumn("dbo.Lines", "PageId", c => c.Int(nullable: false));
            CreateIndex("dbo.Lines", "PageId");
            AddForeignKey("dbo.Lines", "PageId", "dbo.Pages", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lines", "PageId", "dbo.Pages");
            DropIndex("dbo.Lines", new[] { "PageId" });
            AlterColumn("dbo.Lines", "PageId", c => c.Int());
            CreateIndex("dbo.Lines", "PageId");
            AddForeignKey("dbo.Lines", "PageId", "dbo.Pages", "Id");
        }
    }
}

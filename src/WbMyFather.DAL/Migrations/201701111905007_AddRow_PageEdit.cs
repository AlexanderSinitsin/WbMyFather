namespace WbMyFather.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRow_PageEdit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Pages", "RowId", c => c.Int());
            AddColumn("dbo.Pages", "DateRecord", c => c.DateTime());
            AlterColumn("dbo.Pages", "Number", c => c.Int());
            CreateIndex("dbo.Pages", "RowId");
            AddForeignKey("dbo.Pages", "RowId", "dbo.Rows", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pages", "RowId", "dbo.Rows");
            DropIndex("dbo.Pages", new[] { "RowId" });
            AlterColumn("dbo.Pages", "Number", c => c.Int(nullable: false));
            DropColumn("dbo.Pages", "DateRecord");
            DropColumn("dbo.Pages", "RowId");
            DropTable("dbo.Rows");
        }
    }
}

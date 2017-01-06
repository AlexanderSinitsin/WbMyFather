namespace WbMyFather.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateWordBook : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CityOfPublication = c.String(),
                        DateOfPublication = c.DateTime(),
                        Publication = c.String(),
                        Reference = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WordBooks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WordId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId)
                .ForeignKey("dbo.Words", t => t.WordId)
                .Index(t => t.WordId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        WordBookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WordBooks", t => t.WordBookId, cascadeDelete: true)
                .Index(t => t.WordBookId);
            
            CreateTable(
                "dbo.Lines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Up = c.Boolean(nullable: false),
                        Number = c.Int(nullable: false),
                        PageId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId)
                .Index(t => t.PageId);
            
            DropColumn("dbo.Words", "Abbreviation");
            DropColumn("dbo.Words", "Page");
            DropColumn("dbo.Words", "Up");
            DropColumn("dbo.Words", "Line");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Words", "Line", c => c.Int());
            AddColumn("dbo.Words", "Up", c => c.Boolean(nullable: false));
            AddColumn("dbo.Words", "Page", c => c.Int(nullable: false));
            AddColumn("dbo.Words", "Abbreviation", c => c.String());
            DropForeignKey("dbo.WordBooks", "WordId", "dbo.Words");
            DropForeignKey("dbo.Pages", "WordBookId", "dbo.WordBooks");
            DropForeignKey("dbo.Lines", "PageId", "dbo.Pages");
            DropForeignKey("dbo.WordBooks", "BookId", "dbo.Books");
            DropForeignKey("dbo.BookWords", "Word_Id", "dbo.Words");
            DropForeignKey("dbo.BookWords", "Book_Id", "dbo.Books");
            DropIndex("dbo.BookWords", new[] { "Word_Id" });
            DropIndex("dbo.BookWords", new[] { "Book_Id" });
            DropIndex("dbo.Lines", new[] { "PageId" });
            DropIndex("dbo.Pages", new[] { "WordBookId" });
            DropIndex("dbo.WordBooks", new[] { "BookId" });
            DropIndex("dbo.WordBooks", new[] { "WordId" });
            DropTable("dbo.BookWords");
            DropTable("dbo.Lines");
            DropTable("dbo.Pages");
            DropTable("dbo.WordBooks");
            DropTable("dbo.Books");
        }
    }
}

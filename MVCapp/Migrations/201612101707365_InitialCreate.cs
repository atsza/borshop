namespace MVCapp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        WeinRegion = c.String(),
                        IsDiscount = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID)
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ShoppingCart",
                c => new
                    {
                        ShoppingCartID = c.Int(nullable: false, identity: true),
                        Count = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShoppingCartID)
                .ForeignKey("dbo.Product", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Dummy",
                c => new
                    {
                        DummyID = c.Int(nullable: false, identity: true),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DummyID);
            
            CreateTable(
                "dbo.RegionCoordinates",
                c => new
                    {
                        RegionCoordinatesID = c.Int(nullable: false, identity: true),
                        RegionName = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.RegionCoordinatesID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingCart", "ProductID", "dbo.Product");
            DropForeignKey("dbo.Product", "CategoryId", "dbo.Category");
            DropIndex("dbo.ShoppingCart", new[] { "ProductID" });
            DropIndex("dbo.Product", new[] { "CategoryId" });
            DropTable("dbo.RegionCoordinates");
            DropTable("dbo.Dummy");
            DropTable("dbo.ShoppingCart");
            DropTable("dbo.Product");
            DropTable("dbo.Category");
        }
    }
}

namespace MVCapp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShoppingCart", "UserID", c => c.String());
            AddColumn("dbo.ShoppingCart", "DateCreated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShoppingCart", "DateCreated");
            DropColumn("dbo.ShoppingCart", "UserID");
        }
    }
}

namespace ShopApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Something : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Item", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Item", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Item", "Description", c => c.String());
            AlterColumn("dbo.Item", "Name", c => c.String());
        }
    }
}

namespace CategoryApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemImageName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Item", "ImageName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Item", "ImageName");
        }
    }
}

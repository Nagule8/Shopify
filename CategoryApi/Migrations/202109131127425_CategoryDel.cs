namespace CategoryApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryDel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Item", "CategoryId", "dbo.Category");
            DropIndex("dbo.Item", new[] { "CategoryId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Item", "CategoryId");
            AddForeignKey("dbo.Item", "CategoryId", "dbo.Category", "Id", cascadeDelete: true);
        }
    }
}

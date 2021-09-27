namespace CategoryApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryDel1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Item", "CategoryId");
            AddForeignKey("dbo.Item", "CategoryId", "dbo.Category", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Item", "CategoryId", "dbo.Category");
            DropIndex("dbo.Item", new[] { "CategoryId" });
        }
    }
}

namespace ShopApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemCId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Item", "Category_Id", "dbo.Category");
            DropIndex("dbo.Item", new[] { "Category_Id" });
            RenameColumn(table: "dbo.Item", name: "Category_Id", newName: "CategoryId");
            AlterColumn("dbo.Item", "CategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Item", "CategoryId");
            AddForeignKey("dbo.Item", "CategoryId", "dbo.Category", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Item", "CategoryId", "dbo.Category");
            DropIndex("dbo.Item", new[] { "CategoryId" });
            AlterColumn("dbo.Item", "CategoryId", c => c.Int());
            RenameColumn(table: "dbo.Item", name: "CategoryId", newName: "Category_Id");
            CreateIndex("dbo.Item", "Category_Id");
            AddForeignKey("dbo.Item", "Category_Id", "dbo.Category", "Id");
        }
    }
}

namespace ShopApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserBasedCart : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartItem", "RegisterUser_Id", "dbo.RegisterUser");
            DropIndex("dbo.CartItem", new[] { "RegisterUser_Id" });
            RenameColumn(table: "dbo.CartItem", name: "RegisterUser_Id", newName: "RegisterUserId");
            AlterColumn("dbo.CartItem", "RegisterUserId", c => c.Int(nullable: true));
            CreateIndex("dbo.CartItem", "RegisterUserId");
            AddForeignKey("dbo.CartItem", "RegisterUserId", "dbo.RegisterUser", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.CartItem", "User_Id", c => c.Int(nullable: true));
            DropForeignKey("dbo.CartItem", "RegisterUserId", "dbo.RegisterUser");
            DropIndex("dbo.CartItem", new[] { "RegisterUserId" });
            AlterColumn("dbo.CartItem", "RegisterUserId", c => c.Int());
            RenameColumn(table: "dbo.CartItem", name: "RegisterUserId", newName: "RegisterUser_Id");
            CreateIndex("dbo.CartItem", "RegisterUser_Id");
            AddForeignKey("dbo.CartItem", "RegisterUser_Id", "dbo.RegisterUser", "Id");
        }
    }
}

namespace ShopApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRegistration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegisterUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Role = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        role = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.User");
            DropTable("dbo.RegisterUser");
        }
    }
}

namespace ShopApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailRegister : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RegisterUser", "Email", c => c.String());
            DropTable("dbo.User");
        }
        
        public override void Down()
        {
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
            
            DropColumn("dbo.RegisterUser", "Email");
        }
    }
}

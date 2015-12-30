namespace JobTimer.Data.Access.Identity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRefreshTokens : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Clients", "ApplicationType");
            DropColumn("dbo.Clients", "RefreshTokenLifeTime");
            DropTable("dbo.RefreshTokens");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Clients", "RefreshTokenLifeTime", c => c.Int(nullable: false));
            AddColumn("dbo.Clients", "ApplicationType", c => c.Int(nullable: false));
        }
    }
}

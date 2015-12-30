namespace JobTimer.Data.Access.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LastVisited",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Visited = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Overtime",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        Time = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Shortcuts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Shortcuts = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Timer",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        UserName = c.String(),
                        Date = c.DateTime(nullable: false),
                        Time = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TimerType", t => t.Type, cascadeDelete: true)
                .Index(t => t.Type);
            
            CreateTable(
                "dbo.TimerType",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Timer", "Type", "dbo.TimerType");
            DropIndex("dbo.Timer", new[] { "Type" });
            DropTable("dbo.TimerType");
            DropTable("dbo.Timer");
            DropTable("dbo.Shortcuts");
            DropTable("dbo.Overtime");
            DropTable("dbo.LastVisited");
        }
    }
}

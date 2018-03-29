namespace DotAPicker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Heroes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AltNames = c.String(),
                        Notes = c.String(),
                        Preference = c.Int(nullable: false),
                        Labels = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        Email = c.String(nullable: false),
                        Password = c.String(),
                        PasswordResetToken = c.String(),
                        CurrentPatch = c.String(),
                        ProfileType = c.Int(nullable: false),
                        ShowDeprecatedTips = c.Boolean(nullable: false),
                        ShowDeprecatedRelationships = c.Boolean(nullable: false),
                        LabelOptions = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Relationship",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HeroObjectId = c.Int(),
                        LabelObject = c.String(),
                        HeroSubjectId = c.Int(),
                        LabelSubject = c.String(),
                        Type = c.Int(nullable: false),
                        Text = c.String(nullable: false),
                        Patch = c.String(nullable: false),
                        Deprecated = c.Boolean(nullable: false),
                        Source = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Heroes", t => t.HeroObjectId)
                .ForeignKey("dbo.Heroes", t => t.HeroSubjectId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.HeroObjectId)
                .Index(t => t.HeroSubjectId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Tip",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HeroSubjectId = c.Int(),
                        LabelSubject = c.String(),
                        Type = c.Int(nullable: false),
                        Text = c.String(nullable: false),
                        Patch = c.String(nullable: false),
                        Deprecated = c.Boolean(nullable: false),
                        Source = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Heroes", t => t.HeroSubjectId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.HeroSubjectId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tip", "UserId", "dbo.Users");
            DropForeignKey("dbo.Tip", "HeroSubjectId", "dbo.Heroes");
            DropForeignKey("dbo.Relationship", "UserId", "dbo.Users");
            DropForeignKey("dbo.Relationship", "HeroSubjectId", "dbo.Heroes");
            DropForeignKey("dbo.Relationship", "HeroObjectId", "dbo.Heroes");
            DropForeignKey("dbo.Heroes", "UserId", "dbo.Users");
            DropIndex("dbo.Tip", new[] { "UserId" });
            DropIndex("dbo.Tip", new[] { "HeroSubjectId" });
            DropIndex("dbo.Relationship", new[] { "UserId" });
            DropIndex("dbo.Relationship", new[] { "HeroSubjectId" });
            DropIndex("dbo.Relationship", new[] { "HeroObjectId" });
            DropIndex("dbo.Users", new[] { "Name" });
            DropIndex("dbo.Heroes", new[] { "UserId" });
            DropTable("dbo.Tip");
            DropTable("dbo.Relationship");
            DropTable("dbo.Users");
            DropTable("dbo.Heroes");
        }
    }
}

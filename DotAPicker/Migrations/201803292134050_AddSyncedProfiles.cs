namespace DotAPicker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSyncedProfiles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "SyncedProfilesSelected", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "SyncedProfilesSelected");
        }
    }
}

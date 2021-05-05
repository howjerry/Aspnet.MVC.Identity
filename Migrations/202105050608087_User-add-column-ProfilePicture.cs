namespace AspNetIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UseraddcolumnProfilePicture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUser", "ProfilePicture", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUser", "ProfilePicture");
        }
    }
}

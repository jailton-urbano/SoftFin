namespace SoftFin.POC.ConforLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Full02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BAS_CLIENTE", "BCL_PROTOCOL", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BAS_CLIENTE", "BCL_PROTOCOL");
        }
    }
}

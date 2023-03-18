namespace SoftFin.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FULL02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogImportars", "Mensagem", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LogImportars", "Mensagem");
        }
    }
}

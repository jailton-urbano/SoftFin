namespace SoftFin.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180927Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogEventoes", "DataRecebida", c => c.DateTime(nullable: false));
            AddColumn("dbo.LogEventoes", "CadeiaMetodo", c => c.String());
            AlterColumn("dbo.LogEventoes", "Exception", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LogEventoes", "Exception", c => c.String(maxLength: 4000));
            DropColumn("dbo.LogEventoes", "CadeiaMetodo");
            DropColumn("dbo.LogEventoes", "DataRecebida");
        }
    }
}

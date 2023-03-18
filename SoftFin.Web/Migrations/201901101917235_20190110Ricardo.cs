namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190110Ricardo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotaFiscals", "LocalPrestServ", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotaFiscals", "LocalPrestServ");
        }
    }
}

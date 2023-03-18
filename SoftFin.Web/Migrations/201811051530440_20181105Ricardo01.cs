namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181105Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotaFiscals", "TipoFaturamento", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotaFiscals", "TipoFaturamento");
        }
    }
}

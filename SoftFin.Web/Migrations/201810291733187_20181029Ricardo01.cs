namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181029Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContratoItems", "TipoFaturamento", c => c.Int(nullable: false));
            AddColumn("dbo.OrdemVendas", "TipoFaturamento", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrdemVendas", "TipoFaturamento");
            DropColumn("dbo.ContratoItems", "TipoFaturamento");
        }
    }
}

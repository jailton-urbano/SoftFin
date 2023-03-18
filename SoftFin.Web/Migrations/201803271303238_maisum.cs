namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class maisum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bancoes", "NumeroConvenio", c => c.String());
            AddColumn("dbo.Bancoes", "NumeroArquivoRemessa", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bancoes", "NumeroArquivoRemessa");
            DropColumn("dbo.Bancoes", "NumeroConvenio");
        }
    }
}

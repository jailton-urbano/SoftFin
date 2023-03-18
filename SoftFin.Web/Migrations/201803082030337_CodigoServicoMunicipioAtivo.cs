namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodigoServicoMunicipioAtivo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CodigoServicoMunicipios", "Ativo", c => c.Boolean());
            Sql("Update dbo.CodigoServicoMunicipios set Ativo = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.CodigoServicoMunicipios", "Ativo");
        }
    }
}

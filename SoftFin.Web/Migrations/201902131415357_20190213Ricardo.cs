namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190213Ricardo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UnidadeNegocios", "Codigo", c => c.String(maxLength: 20));
            AddColumn("dbo.ContaContabils", "DataVigenciaInicial", c => c.DateTime());
            AddColumn("dbo.ContaContabils", "Tipo", c => c.String(maxLength: 2));
            AddColumn("dbo.ContaContabils", "CategoriaGeral", c => c.String(maxLength: 2));
            AddColumn("dbo.ContaContabils", "SubCategoria", c => c.String(maxLength: 2));
            AddColumn("dbo.ContaContabils", "IndicacaoPublicacao", c => c.String(maxLength: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContaContabils", "IndicacaoPublicacao");
            DropColumn("dbo.ContaContabils", "SubCategoria");
            DropColumn("dbo.ContaContabils", "CategoriaGeral");
            DropColumn("dbo.ContaContabils", "Tipo");
            DropColumn("dbo.ContaContabils", "DataVigenciaInicial");
            DropColumn("dbo.UnidadeNegocios", "Codigo");
        }
    }
}

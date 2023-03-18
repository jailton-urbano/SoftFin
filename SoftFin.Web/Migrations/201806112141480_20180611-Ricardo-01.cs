namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180611Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SistemaPrincipals", "Codigo", c => c.String(maxLength: 12));
            AddColumn("dbo.SistemaPrincipalPerfils", "Json", c => c.String(nullable: false, maxLength: 4000));
            AlterColumn("dbo.SistemaPrincipals", "descricao", c => c.String(maxLength: 40));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SistemaPrincipals", "descricao", c => c.String());
            DropColumn("dbo.SistemaPrincipalPerfils", "Json");
            DropColumn("dbo.SistemaPrincipals", "Codigo");
        }
    }
}

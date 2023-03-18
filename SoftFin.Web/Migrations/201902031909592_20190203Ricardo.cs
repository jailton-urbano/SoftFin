namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190203Ricardo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SistemaDashBoards", "Ordem", c => c.Int());
            AddColumn("dbo.SistemaMenus", "Ordem", c => c.Int());
            AlterColumn("dbo.TaxaHoras", "descricao", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TaxaHoras", "descricao", c => c.String(nullable: false));
            DropColumn("dbo.SistemaMenus", "Ordem");
            DropColumn("dbo.SistemaDashBoards", "Ordem");
        }
    }
}

namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190111Ricardo02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LayoutTemplates", "Empresa_id", c => c.Int(nullable: false));
            CreateIndex("dbo.LayoutTemplates", "Empresa_id");
            AddForeignKey("dbo.LayoutTemplates", "Empresa_id", "dbo.Empresas", "id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LayoutTemplates", "Empresa_id", "dbo.Empresas");
            DropIndex("dbo.LayoutTemplates", new[] { "Empresa_id" });
            DropColumn("dbo.LayoutTemplates", "Empresa_id");
        }
    }
}

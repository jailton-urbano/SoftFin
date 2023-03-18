namespace SoftFin.GestorProcessos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Full05 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Funcaos", "IdEmpresa", c => c.Int(nullable: false));
            CreateIndex("dbo.Funcaos", "IdEmpresa");
            AddForeignKey("dbo.Funcaos", "IdEmpresa", "dbo.Empresas", "Id", cascadeDelete: false);
            DropColumn("dbo.Funcaos", "CodigoEmpresa");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Funcaos", "CodigoEmpresa", c => c.String(maxLength: 50));
            DropForeignKey("dbo.Funcaos", "IdEmpresa", "dbo.Empresas");
            DropIndex("dbo.Funcaos", new[] { "IdEmpresa" });
            DropColumn("dbo.Funcaos", "IdEmpresa");
        }
    }
}

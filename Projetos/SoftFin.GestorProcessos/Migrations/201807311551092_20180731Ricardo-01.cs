namespace SoftFin.GestorProcessos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180731Ricardo01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmpresaParametroes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdEmpresa = c.Int(),
                        Codigo = c.String(maxLength: 25),
                        Valor = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Empresas", t => t.IdEmpresa)
                .Index(t => t.IdEmpresa);
            
            CreateTable(
                "dbo.VisaoHashcodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Valor = c.String(),
                        UsarPadrao = c.Boolean(nullable: false),
                        IdHashcode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hashcodes", t => t.IdHashcode, cascadeDelete: false)
                .Index(t => t.IdHashcode);
            
            AlterColumn("dbo.Hashcodes", "ValorPadrao", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VisaoHashcodes", "IdHashcode", "dbo.Hashcodes");
            DropForeignKey("dbo.EmpresaParametroes", "IdEmpresa", "dbo.Empresas");
            DropIndex("dbo.VisaoHashcodes", new[] { "IdHashcode" });
            DropIndex("dbo.EmpresaParametroes", new[] { "IdEmpresa" });
            AlterColumn("dbo.Hashcodes", "ValorPadrao", c => c.String(maxLength: 4000));
            DropTable("dbo.VisaoHashcodes");
            DropTable("dbo.EmpresaParametroes");
        }
    }
}

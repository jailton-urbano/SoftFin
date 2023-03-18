namespace SoftFin.GestorProcessos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Full03 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TabelaCampoHashcodes", "IdHashcode", "dbo.Hashcodes");
            DropForeignKey("dbo.TabelaCampoHashcodes", "IdTabelaCampo", "dbo.TabelaCampoes");
            DropIndex("dbo.TabelaCampoHashcodes", new[] { "IdHashcode" });
            DropIndex("dbo.TabelaCampoHashcodes", new[] { "IdTabelaCampo" });
            CreateTable(
                "dbo.TipoVisaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(maxLength: 250),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Versaos", "IdEmpresa", c => c.Int());
            AddColumn("dbo.Visaos", "IdEmpresa", c => c.Int());
            AddColumn("dbo.Visaos", "IdTipoVisao", c => c.Int(nullable: false));
            AddColumn("dbo.Empresas", "ConectionString", c => c.String(maxLength: 500));
            CreateIndex("dbo.Versaos", "IdEmpresa");
            CreateIndex("dbo.Visaos", "IdEmpresa");
            CreateIndex("dbo.Visaos", "IdTipoVisao");
            AddForeignKey("dbo.Versaos", "IdEmpresa", "dbo.Empresas", "Id");
            AddForeignKey("dbo.Visaos", "IdEmpresa", "dbo.Empresas", "Id");
            AddForeignKey("dbo.Visaos", "IdTipoVisao", "dbo.TipoVisaos", "Id", cascadeDelete: false);
            DropColumn("dbo.Versaos", "CodigoEmpresa");
            DropColumn("dbo.Visaos", "CodigoEmpresa");
            DropColumn("dbo.Visaos", "TipoVisao");
            DropTable("dbo.TabelaCampoHashcodes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TabelaCampoHashcodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Valor = c.String(),
                        UsarPadrao = c.Boolean(nullable: false),
                        IdHashcode = c.Int(nullable: false),
                        IdTabelaCampo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Visaos", "TipoVisao", c => c.String(maxLength: 50));
            AddColumn("dbo.Visaos", "CodigoEmpresa", c => c.String(maxLength: 50));
            AddColumn("dbo.Versaos", "CodigoEmpresa", c => c.String());
            DropForeignKey("dbo.Visaos", "IdTipoVisao", "dbo.TipoVisaos");
            DropForeignKey("dbo.Visaos", "IdEmpresa", "dbo.Empresas");
            DropForeignKey("dbo.Versaos", "IdEmpresa", "dbo.Empresas");
            DropIndex("dbo.Visaos", new[] { "IdTipoVisao" });
            DropIndex("dbo.Visaos", new[] { "IdEmpresa" });
            DropIndex("dbo.Versaos", new[] { "IdEmpresa" });
            DropColumn("dbo.Empresas", "ConectionString");
            DropColumn("dbo.Visaos", "IdTipoVisao");
            DropColumn("dbo.Visaos", "IdEmpresa");
            DropColumn("dbo.Versaos", "IdEmpresa");
            DropTable("dbo.TipoVisaos");
            CreateIndex("dbo.TabelaCampoHashcodes", "IdTabelaCampo");
            CreateIndex("dbo.TabelaCampoHashcodes", "IdHashcode");
            AddForeignKey("dbo.TabelaCampoHashcodes", "IdTabelaCampo", "dbo.TabelaCampoes", "Id", cascadeDelete: false);
            AddForeignKey("dbo.TabelaCampoHashcodes", "IdHashcode", "dbo.Hashcodes", "Id", cascadeDelete: false);
        }
    }
}

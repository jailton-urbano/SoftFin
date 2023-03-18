namespace SoftFin.GestorProcessos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Full04 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Processoes", "VersaoId", "dbo.Versaos");
            DropForeignKey("dbo.AtividadePlanoes", "VersaoId", "dbo.Versaos");
            DropIndex("dbo.AtividadePlanoes", new[] { "VersaoId" });
            DropIndex("dbo.Processoes", new[] { "VersaoId" });
            AddColumn("dbo.Atividades", "IdProcesso", c => c.Int(nullable: false));
            AddColumn("dbo.Processoes", "IdEmpresa", c => c.Int(nullable: false));
            CreateIndex("dbo.Atividades", "IdProcesso");
            CreateIndex("dbo.Processoes", "IdEmpresa");
            AddForeignKey("dbo.Processoes", "IdEmpresa", "dbo.Empresas", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Atividades", "IdProcesso", "dbo.Processoes", "Id", cascadeDelete: false);
            DropColumn("dbo.Atividades", "CodigoEmpresa");
            DropColumn("dbo.Atividades", "CodigoEstabelecimento");
            DropColumn("dbo.AtividadePlanoes", "VersaoId");
            DropColumn("dbo.Processoes", "CodigoEmpresa");
            DropColumn("dbo.Processoes", "VersaoId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Processoes", "VersaoId", c => c.Int(nullable: false));
            AddColumn("dbo.Processoes", "CodigoEmpresa", c => c.String());
            AddColumn("dbo.AtividadePlanoes", "VersaoId", c => c.Int(nullable: false));
            AddColumn("dbo.Atividades", "CodigoEstabelecimento", c => c.String(maxLength: 50));
            AddColumn("dbo.Atividades", "CodigoEmpresa", c => c.String(maxLength: 50));
            DropForeignKey("dbo.Atividades", "IdProcesso", "dbo.Processoes");
            DropForeignKey("dbo.Processoes", "IdEmpresa", "dbo.Empresas");
            DropIndex("dbo.Processoes", new[] { "IdEmpresa" });
            DropIndex("dbo.Atividades", new[] { "IdProcesso" });
            DropColumn("dbo.Processoes", "IdEmpresa");
            DropColumn("dbo.Atividades", "IdProcesso");
            CreateIndex("dbo.Processoes", "VersaoId");
            CreateIndex("dbo.AtividadePlanoes", "VersaoId");
            AddForeignKey("dbo.AtividadePlanoes", "VersaoId", "dbo.Versaos", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Processoes", "VersaoId", "dbo.Versaos", "Id", cascadeDelete: true);
        }
    }
}

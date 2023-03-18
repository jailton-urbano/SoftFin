namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addContaAplicacao : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransfContas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BancoSaida_Id = c.Int(nullable: false),
                        BancoEntrada_Id = c.Int(nullable: false),
                        UnidadeSaida_Id = c.Int(nullable: false),
                        UnidadeEntrada_Id = c.Int(nullable: false),
                        BancoMovimentoSaida_Id = c.Int(nullable: false),
                        BancoMovimentoEntrada_Id = c.Int(nullable: false),
                        Data = c.DateTime(nullable: false),
                        UsuarioInclusao_Id = c.Int(nullable: false),
                        DataInclusao = c.DateTime(nullable: false),
                        UsuarioAlteracao_Id = c.Int(nullable: false),
                        DataAlteracao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bancoes", t => t.BancoEntrada_Id, cascadeDelete: false)
                .ForeignKey("dbo.BancoMovimentoes", t => t.BancoMovimentoEntrada_Id, cascadeDelete: false)
                .ForeignKey("dbo.BancoMovimentoes", t => t.BancoMovimentoSaida_Id, cascadeDelete: false)
                .ForeignKey("dbo.Bancoes", t => t.BancoSaida_Id, cascadeDelete: false)
                .ForeignKey("dbo.UnidadeNegocios", t => t.UnidadeEntrada_Id, cascadeDelete: false)
                .ForeignKey("dbo.UnidadeNegocios", t => t.UnidadeSaida_Id, cascadeDelete: false)
                .Index(t => t.BancoSaida_Id)
                .Index(t => t.BancoEntrada_Id)
                .Index(t => t.UnidadeSaida_Id)
                .Index(t => t.UnidadeEntrada_Id)
                .Index(t => t.BancoMovimentoSaida_Id)
                .Index(t => t.BancoMovimentoEntrada_Id);
            
            AddColumn("dbo.Bancoes", "DiaFechamentoFatura", c => c.Int(nullable: false));
            AddColumn("dbo.Pagamentoes", "ValorRefinanciado", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Pagamentoes", "PorcentagemJurosRefinanciamento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Pagamentoes", "ValorJurosRefinanciamento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Pagamentoes", "ValorRefinanciadoAnterior", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Pagamentoes", "PorcentagemJurosRefinanciamentoAnterior", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Pagamentoes", "ValorJurosRefinanciamentoAnterior", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Pagamentoes", "FlagCartao", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransfContas", "UnidadeSaida_Id", "dbo.UnidadeNegocios");
            DropForeignKey("dbo.TransfContas", "UnidadeEntrada_Id", "dbo.UnidadeNegocios");
            DropForeignKey("dbo.TransfContas", "BancoSaida_Id", "dbo.Bancoes");
            DropForeignKey("dbo.TransfContas", "BancoMovimentoSaida_Id", "dbo.BancoMovimentoes");
            DropForeignKey("dbo.TransfContas", "BancoMovimentoEntrada_Id", "dbo.BancoMovimentoes");
            DropForeignKey("dbo.TransfContas", "BancoEntrada_Id", "dbo.Bancoes");
            DropIndex("dbo.TransfContas", new[] { "BancoMovimentoEntrada_Id" });
            DropIndex("dbo.TransfContas", new[] { "BancoMovimentoSaida_Id" });
            DropIndex("dbo.TransfContas", new[] { "UnidadeEntrada_Id" });
            DropIndex("dbo.TransfContas", new[] { "UnidadeSaida_Id" });
            DropIndex("dbo.TransfContas", new[] { "BancoEntrada_Id" });
            DropIndex("dbo.TransfContas", new[] { "BancoSaida_Id" });
            DropColumn("dbo.Pagamentoes", "FlagCartao");
            DropColumn("dbo.Pagamentoes", "ValorJurosRefinanciamentoAnterior");
            DropColumn("dbo.Pagamentoes", "PorcentagemJurosRefinanciamentoAnterior");
            DropColumn("dbo.Pagamentoes", "ValorRefinanciadoAnterior");
            DropColumn("dbo.Pagamentoes", "ValorJurosRefinanciamento");
            DropColumn("dbo.Pagamentoes", "PorcentagemJurosRefinanciamento");
            DropColumn("dbo.Pagamentoes", "ValorRefinanciado");
            DropColumn("dbo.Bancoes", "DiaFechamentoFatura");
            DropTable("dbo.TransfContas");
        }
    }
}

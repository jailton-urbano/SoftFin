namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TipoFechamento : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LojaTipoRecebimentoCaixaVigencias",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        historico = c.String(maxLength: 150),
                        prazoDias = c.Int(nullable: false),
                        taxa = c.Decimal(nullable: false, precision: 18, scale: 2),
                        dataFimVigencia = c.DateTime(),
                        LojaTipoRecebimentoCaixa_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.LojaTipoRecebimentoCaixas", t => t.LojaTipoRecebimentoCaixa_id, cascadeDelete: true)
                .Index(t => t.LojaTipoRecebimentoCaixa_id);

            Sql("INSERT INTO dbo.LojaTipoRecebimentoCaixaVigencias SELECT 'Ajuste automático', prazoDias, taxa, null, id from dbo.LojaTipoRecebimentoCaixas ");
            AddColumn("dbo.LojaFechamentoes", "flgSituacao", c => c.String(maxLength: 1));
            Sql("UPDATE LojaFechamentoes SET flgSituacao = 'L'");
            DropColumn("dbo.LojaTipoRecebimentoCaixas", "prazoDias");
            DropColumn("dbo.LojaTipoRecebimentoCaixas", "taxa");

            Sql("update [SistemaDashBoardFuncionalidades] set Descricao = 'Fechamento caixa loja'   where Descricao like '%Loja Fechamento%'");
            Sql("update Funcionalidades set Descricao = 'Fechamento caixa loja'   where Descricao like '%Loja Fechamento%'");

        }
        
        public override void Down()
        {
            AddColumn("dbo.LojaTipoRecebimentoCaixas", "taxa", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.LojaTipoRecebimentoCaixas", "prazoDias", c => c.Int(nullable: false));
            DropForeignKey("dbo.LojaTipoRecebimentoCaixaVigencias", "LojaTipoRecebimentoCaixa_id", "dbo.LojaTipoRecebimentoCaixas");
            DropIndex("dbo.LojaTipoRecebimentoCaixaVigencias", new[] { "LojaTipoRecebimentoCaixa_id" });
            DropColumn("dbo.LojaFechamentoes", "flgSituacao");
            DropTable("dbo.LojaTipoRecebimentoCaixaVigencias");
        }
    }
}

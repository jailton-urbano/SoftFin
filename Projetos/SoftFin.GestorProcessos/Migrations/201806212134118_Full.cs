namespace SoftFin.GestorProcessos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Full : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Arquivos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataInclucao = c.DateTime(nullable: false),
                        Descricao = c.String(maxLength: 50),
                        ArquivoReal = c.String(nullable: false, maxLength: 750),
                        ArquivoOriginal = c.String(nullable: false, maxLength: 750),
                        Tamanho = c.Int(nullable: false),
                        ArquivoExtensao = c.String(maxLength: 10),
                        RotinaOwner = c.String(maxLength: 75),
                        Empresa = c.String(nullable: false),
                        Codigo = c.String(),
                        Usuario = c.String(maxLength: 200),
                        Processo = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Atividades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(maxLength: 50),
                        Codigo = c.String(maxLength: 20),
                        Ativo = c.Boolean(nullable: false),
                        CodigoEmpresa = c.String(maxLength: 50),
                        CodigoEstabelecimento = c.String(maxLength: 50),
                        Url = c.String(),
                        IdAtividadeTipo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AtividadeTipoes", t => t.IdAtividadeTipo, cascadeDelete: false)
                .Index(t => t.IdAtividadeTipo);
            
            CreateTable(
                "dbo.AtividadeTipoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(maxLength: 50),
                        Ativo = c.Boolean(nullable: false),
                        Codigo = c.String(maxLength: 15),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AtividadeFuncaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdAtividade = c.Int(nullable: false),
                        IdFuncao = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Atividades", t => t.IdAtividade, cascadeDelete: false)
                .ForeignKey("dbo.Funcaos", t => t.IdFuncao, cascadeDelete: false)
                .Index(t => t.IdAtividade)
                .Index(t => t.IdFuncao);
            
            CreateTable(
                "dbo.Funcaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(maxLength: 50),
                        Ativo = c.Boolean(nullable: false),
                        CodigoEmpresa = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AtividadePlanoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessoId = c.Int(nullable: false),
                        VersaoId = c.Int(nullable: false),
                        AtividadeId = c.Int(nullable: false),
                        AtividadeIdEntrada = c.Int(),
                        CondicaoEntrada = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Atividades", t => t.AtividadeId, cascadeDelete: false)
                .ForeignKey("dbo.Atividades", t => t.AtividadeIdEntrada)
                .ForeignKey("dbo.Processoes", t => t.ProcessoId, cascadeDelete: false)
                .ForeignKey("dbo.Versaos", t => t.VersaoId, cascadeDelete: false)
                .Index(t => t.ProcessoId)
                .Index(t => t.VersaoId)
                .Index(t => t.AtividadeId)
                .Index(t => t.AtividadeIdEntrada);
            
            CreateTable(
                "dbo.Processoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        CodigoProcessoTemplate = c.String(maxLength: 50),
                        Descricao = c.String(maxLength: 50),
                        Ativo = c.Boolean(nullable: false),
                        CodigoEmpresa = c.String(),
                        VersaoId = c.Int(nullable: false),
                        Contador = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Versaos", t => t.VersaoId, cascadeDelete: false)
                .Index(t => t.VersaoId);
            
            CreateTable(
                "dbo.Versaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(),
                        Ativo = c.Boolean(nullable: false),
                        CodigoEmpresa = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AtividadeVisaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ordem = c.Int(nullable: false),
                        IdAtividade = c.Int(nullable: false),
                        IdVisao = c.Int(nullable: false),
                        Titulo = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Atividades", t => t.IdAtividade, cascadeDelete: false)
                .ForeignKey("dbo.Visaos", t => t.IdVisao, cascadeDelete: false)
                .Index(t => t.IdAtividade)
                .Index(t => t.IdVisao);
            
            CreateTable(
                "dbo.Visaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(maxLength: 250),
                        Ativo = c.Boolean(nullable: false),
                        CodigoEmpresa = c.String(maxLength: 50),
                        IdTabela = c.Int(nullable: false),
                        TipoVisao = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tabelas", t => t.IdTabela, cascadeDelete: false)
                .Index(t => t.IdTabela);
            
            CreateTable(
                "dbo.Tabelas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(maxLength: 250),
                        Descricao = c.String(maxLength: 250),
                        Ativo = c.Boolean(nullable: false),
                        CadastroAuxiliar = c.Boolean(nullable: false),
                        SQLCadastroAuxiliar = c.String(maxLength: 300),
                        OrdemCriacao = c.Int(nullable: false),
                        IdEmpresa = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Empresas", t => t.IdEmpresa, cascadeDelete: false)
                .Index(t => t.IdEmpresa);
            
            CreateTable(
                "dbo.Empresas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(maxLength: 250),
                        Codigo = c.String(maxLength: 25),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TabelaCampoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Campo = c.String(maxLength: 250),
                        Descricao = c.String(maxLength: 250),
                        Ativo = c.Boolean(nullable: false),
                        Tabela_Id = c.Int(nullable: false),
                        IdTipoCampo = c.Int(nullable: false),
                        TamanhoColuna = c.String(maxLength: 12),
                        TamanhoCampo = c.String(maxLength: 10),
                        Precisao = c.Int(nullable: false),
                        Ordem = c.Int(nullable: false),
                        Obrigatorio = c.Boolean(nullable: false),
                        SQLDefault = c.String(maxLength: 500),
                        IdChaveEstrageira = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tabelas", t => t.IdChaveEstrageira)
                .ForeignKey("dbo.Tabelas", t => t.Tabela_Id, cascadeDelete: false)
                .ForeignKey("dbo.TipoCampoes", t => t.IdTipoCampo, cascadeDelete: false)
                .Index(t => t.Tabela_Id)
                .Index(t => t.IdTipoCampo)
                .Index(t => t.IdChaveEstrageira);
            
            CreateTable(
                "dbo.TipoCampoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descricao = c.String(maxLength: 250),
                        Ativo = c.Boolean(nullable: false),
                        TemplateHTML = c.String(),
                        TemplateHTMLModal = c.String(),
                        TemplateJSControl = c.String(),
                        TemplateJSScript = c.String(),
                        TipoBancoDados = c.String(maxLength: 30),
                        SQLDefault = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hashcodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(maxLength: 50),
                        ValorPadrao = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProcessoAnotacaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProcessoExecucaoId = c.Int(nullable: false),
                        DataInclusao = c.DateTime(nullable: false),
                        Empresa = c.String(maxLength: 250),
                        Usuario = c.String(maxLength: 250),
                        Descricao = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProcessoExecucaos", t => t.ProcessoExecucaoId, cascadeDelete: false)
                .Index(t => t.ProcessoExecucaoId);
            
            CreateTable(
                "dbo.ProcessoExecucaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.Guid(nullable: false),
                        ProcessoId = c.Int(nullable: false),
                        InicioProcesso = c.DateTime(nullable: false),
                        FimProcesso = c.DateTime(),
                        IdUsuario = c.Int(),
                        Protocolo = c.String(maxLength: 20),
                        MotivoCancelado = c.String(maxLength: 500),
                        IdUsuarioCancelamento = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processoes", t => t.ProcessoId, cascadeDelete: false)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuarioCancelamento)
                .Index(t => t.ProcessoId)
                .Index(t => t.IdUsuario)
                .Index(t => t.IdUsuarioCancelamento);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(maxLength: 250),
                        Nome = c.String(maxLength: 250),
                        Ativo = c.Boolean(nullable: false),
                        DataCadastro = c.DateTime(nullable: false),
                        Senha = c.String(maxLength: 250),
                        IdEmpresa = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Empresas", t => t.IdEmpresa)
                .Index(t => t.IdEmpresa);
            
            CreateTable(
                "dbo.UsuarioFuncaos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdUsuario = c.Int(nullable: false),
                        IdFuncao = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Funcaos", t => t.IdFuncao, cascadeDelete: false)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario, cascadeDelete: false)
                .Index(t => t.IdUsuario)
                .Index(t => t.IdFuncao);
            
            CreateTable(
                "dbo.ProcessoArquivoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataInclucao = c.DateTime(nullable: false),
                        Descricao = c.String(maxLength: 50),
                        ArquivoReal = c.String(nullable: false, maxLength: 750),
                        ArquivoOriginal = c.String(nullable: false, maxLength: 750),
                        Tamanho = c.Int(nullable: false),
                        ArquivoExtensao = c.String(maxLength: 10),
                        RotinaOwner = c.String(maxLength: 75),
                        Codigo = c.String(maxLength: 75),
                        CodigoEmpresa = c.String(),
                        ProcessoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Processoes", t => t.ProcessoId, cascadeDelete: false)
                .Index(t => t.ProcessoId);
            
            CreateTable(
                "dbo.ProcessoExecucaoAtividades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Codigo = c.String(),
                        ProcessoExecucaoId = c.Int(nullable: false),
                        InicioAtividade = c.DateTime(),
                        FimAtividade = c.DateTime(),
                        IdUsuario = c.Int(),
                        IdUsuarioExecucao = c.Int(),
                        ResultadoFinal = c.String(),
                        Situacao = c.String(maxLength: 15),
                        IdAtividade = c.Int(nullable: false),
                        Motivo = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Atividades", t => t.IdAtividade, cascadeDelete: false)
                .ForeignKey("dbo.ProcessoExecucaos", t => t.ProcessoExecucaoId, cascadeDelete: false)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuarioExecucao)
                .Index(t => t.ProcessoExecucaoId)
                .Index(t => t.IdUsuario)
                .Index(t => t.IdUsuarioExecucao)
                .Index(t => t.IdAtividade);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hashcodes", t => t.IdHashcode, cascadeDelete: false)
                .ForeignKey("dbo.TabelaCampoes", t => t.IdTabelaCampo, cascadeDelete: false)
                .Index(t => t.IdHashcode)
                .Index(t => t.IdTabelaCampo);
            
            CreateTable(
                "dbo.VisaoCampoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdTabelaCampo = c.Int(nullable: false),
                        Ativo = c.Boolean(nullable: false),
                        Visivel = c.Boolean(nullable: false),
                        Transferivel = c.Boolean(nullable: false),
                        IdVisao = c.Int(nullable: false),
                        PadraoSalva = c.String(maxLength: 150),
                        ReferenciaNgModel = c.String(maxLength: 150),
                        ReferenciaNgChange = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TabelaCampoes", t => t.IdTabelaCampo, cascadeDelete: false)
                .ForeignKey("dbo.Visaos", t => t.IdVisao, cascadeDelete: false)
                .Index(t => t.IdTabelaCampo)
                .Index(t => t.IdVisao);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VisaoCampoes", "IdVisao", "dbo.Visaos");
            DropForeignKey("dbo.VisaoCampoes", "IdTabelaCampo", "dbo.TabelaCampoes");
            DropForeignKey("dbo.TabelaCampoHashcodes", "IdTabelaCampo", "dbo.TabelaCampoes");
            DropForeignKey("dbo.TabelaCampoHashcodes", "IdHashcode", "dbo.Hashcodes");
            DropForeignKey("dbo.ProcessoExecucaoAtividades", "IdUsuarioExecucao", "dbo.Usuarios");
            DropForeignKey("dbo.ProcessoExecucaoAtividades", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.ProcessoExecucaoAtividades", "ProcessoExecucaoId", "dbo.ProcessoExecucaos");
            DropForeignKey("dbo.ProcessoExecucaoAtividades", "IdAtividade", "dbo.Atividades");
            DropForeignKey("dbo.ProcessoArquivoes", "ProcessoId", "dbo.Processoes");
            DropForeignKey("dbo.ProcessoAnotacaos", "ProcessoExecucaoId", "dbo.ProcessoExecucaos");
            DropForeignKey("dbo.ProcessoExecucaos", "IdUsuarioCancelamento", "dbo.Usuarios");
            DropForeignKey("dbo.ProcessoExecucaos", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.UsuarioFuncaos", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.UsuarioFuncaos", "IdFuncao", "dbo.Funcaos");
            DropForeignKey("dbo.Usuarios", "IdEmpresa", "dbo.Empresas");
            DropForeignKey("dbo.ProcessoExecucaos", "ProcessoId", "dbo.Processoes");
            DropForeignKey("dbo.AtividadeVisaos", "IdVisao", "dbo.Visaos");
            DropForeignKey("dbo.Visaos", "IdTabela", "dbo.Tabelas");
            DropForeignKey("dbo.TabelaCampoes", "IdTipoCampo", "dbo.TipoCampoes");
            DropForeignKey("dbo.TabelaCampoes", "Tabela_Id", "dbo.Tabelas");
            DropForeignKey("dbo.TabelaCampoes", "IdChaveEstrageira", "dbo.Tabelas");
            DropForeignKey("dbo.Tabelas", "IdEmpresa", "dbo.Empresas");
            DropForeignKey("dbo.AtividadeVisaos", "IdAtividade", "dbo.Atividades");
            DropForeignKey("dbo.AtividadePlanoes", "VersaoId", "dbo.Versaos");
            DropForeignKey("dbo.AtividadePlanoes", "ProcessoId", "dbo.Processoes");
            DropForeignKey("dbo.Processoes", "VersaoId", "dbo.Versaos");
            DropForeignKey("dbo.AtividadePlanoes", "AtividadeIdEntrada", "dbo.Atividades");
            DropForeignKey("dbo.AtividadePlanoes", "AtividadeId", "dbo.Atividades");
            DropForeignKey("dbo.AtividadeFuncaos", "IdFuncao", "dbo.Funcaos");
            DropForeignKey("dbo.AtividadeFuncaos", "IdAtividade", "dbo.Atividades");
            DropForeignKey("dbo.Atividades", "IdAtividadeTipo", "dbo.AtividadeTipoes");
            DropIndex("dbo.VisaoCampoes", new[] { "IdVisao" });
            DropIndex("dbo.VisaoCampoes", new[] { "IdTabelaCampo" });
            DropIndex("dbo.TabelaCampoHashcodes", new[] { "IdTabelaCampo" });
            DropIndex("dbo.TabelaCampoHashcodes", new[] { "IdHashcode" });
            DropIndex("dbo.ProcessoExecucaoAtividades", new[] { "IdAtividade" });
            DropIndex("dbo.ProcessoExecucaoAtividades", new[] { "IdUsuarioExecucao" });
            DropIndex("dbo.ProcessoExecucaoAtividades", new[] { "IdUsuario" });
            DropIndex("dbo.ProcessoExecucaoAtividades", new[] { "ProcessoExecucaoId" });
            DropIndex("dbo.ProcessoArquivoes", new[] { "ProcessoId" });
            DropIndex("dbo.UsuarioFuncaos", new[] { "IdFuncao" });
            DropIndex("dbo.UsuarioFuncaos", new[] { "IdUsuario" });
            DropIndex("dbo.Usuarios", new[] { "IdEmpresa" });
            DropIndex("dbo.ProcessoExecucaos", new[] { "IdUsuarioCancelamento" });
            DropIndex("dbo.ProcessoExecucaos", new[] { "IdUsuario" });
            DropIndex("dbo.ProcessoExecucaos", new[] { "ProcessoId" });
            DropIndex("dbo.ProcessoAnotacaos", new[] { "ProcessoExecucaoId" });
            DropIndex("dbo.TabelaCampoes", new[] { "IdChaveEstrageira" });
            DropIndex("dbo.TabelaCampoes", new[] { "IdTipoCampo" });
            DropIndex("dbo.TabelaCampoes", new[] { "Tabela_Id" });
            DropIndex("dbo.Tabelas", new[] { "IdEmpresa" });
            DropIndex("dbo.Visaos", new[] { "IdTabela" });
            DropIndex("dbo.AtividadeVisaos", new[] { "IdVisao" });
            DropIndex("dbo.AtividadeVisaos", new[] { "IdAtividade" });
            DropIndex("dbo.Processoes", new[] { "VersaoId" });
            DropIndex("dbo.AtividadePlanoes", new[] { "AtividadeIdEntrada" });
            DropIndex("dbo.AtividadePlanoes", new[] { "AtividadeId" });
            DropIndex("dbo.AtividadePlanoes", new[] { "VersaoId" });
            DropIndex("dbo.AtividadePlanoes", new[] { "ProcessoId" });
            DropIndex("dbo.AtividadeFuncaos", new[] { "IdFuncao" });
            DropIndex("dbo.AtividadeFuncaos", new[] { "IdAtividade" });
            DropIndex("dbo.Atividades", new[] { "IdAtividadeTipo" });
            DropTable("dbo.VisaoCampoes");
            DropTable("dbo.TabelaCampoHashcodes");
            DropTable("dbo.ProcessoExecucaoAtividades");
            DropTable("dbo.ProcessoArquivoes");
            DropTable("dbo.UsuarioFuncaos");
            DropTable("dbo.Usuarios");
            DropTable("dbo.ProcessoExecucaos");
            DropTable("dbo.ProcessoAnotacaos");
            DropTable("dbo.Hashcodes");
            DropTable("dbo.TipoCampoes");
            DropTable("dbo.TabelaCampoes");
            DropTable("dbo.Empresas");
            DropTable("dbo.Tabelas");
            DropTable("dbo.Visaos");
            DropTable("dbo.AtividadeVisaos");
            DropTable("dbo.Versaos");
            DropTable("dbo.Processoes");
            DropTable("dbo.AtividadePlanoes");
            DropTable("dbo.Funcaos");
            DropTable("dbo.AtividadeFuncaos");
            DropTable("dbo.AtividadeTipoes");
            DropTable("dbo.Atividades");
            DropTable("dbo.Arquivos");
        }
    }
}

namespace SoftFin.POC.ConforLab.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Full : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BAS_CAD_TECNICO",
                c => new
                    {
                        BCT_ID = c.Int(nullable: false, identity: true),
                        BCT_NOME = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.BCT_ID);
            
            CreateTable(
                "dbo.BAS_CLIENTE",
                c => new
                    {
                        BCL_ID = c.Int(nullable: false, identity: true),
                        BCL_RAZAO = c.String(maxLength: 400),
                        BCL_NOME_FANTASIA = c.String(maxLength: 400),
                        BCL_ENDERECO = c.String(maxLength: 300),
                        BCL_BAIRRO = c.String(maxLength: 300),
                        BCL_CIDADE = c.String(maxLength: 300),
                        BCL_UF = c.String(maxLength: 2),
                        BCL_CEP = c.String(maxLength: 9),
                        BCL_CONTATO_AGENDAMENTO = c.String(maxLength: 300),
                        BCL_EMAIL = c.String(maxLength: 300),
                        BCL_TELEFONE1 = c.String(maxLength: 13),
                        BCL_CONTATO_LOCAL = c.String(maxLength: 300),
                        BCL_DEPARTAMENTO = c.String(maxLength: 300),
                        BCL_TELEFONE2 = c.String(maxLength: 13),
                    })
                .PrimaryKey(t => t.BCL_ID);
            
            CreateTable(
                "dbo.BAS_GP_ANALISE",
                c => new
                    {
                        BGA_ID = c.Int(nullable: false, identity: true),
                        BGA_GRUPO = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.BGA_ID);
            
            CreateTable(
                "dbo.BAS_PROCEDIMENTO",
                c => new
                    {
                        BGA_ID = c.Int(nullable: false, identity: true),
                        BGA_GRUPO = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.BGA_ID);
            
            CreateTable(
                "dbo.BAS_TEMPERATURA",
                c => new
                    {
                        BPR_ID = c.Int(nullable: false, identity: true),
                        BPR_PROCEDIMENTO = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.BPR_ID);
            
            CreateTable(
                "dbo.POC_RETIRADA_ANALISE_AMOSTRA_CAB",
                c => new
                    {
                        RAC_ID = c.Int(nullable: false, identity: true),
                        PRE_ID = c.Int(nullable: false),
                        PEA_ID = c.Int(nullable: false),
                        RAC_N_PEDIDO = c.String(maxLength: 18),
                        BCL_ID = c.Int(nullable: false),
                        BCT_ID = c.Int(nullable: false),
                        BGA_ID = c.Int(nullable: false),
                        BPR_ID = c.Int(nullable: false),
                        BTE_ID = c.Int(nullable: false),
                        BCL_NOME_FANTASIA = c.String(maxLength: 400),
                        BCL_ENDERECO = c.String(maxLength: 300),
                        BCL_BAIRRO = c.String(maxLength: 300),
                        BCL_CIDADE = c.String(maxLength: 300),
                        BCL_UF = c.String(maxLength: 2),
                        BCL_CEP = c.String(maxLength: 9),
                        BCL_CONTATO_AGENDAMENTO = c.String(maxLength: 300),
                        BCL_EMAIL = c.String(maxLength: 300),
                        BCL_TELEFONE1 = c.String(maxLength: 13),
                        BCL_CONTATO_LOCAL = c.String(maxLength: 300),
                        BCL_DEPARTAMENTO = c.String(maxLength: 300),
                        BCL_TELEFONE2 = c.String(maxLength: 13),
                        RAC_PROC_AMOSTRA = c.String(maxLength: 400),
                        RAC_N_PONTOS = c.String(maxLength: 100),
                        RAC_ACOMPANHANTE = c.String(maxLength: 300),
                        RAC_DT_RET_AMOSTRA = c.DateTime(nullable: false),
                        RAC_COMPLEMENTO = c.String(maxLength: 200),
                        RAC_DT_REC_AMOSTRA = c.DateTime(nullable: false),
                        RAC_H_REC_AMOSTRA_LAB = c.DateTime(nullable: false),
                        RAC_CONFERIDO = c.String(maxLength: 300),
                        RAC_PLANO = c.String(maxLength: 10),
                        RAC_N_PATRIMONIO = c.String(maxLength: 200),
                        RAC_VER_APA = c.String(maxLength: 200),
                        RAC_BUFFER_1 = c.String(maxLength: 5),
                        RAC_BUFFER_2 = c.String(maxLength: 5),
                        RAC_BUFFER_3 = c.String(maxLength: 5),
                        RAC_DATA = c.DateTime(nullable: false),
                        RAC_PLAN_AMOSTRAGEM = c.String(maxLength: 400),
                        RAC_OBS = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.RAC_ID)
                .ForeignKey("dbo.BAS_CAD_TECNICO", t => t.BCT_ID, cascadeDelete: true)
                .ForeignKey("dbo.BAS_CLIENTE", t => t.BCL_ID, cascadeDelete: true)
                .ForeignKey("dbo.BAS_PROCEDIMENTO", t => t.BGA_ID, cascadeDelete: true)
                .Index(t => t.BCL_ID)
                .Index(t => t.BCT_ID)
                .Index(t => t.BGA_ID);
            
            CreateTable(
                "dbo.POC_RETIRADA_ANALISE_AMOSTRA_ITEM",
                c => new
                    {
                        RAC_ID = c.Int(nullable: false, identity: true),
                        RAI_ID = c.Int(nullable: false),
                        RAI_NUMERO = c.String(maxLength: 2),
                        RAI_PH = c.String(maxLength: 100),
                        RAI_TEMP = c.String(maxLength: 100),
                        RAI_CLORO = c.String(maxLength: 100),
                        RAI_LOCAL = c.String(maxLength: 100),
                        RAI_HORA = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RAC_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.POC_RETIRADA_ANALISE_AMOSTRA_CAB", "BGA_ID", "dbo.BAS_PROCEDIMENTO");
            DropForeignKey("dbo.POC_RETIRADA_ANALISE_AMOSTRA_CAB", "BCL_ID", "dbo.BAS_CLIENTE");
            DropForeignKey("dbo.POC_RETIRADA_ANALISE_AMOSTRA_CAB", "BCT_ID", "dbo.BAS_CAD_TECNICO");
            DropIndex("dbo.POC_RETIRADA_ANALISE_AMOSTRA_CAB", new[] { "BGA_ID" });
            DropIndex("dbo.POC_RETIRADA_ANALISE_AMOSTRA_CAB", new[] { "BCT_ID" });
            DropIndex("dbo.POC_RETIRADA_ANALISE_AMOSTRA_CAB", new[] { "BCL_ID" });
            DropTable("dbo.POC_RETIRADA_ANALISE_AMOSTRA_ITEM");
            DropTable("dbo.POC_RETIRADA_ANALISE_AMOSTRA_CAB");
            DropTable("dbo.BAS_TEMPERATURA");
            DropTable("dbo.BAS_PROCEDIMENTO");
            DropTable("dbo.BAS_GP_ANALISE");
            DropTable("dbo.BAS_CLIENTE");
            DropTable("dbo.BAS_CAD_TECNICO");
        }
    }
}

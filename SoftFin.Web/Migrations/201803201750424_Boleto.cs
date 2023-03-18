namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Boleto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boletoes", "DataVencimento", c => c.DateTime(nullable: false));
            AddColumn("dbo.Boletoes", "NumeroDoc", c => c.String(nullable: false, maxLength: 15));
            AddColumn("dbo.Boletoes", "NotaFiscal_ID", c => c.Int());
            AddColumn("dbo.Boletoes", "CnabGerado", c => c.Boolean(nullable: false));
            AddColumn("dbo.Boletoes", "CnabDataGerado", c => c.DateTime());
            AddColumn("dbo.Boletoes", "CnabCancelado", c => c.Boolean(nullable: false));
            AddColumn("dbo.Boletoes", "CnabDataCancelado", c => c.DateTime());
            AlterColumn("dbo.Boletoes", "DataPagamento", c => c.DateTime());
            AlterColumn("dbo.Boletoes", "ValorPago", c => c.Decimal(precision: 18, scale: 2));
            CreateIndex("dbo.Boletoes", "NotaFiscal_ID");
            AddForeignKey("dbo.Boletoes", "NotaFiscal_ID", "dbo.NotaFiscals", "id");
            DropColumn("dbo.Boletoes", "Data");
            DropColumn("dbo.Boletoes", "numeroDocumento");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Boletoes", "numeroDocumento", c => c.String(nullable: false, maxLength: 3));
            AddColumn("dbo.Boletoes", "Data", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Boletoes", "NotaFiscal_ID", "dbo.NotaFiscals");
            DropIndex("dbo.Boletoes", new[] { "NotaFiscal_ID" });
            AlterColumn("dbo.Boletoes", "ValorPago", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Boletoes", "DataPagamento", c => c.DateTime(nullable: false));
            DropColumn("dbo.Boletoes", "CnabDataCancelado");
            DropColumn("dbo.Boletoes", "CnabCancelado");
            DropColumn("dbo.Boletoes", "CnabDataGerado");
            DropColumn("dbo.Boletoes", "CnabGerado");
            DropColumn("dbo.Boletoes", "NotaFiscal_ID");
            DropColumn("dbo.Boletoes", "NumeroDoc");
            DropColumn("dbo.Boletoes", "DataVencimento");
        }
    }
}

namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180831Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParcelaContratoes", "DataVencimento", c => c.DateTime());
            AddColumn("dbo.Bancoes", "ReferenciaIntegracao", c => c.String(maxLength: 20));
            AddColumn("dbo.DocumentoPagarMestres", "QtdArquivosUpload", c => c.Int(nullable: false));
            AddColumn("dbo.DocumentoPagarMestres", "ReferenciaProjeto", c => c.String());
            AddColumn("dbo.NotaFiscals", "DataVencimentoPrevisto", c => c.DateTime());
            Sql("update DocumentoPagarMestres set QtdArquivosUpload = (select count(*) from DocumentoPagarArquivoes cpa where cpa.documentoPagarMestre_id = DocumentoPagarMestres.id )");
            Sql("update DocumentoPagarMestres set ReferenciaProjeto = (select Projetoes.nomeProjeto from Projetoes where id = DocumentoPagarMestres.Projeto_Id )");

        }

        public override void Down()
        {
            DropColumn("dbo.NotaFiscals", "DataVencimentoPrevisto");
            DropColumn("dbo.DocumentoPagarMestres", "ReferenciaProjeto");
            DropColumn("dbo.DocumentoPagarMestres", "QtdArquivosUpload");
            DropColumn("dbo.Bancoes", "ReferenciaIntegracao");
            DropColumn("dbo.ParcelaContratoes", "DataVencimento");
        }
    }
}

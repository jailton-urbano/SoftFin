namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180912Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotaFiscals", "DataVencimentoOriginal", c => c.DateTime(nullable: false));
            Sql("update dbo.NotaFiscals set DataVencimentoOriginal = dataVencimentoNfse where DataVencimentoOriginal is null;");
            AlterColumn("dbo.NotaFiscals", "DataVencimentoOriginal", c => c.DateTime(nullable: false));
            DropColumn("dbo.NotaFiscals", "DataVencimentoPrevisto");
            Sql("update dbo.NotaFiscals set DataVencimentoOriginal = dataVencimentoNfse where year(DataVencimentoOriginal) = 1900;");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NotaFiscals", "DataVencimentoPrevisto", c => c.DateTime(nullable: false));
            DropColumn("dbo.NotaFiscals", "DataVencimentoOriginal");
        }
    }
}

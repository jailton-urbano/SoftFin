namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180928Ricardo01 : DbMigration
    {
        public override void Up()
        {
            Sql("update dbo.NotaFiscals set DataVencimentoPrevisto = dataVencimentoNfse where DataVencimentoPrevisto is null;");
            AlterColumn("dbo.NotaFiscals", "DataVencimentoPrevisto", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.NotaFiscals", "DataVencimentoPrevisto", c => c.DateTime());
        }
    }
}

namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180514Ricardo01 : DbMigration
    {
        public override void Up()
        {
            Sql("update dbo.DocumentoPagarParcelas set historico = '.' where historico is null");
            AlterColumn("dbo.DocumentoPagarParcelas", "historico", c => c.String(nullable: false, maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DocumentoPagarParcelas", "historico", c => c.String(maxLength: 500));
        }
    }
}

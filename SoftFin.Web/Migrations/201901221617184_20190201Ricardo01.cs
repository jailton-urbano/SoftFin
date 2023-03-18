namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190201Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotaFiscals", "ObraNumEncapsulamento", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NotaFiscals", "ObraNumEncapsulamento");
        }
    }
}

namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181112Ricardo02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boletoes", "CodigoTransmissao", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boletoes", "CodigoTransmissao");
        }
    }
}

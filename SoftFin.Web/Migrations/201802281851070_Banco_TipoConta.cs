namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Banco_TipoConta : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bancoes", "TipoConta", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bancoes", "TipoConta");
        }
    }
}

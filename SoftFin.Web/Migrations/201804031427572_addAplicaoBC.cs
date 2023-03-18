namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAplicaoBC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bancoes", "AplicacaoAutomatica", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bancoes", "AplicacaoAutomatica");
        }
    }
}

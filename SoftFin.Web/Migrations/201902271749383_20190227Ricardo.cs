namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190227Ricardo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContaContabils", "Ativo", c => c.Boolean(nullable: false));
            AddColumn("dbo.PlanoDeContas", "Ativo", c => c.Boolean(nullable: false));
            Sql("update dbo.ContaContabils set Ativo = 1");
            Sql("update dbo.PlanoDeContas set Ativo = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlanoDeContas", "Ativo");
            DropColumn("dbo.ContaContabils", "Ativo");
        }
    }
}

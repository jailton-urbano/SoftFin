namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M04052018Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projetoes", "ativo", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projetoes", "ativo");
        }
    }
}

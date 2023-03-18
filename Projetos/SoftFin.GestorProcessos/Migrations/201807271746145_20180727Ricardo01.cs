namespace SoftFin.GestorProcessos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180727Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AtividadePlanoes", "Inicial", c => c.Boolean(nullable: false));
            AddColumn("dbo.AtividadePlanoes", "Final", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AtividadePlanoes", "Final");
            DropColumn("dbo.AtividadePlanoes", "Inicial");
        }
    }
}

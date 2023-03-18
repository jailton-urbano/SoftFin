namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181022Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bancoes", "JurosDia", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Bancoes", "Multa", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Bancoes", "SequencialLoteCNAB", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bancoes", "SequencialLoteCNAB");
            DropColumn("dbo.Bancoes", "Multa");
            DropColumn("dbo.Bancoes", "JurosDia");
        }
    }
}

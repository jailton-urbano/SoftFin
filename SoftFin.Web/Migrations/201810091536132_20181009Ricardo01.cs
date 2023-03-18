namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181009Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boletoes", "Multa", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Boletoes", "JurosMora", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boletoes", "JurosMora");
            DropColumn("dbo.Boletoes", "Multa");
        }
    }
}

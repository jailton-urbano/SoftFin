namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181002Ricardo02 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.IBPTs", "Descricao", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.IBPTs", "Descricao", c => c.String(maxLength: 150));
        }
    }
}

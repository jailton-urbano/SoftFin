namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190201Ricardo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SistemaDashBoardFuncionalidades", "Ordem", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SistemaDashBoardFuncionalidades", "Ordem");
        }
    }
}

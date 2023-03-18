namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181129Ricardo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SituacaoTributariaNotas", "CodeMigrate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SituacaoTributariaNotas", "CodeMigrate");
        }
    }
}

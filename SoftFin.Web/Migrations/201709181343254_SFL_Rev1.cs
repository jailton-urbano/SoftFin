namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SFL_Rev1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SFDetalhes", "flag_debcred", c => c.String(maxLength: 1));
            AlterColumn("dbo.SFDetalhes", "tabela", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SFDetalhes", "tabela", c => c.String());
            DropColumn("dbo.SFDetalhes", "flag_debcred");
        }
    }
}

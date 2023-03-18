namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20191119Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Estabelecimentoes", "MigateCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Estabelecimentoes", "MigateCode");
        }
    }
}

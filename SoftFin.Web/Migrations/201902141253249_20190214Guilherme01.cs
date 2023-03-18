namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20190214Guilherme01 : DbMigration
    {
        public override void Up()
        {

            AlterColumn("dbo.ContaContabils", "Tipo", c => c.String(maxLength: 1));
            AlterColumn("dbo.ContaContabils", "SubCategoria", c => c.String(maxLength: 3));
            DropColumn("dbo.ContaContabils", "DataVigenciaInicial");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContaContabils", "DataVigenciaInicial", c => c.DateTime());
            AlterColumn("dbo.ContaContabils", "SubCategoria", c => c.String(maxLength: 2));
            AlterColumn("dbo.ContaContabils", "Tipo", c => c.String(maxLength: 2));
        }
    }
}

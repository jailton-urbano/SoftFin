namespace SoftFin.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181015Ricardo02 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogComercials", "Assunto", c => c.String(maxLength: 1000));
            AddColumn("dbo.LogComercials", "Mensagem", c => c.String());
            AddColumn("dbo.LogComercials", "Local", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LogComercials", "Local");
            DropColumn("dbo.LogComercials", "Mensagem");
            DropColumn("dbo.LogComercials", "Assunto");
        }
    }
}

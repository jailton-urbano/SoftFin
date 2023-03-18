namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Boletoes", "pessoas_ID", "dbo.Pessoas");
            DropIndex("dbo.Boletoes", new[] { "pessoas_ID" });
            DropColumn("dbo.Boletoes", "pessoas_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Boletoes", "pessoas_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Boletoes", "pessoas_ID");
            AddForeignKey("dbo.Boletoes", "pessoas_ID", "dbo.Pessoas", "id", cascadeDelete: true);
        }
    }
}

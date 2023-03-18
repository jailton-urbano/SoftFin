namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bm_with_unidades : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BancoMovimentoes", "UnidadeNegocio_id", c => c.Int());
            CreateIndex("dbo.BancoMovimentoes", "UnidadeNegocio_id");
            AddForeignKey("dbo.BancoMovimentoes", "UnidadeNegocio_id", "dbo.UnidadeNegocios", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BancoMovimentoes", "UnidadeNegocio_id", "dbo.UnidadeNegocios");
            DropIndex("dbo.BancoMovimentoes", new[] { "UnidadeNegocio_id" });
            DropColumn("dbo.BancoMovimentoes", "UnidadeNegocio_id");
        }
    }
}

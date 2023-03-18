namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180918Ricardo01 : DbMigration
    {
        public override void Up()
        {
            Sql("insert into DocumentoPagarProjetoes select DPM.id, '...', DPM.valorBruto, DPM.projeto_id  from DocumentoPagarMestres DPM where DPM.projeto_id is not null");
            DropForeignKey("dbo.DocumentoPagarMestres", "Projeto_Id", "dbo.Projetoes");
            DropIndex("dbo.DocumentoPagarMestres", new[] { "Projeto_Id" });
            DropColumn("dbo.DocumentoPagarMestres", "Projeto_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DocumentoPagarMestres", "Projeto_Id", c => c.Int());
            CreateIndex("dbo.DocumentoPagarMestres", "Projeto_Id");
            AddForeignKey("dbo.DocumentoPagarMestres", "Projeto_Id", "dbo.Projetoes", "id");
        }
    }
}

namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BM_Valida_Historico : DbMigration
    {
        public override void Up()
        {
            Sql("update BancoMovimentoes set historico = 'Corrigir' where historico is null");
            AlterColumn("dbo.BancoMovimentoes", "historico", c => c.String(nullable: false, maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BancoMovimentoes", "historico", c => c.String(maxLength: 500));
        }
    }
}

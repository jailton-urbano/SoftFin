namespace SoftFin.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181015Ricardo01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogComercials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoSistema = c.String(maxLength: 15),
                        Estabelecimento = c.String(maxLength: 50),
                        Data = c.DateTime(nullable: false),
                        DataRecebida = c.DateTime(nullable: false),
                        Nome = c.String(maxLength: 300),
                        NomeEmpresa = c.String(maxLength: 300),
                        Email = c.String(maxLength: 300),
                        Telefone = c.String(maxLength: 50),
                        Senha = c.String(maxLength: 500),
                        TemEmpresa = c.String(maxLength: 50),
                        IP = c.String(maxLength: 50),
                        IPLogado = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.LogEventoes", "IPLogado", c => c.String(maxLength: 50));
            AddColumn("dbo.LogImportars", "IPLogado", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LogImportars", "IPLogado");
            DropColumn("dbo.LogEventoes", "IPLogado");
            DropTable("dbo.LogComercials");
        }
    }
}

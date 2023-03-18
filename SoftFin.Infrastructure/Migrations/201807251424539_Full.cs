namespace SoftFin.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Full : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogEventoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoSistema = c.String(maxLength: 15),
                        Estabelecimento = c.String(),
                        Data = c.DateTime(nullable: false),
                        Usuario = c.String(maxLength: 400),
                        Agrupador = c.Guid(nullable: false),
                        Tipo = c.String(maxLength: 15),
                        Ip = c.String(maxLength: 50),
                        Rotina = c.String(maxLength: 4000),
                        Descricao = c.String(maxLength: 4000),
                        Exception = c.String(maxLength: 4000),
                        Json = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogImportars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoSistema = c.String(maxLength: 15),
                        Estabelecimento = c.String(),
                        Data = c.DateTime(nullable: false),
                        Agrupador = c.Guid(nullable: false),
                        Usuario = c.String(maxLength: 400),
                        Tipo = c.String(maxLength: 15),
                        Ip = c.String(maxLength: 50),
                        Descricao = c.String(maxLength: 4000),
                        Json = c.String(),
                        Registro = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LogImportars");
            DropTable("dbo.LogEventoes");
        }
    }
}

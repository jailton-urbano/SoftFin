namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20181002Ricardo01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IBPTs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UF = c.String(),
                        Codigo = c.String(maxLength: 9),
                        Ex = c.String(maxLength: 1),
                        Tipo = c.String(maxLength: 1),
                        Descricao = c.String(maxLength: 150),
                        Nacionalfederal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Importadosfederal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Estadual = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Municipal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Vigenciainicio = c.DateTime(nullable: false),
                        Vigenciafim = c.DateTime(nullable: false),
                        Versao = c.String(maxLength: 15),
                        Fonte = c.String(maxLength: 15),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IBPTs");
        }
    }
}

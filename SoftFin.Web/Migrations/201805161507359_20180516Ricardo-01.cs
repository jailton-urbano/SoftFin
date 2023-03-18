namespace SoftFin.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20180516Ricardo01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contratoes", "DataInicioVigencia", c => c.DateTime());
            AddColumn("dbo.Contratoes", "DataFinalVigencia", c => c.DateTime());
            AddColumn("dbo.Contratoes", "MunicipioPrestador_id", c => c.Int());
            AddColumn("dbo.Contratoes", "Vendedor_id", c => c.Int());
            CreateIndex("dbo.Contratoes", "MunicipioPrestador_id");
            CreateIndex("dbo.Contratoes", "Vendedor_id");
            AddForeignKey("dbo.Contratoes", "MunicipioPrestador_id", "dbo.Municipios", "ID_MUNICIPIO");
            AddForeignKey("dbo.Contratoes", "Vendedor_id", "dbo.Pessoas", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contratoes", "Vendedor_id", "dbo.Pessoas");
            DropForeignKey("dbo.Contratoes", "MunicipioPrestador_id", "dbo.Municipios");
            DropIndex("dbo.Contratoes", new[] { "Vendedor_id" });
            DropIndex("dbo.Contratoes", new[] { "MunicipioPrestador_id" });
            DropColumn("dbo.Contratoes", "Vendedor_id");
            DropColumn("dbo.Contratoes", "MunicipioPrestador_id");
            DropColumn("dbo.Contratoes", "DataFinalVigencia");
            DropColumn("dbo.Contratoes", "DataInicioVigencia");
        }
    }
}

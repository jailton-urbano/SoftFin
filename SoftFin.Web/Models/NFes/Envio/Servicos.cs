using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class Servicos     {
        public string ItemListaServico { get; set; }
        public int? CodigoCnae { get; set; }
        public String CodigoTributacaoMunicipio { get; set; }
        public String Discriminacao { get; set; }
        public int MunicipioIncidencia { get; set; }
        public int? MunicipioIncidenciaSiafi { get; set; }
        public String NumeroProcesso { get; set; }
        public String DescricaoRPS { get; set; }
        public int IssRetido { get; set; }
        public int? ResponsavelRetencao { get; set; }
        public int? MunicipioIncidenciaOutros { get; set; }
        public int? ServicoPrestadoViasPublicas { get; set; }
        public int? ServicoExportacao { get; set; }
        public int? Observacao { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class DadosComplementaresOperacao
    {
        public int LocalTributacao { get; set; }
        public int Incidencia { get; set; }
        public int FormaContribuicao { get; set; }
        public int Deducoes { get; set; }
        public int Natureza { get; set; }
        public int SituacaoDocumento { get; set; }
        public int TipoEmpresa { get; set; }
    }
}
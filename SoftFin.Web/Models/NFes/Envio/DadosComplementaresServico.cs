using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models.NFes.Envio
{
    public class DadosComplementaresServico
    {
        public int TipoRecolhimento { get; set; }
        public String MotivoRetencao { get; set; }
        public String MunicipioPrestacaoDescricao { get; set; }
        public int SeriePrestacao { get; set; }
        public String MotCancelamento { get; set; }

    }
}
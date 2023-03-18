using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class FolhaExportacaoController : Controller
    {
        //
        // GET: /FolhaExportacao/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GerarArquivo()
        {
            StringBuilder sb = new StringBuilder();
            var item = new folhalayout();
            sb.AppendFormat("{0};", item.idInterna.ToString());
            sb.AppendFormat("{0};", item.numRegistro.ToString());
            sb.AppendFormat("{0};", item.data.ToString());
            sb.AppendFormat("{0};", item.PeriodoPostagem.ToString());
            sb.AppendFormat("{0};", item.SubSidiaria.ToString());
            sb.AppendFormat("{0};", item.Conta.ToString());
            sb.AppendFormat("{0};", item.Debito.ToString());
            sb.AppendFormat("{0};", item.Credito.ToString());
            sb.AppendFormat("{0};", item.memo.ToString());
            sb.AppendFormat("{0};", item.departamento.ToString());
            sb.AppendFormat("{0};", item.centroCusto.ToString());
            sb.AppendLine();

            var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "FOLHA_" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".txt");
            

        }

        public class folhalayout
        {
            public int idInterna { get; set; }
            public int numRegistro { get; set; }
            public DateTime data { get; set; }
            public string PeriodoPostagem { get; set; }
            public string SubSidiaria { get; set; }
            public string Conta { get; set; }
            public decimal? Debito { get; set; }
            public decimal? Credito { get; set; }
            public string memo { get; set; }
            public string departamento { get; set; }
            public string centroCusto { get; set; }
        }


    }
}

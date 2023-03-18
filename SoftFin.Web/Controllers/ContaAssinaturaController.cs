using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.Models;
using System.Web.Helpers;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using Lib.Web.Mvc.JQuery.JqGrid;

namespace SoftFin.Web.Controllers
{
    public class ContaAssinaturaController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {

            var regs = new ContaAssinatura().ObterTodos(_paramBase);
            int totalRecords = regs.Count();

            //Fix for grouping, because it adds column name instead of index to SortingName
            //string sortingName = "cdg_perfil";

            //Prepare JqGridData instance
            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            regs = regs.OrderBy(p => p.dataInicial).Skip(12 * request.PageIndex).Take(12).ToList();

            //Table with rows data
            foreach (var item in
                regs)
            {
                var estab = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);
                var nfps = "";

                if (item.notaFiscal != 0)
                {
                    string url = "https://nfe.prefeitura.sp.gov.br/contribuinte/notaprint.aspx?inscricao={inscricao}&nf={numeronf}&verificacao={codigoverificacao}";

                    url = url.Replace("{codigoverificacao}", item.codigoVerificacao);
                    url = url.Replace("{numeronf}", item.notaFiscal.ToString());
                    url = url.Replace("{inscricao}", "37953621");

                    nfps = "<a href='{url}' target='_blank' >{valor}</a>";
                    nfps = nfps.Replace("{url}", url);
                    nfps = nfps.Replace("{valor}", item.notaFiscal.ToString());
                }

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.dataInicial.ToShortDateString(),
                    item.dataFinal.ToShortDateString(),
                    nfps,
                    item.codigoVerificacao,
                    item.dataVencimentoNotaFiscal.ToShortDateString(),
                    item.valor.ToString("n"),
                    item.situacao,
                    item.historico
                }));
            }


            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }

    }
}

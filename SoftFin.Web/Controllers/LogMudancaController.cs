using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

using System.Configuration;
using System.IO;
using System.Text;


namespace SoftFin.Web.Controllers
{
    public class LogMudancaController : BaseController
    {

        public ActionResult Index()
        {
            CarregaViewData();
            return View();
        }

        private void CarregaViewData()
        {
            ViewData["usuario"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase).OrderBy(x => x.nome), "id", "nome");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Listas(JqGridRequest request)
        {
            string usuario = Request.QueryString["usuario"];
            string dataInicial = Request.QueryString["dataInicial"];
            string dataFinal = Request.QueryString["dataFinal"];
            DateTime aux1;
            DateTime.TryParse(dataInicial, out aux1);
            DateTime aux2;
            DateTime.TryParse(dataFinal, out aux2);
            int usuarioAux;
            int.TryParse(usuario, out usuarioAux);
            var db = new DbControle();
            var regs = new LogMudanca().ObterTodos(_paramBase).Where(p => p.usuario_id == usuarioAux && p.data >= aux1 && p.data <= aux2.AddDays(1)).ToList();
            var totalRecords = regs.Count();

            JqGridResponse response = new JqGridResponse()
            {
                //Total pages count
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                //Page number
                PageIndex = request.PageIndex,
                //Total records count
                TotalRecordsCount = totalRecords
            };

            regs = regs.OrderBy(p => p.data).Skip(12 * request.PageIndex).Take(12).ToList();

            //Table with rows data
            foreach (var item in
                regs)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.data.ToString(),
                    item.Usuario.nome,
                    item.metodo,
                    item.registroNovo,
                    item.registroAnterior
                }));
            }

            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }


    }
}

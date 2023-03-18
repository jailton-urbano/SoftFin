using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class RelatorioDadosCadastraisEstabelecimentoController : BaseController
    {
        public ActionResult Index()
        {
            ViewData["Estabelecimento"] = new SelectList(new Estabelecimento().ObterTodos(_paramBase), "id", "NomeCompleto");

            return View();
        }


        public class DtoEmpresa
        {
            public DtoEmpresa()
            {
                bancos = new List<DtoBanco>(); 
            }
            public string RazaoSocial { get; set; }
            public string Endereco { get; set; }
            public string Numero { get; set; }
            public string Bairro { get; set; }
            public string UF { get; set; }
            public string Complemento { get; set; }
            public string Cidade { get; set; }
            public string Cep { get; set; }
            public string CNPJ { get; set; }
            public string IEstadual { get; set; }
            public string IMunicipal { get; set; }

            public List<DtoBanco> bancos { get; set; }

        }


        public class DtoBanco
        {
            public string banco { get; set; }
            public string codigo { get; set; }
            public string agencia { get; set; }
            public string conta { get; set; }
            

        }

        public JsonResult GetReport(int estabelecimento_id)
        {
            var estab = new Estabelecimento().ObterPorId(estabelecimento_id, _paramBase);
            var DtoEmpresa = new DtoEmpresa();

            DtoEmpresa.RazaoSocial= estab.NomeCompleto;
            DtoEmpresa.Endereco= estab.Logradouro;
            DtoEmpresa.Numero= estab.NumeroLogradouro.ToString();
            DtoEmpresa.Bairro= estab.BAIRRO;
            DtoEmpresa.UF= estab.UF;
            DtoEmpresa.Complemento= estab.Complemento;
            DtoEmpresa.Cep= estab.CEP;
            DtoEmpresa.CNPJ= estab.CNPJ;
            DtoEmpresa.IEstadual = estab.InscricaoEstadual;
            DtoEmpresa.IMunicipal= estab.InscricaoMunicipal.ToString();

            var bancos = new Banco().ObterTodosEstab(estabelecimento_id);

            foreach (var item in bancos)
	        {
                

		        DtoEmpresa.bancos.Add(new DtoBanco{ agencia = item.agencia + " - " + item.agenciaDigito ,
                     banco = item.nomeBanco,
                     codigo = item.codigo,
                     conta = item.contaCorrente + " - " + item.contaCorrenteDigito
                 });
	        }
            


            return Json(DtoEmpresa, JsonRequestBehavior.AllowGet);
        }
 
    }
}

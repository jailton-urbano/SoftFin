using SoftFin.Utils;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class CobrancaController : BaseController
    {
        // GET: Cobranca
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ObterCobranca()
        {
            var objs = new OrdemVenda().ObterTodosEmitidos(_paramBase);
            
            var dtos = new List<DTOCobranca>();

            
            foreach (var item in objs)
            {
                var objcliente = item.Pessoa.nome;
                var objEmail = item.Pessoa.eMail;
                var objTipo = "";
                decimal objValor = 0;
                var objVencimento = DateTime.Now;

                var nf = new NotaFiscal().ObterPorOV(item.id);


                if (nf == null)
                    continue;

                if (nf.SituacaoRecebimento != 1) //Em Aberto
                    continue;

                if (nf.situacaoPrefeitura_id != NotaFiscal.NFGERADAENVIADA)
                    continue;

                if (nf.dataVencimentoNfse >= SoftFin.Utils.UtilSoftFin.DateBrasilia().AddDays(-3))
                    continue;


                var contatos = new PessoaContato().ObterPorTodos(item.id, _paramBase);

                foreach (var cnt in contatos)
                {
                    if (cnt.RecebeCobranca)
                        objEmail += ";" + cnt.email;
                }



                switch (nf.TipoFaturamento)
                {
                    case 0:
                        objTipo = "Nota Fiscal de Serviço";
                        objValor = nf.valorNfse;
                        objVencimento = nf.dataVencimentoNfse;
                        break;
                    case 1:
                        objTipo = "Nota Fiscal de Mercadoria";
                        objValor = nf.valorNfse;
                        objVencimento = nf.dataVencimentoNfse;
                        break;
                    case 2:
                        objTipo = "Outros Recebimento";
                        objValor = nf.valorNfse;
                        objVencimento = nf.dataVencimentoNfse;
                        break;
                    default:
                        break;
                }

                dtos.Add(new DTOCobranca
                {
                    Cliente = objcliente,
                    Email = objEmail,
                    Enviar = true,
                    ov_id = item.id,
                    Tipo = objTipo,
                    Valor = objValor,
                    Vencimento = objVencimento,
                    Numero = item.Numero.ToString()
                });
            }

            return Json(dtos.Select(p => new
            {
                p.Cliente,
                p.Email,
                p.Enviar,
                p.ov_id,
                p.Tipo,
                Vencimento = p.Vencimento.ToString("o"),
                p.Valor,
                p.Numero
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Salvar(string titulo, string texto, List<DTOCobranca> objs)
        {
            try
            {
                foreach (var item in objs)
                {
                    if (item.Enviar)
                    {
                        var email = new Email();
                        email.EnviarMensagem(item.Email, titulo, texto, false);
                        item.texto = texto;
                        item.titulo = titulo;
                        _eventos.Info("Envio de Email de Cobrança", item);
                    }
                }

                return Json(new { CDStatus = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message.ToString()}, JsonRequestBehavior.AllowGet);
            }

        }

        public class DTOCobranca
        {
            public int ov_id { get; set; }
            public bool Enviar { get; set; }
            public string Tipo { get; set; }
            public string Cliente { get; set; }
            public string Email { get; set; }
            public DateTime Vencimento { get; set; }
            public decimal Valor { get; set; }

            public string texto { get; set; }

            public string titulo { get; set; }

            public string Numero { get; set; }
        }
    }
}
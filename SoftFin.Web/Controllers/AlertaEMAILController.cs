using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class AlertaEMAILController : Controller
    {
        //
        // GET: /AlertaEMAIL/

        public JsonResult AvisoPagamentos()
        {
            var email = new Email();

            var usuarios = new Usuario().ObterTodosEmail();
            

            foreach (var usuario in usuarios)
            {
                var usuestabs = new UsuarioEstabelecimento().ObterTodosEmail(usuario.id);
                var tabela = new StringBuilder();

                tabela.AppendLine("<img src='http://softfin-homolog.azurewebsites.net/Content/images/SoftFin_Logo2.png'/>");
                tabela.AppendLine("<h2>Aviso de Contas a Pagar</H2>");
                var envia = false;            

                foreach (var usuestab in usuestabs)
                {
                    var cpags = new DocumentoPagarParcela().ObterTodosEmail(usuestab.estabelecimento_id);
                    if (cpags.Count == 0)
                        continue;

                    envia = true;
                    var empresa = new Empresa().ObterPorId(usuario.empresa_id);
                    var estab = new Estabelecimento().ObterPorIdMail(usuestab.estabelecimento_id);
                                        
                    tabela.AppendLine("Empresa:");
                    tabela.AppendLine(empresa.nome);
                    tabela.AppendLine("Estabelecimento/Filial:");
                    tabela.AppendLine(estab.NomeCompleto + " - CNPJ: " + estab.CNPJ);

                    tabela.AppendLine("<table>");
                    tabela.AppendLine(" <thead>");
                    tabela.AppendLine("     <tr>");
                    tabela.AppendLine("         <th width='20%'>Vencimento</th>");
                    tabela.AppendLine("         <th width='20%'>Valor</th>");
                    tabela.AppendLine("         <th width='60%'>Pessoa</th>");
                    tabela.AppendLine("     </tr>");
                    tabela.AppendLine(" </thead>");
                    tabela.AppendLine(" <tbody>");
                    
                    foreach (var cpag in cpags)
                    {
                        tabela.AppendLine("     <tr>");
                        tabela.AppendLine("         <td>" + cpag.vencimentoPrevisto.ToShortDateString() + "</td>");
                        tabela.AppendLine("         <td>" + cpag.valor.ToString("n") + "</td>");
                        tabela.AppendLine("         <td>" + cpag.DocumentoPagarMestre.Pessoa.nome + "</td>");
                        tabela.AppendLine("     </tr>");
                    }

                    tabela.AppendLine(" </tbody>");
                    tabela.AppendLine("</table>");

                    tabela.AppendLine("<br><br><br><br>Atenciosamente<br>Equipe SoftFin<br><a href='mailto:adm@softfin.com.br' >adm@softfin.com.br</a><br>" + 
                        ConfigurationManager.AppSettings["textocomplementaremail"].ToString());

                }

                if (envia)
                {
                    try
                    {
                        email.EnviarMensagem("ricardo.santos@softfin.com.br",
                            "Aviso de Contas a Pagar vencendo nos próximos dias",
                            tabela.ToString(), true);
                    }
                    catch
                    {
                    }
                }
            }


            return Json("OK-" + usuarios.Count().ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AvisoParcelaContratos()
        {
            var email = new Email();

            var usuarios = new Usuario().ObterTodosEmail();


            foreach (var usuario in usuarios)
            {
                var usuestabs = new UsuarioEstabelecimento().ObterTodosEmail(usuario.id);
                var tabela = new StringBuilder();

                tabela.AppendLine("<img src='http://softfin-homolog.azurewebsites.net/Content/images/SoftFin_Logo2.png'/>");
                tabela.AppendLine("<h2>Aviso de Parcelas de Contratos</H2>");
                var envia = false;

                foreach (var usuestab in usuestabs)
                {
                    var cpags = new ParcelaContrato().ObterTodosEmail(usuestab.estabelecimento_id);
                    if (cpags.Count == 0)
                        continue;

                    envia = true;
                    var empresa = new Empresa().ObterPorId(usuario.empresa_id);
                    var estab = new Estabelecimento().ObterPorIdMail(usuestab.estabelecimento_id);

                    tabela.AppendLine("Empresa:");
                    tabela.AppendLine(empresa.nome);
                    tabela.AppendLine("Estabelecimento/Filial:");
                    tabela.AppendLine(estab.NomeCompleto + " - CNPJ: " + estab.CNPJ);

                    tabela.AppendLine("<table>");
                    tabela.AppendLine(" <thead>");
                    tabela.AppendLine("     <tr>");
                    tabela.AppendLine("         <th width='20%'>Vencimento</th>");
                    tabela.AppendLine("         <th width='20%'>Valor</th>");
                    tabela.AppendLine("         <th width='20%'>Contrato</th>");
                    tabela.AppendLine("         <th width='20%'>Item</th>");
                    tabela.AppendLine("         <th width='20%'>Pessoa</th>");
                    tabela.AppendLine("     </tr>");
                    tabela.AppendLine(" </thead>");
                    tabela.AppendLine(" <tbody>");

                    foreach (var cpag in cpags)
                    {
                        tabela.AppendLine("     <tr>");
                        tabela.AppendLine("         <td>" + cpag.data.ToShortDateString() + "</td>");
                        tabela.AppendLine("         <td>" + cpag.valor.ToString("n") + "</td>");
                        tabela.AppendLine("         <td>" + cpag.ContratoItem.Contrato.descricao + "</td>");
                        tabela.AppendLine("         <td>" + cpag.ContratoItem.ItemProdutoServico.descricao + "</td>");
                        tabela.AppendLine("         <td>" + cpag.ContratoItem.Contrato.Pessoa.nome + "</td>");
                        tabela.AppendLine("     </tr>");
                    }

                    tabela.AppendLine(" </tbody>");
                    tabela.AppendLine("</table>");

                    tabela.AppendLine("<br><br><br><br>Atenciosamente<br>Equipe SoftFin<br><a href='mailto:adm@softfin.com.br' >adm@softfin.com.br</a><br>" +
                        ConfigurationManager.AppSettings["textocomplementaremail"].ToString());

                }

                if (envia)
                {
                    try
                    {
                        email.EnviarMensagem("ricardo.santos@softfin.com.br",
                            "Aviso de Parcelas de Contratos nos próximos dias",
                            tabela.ToString(), true);
                    }
                    catch
                    {
                    }
                }
            }


            return Json("OK-" + usuarios.Count().ToString(), JsonRequestBehavior.AllowGet);
        }

    }
}

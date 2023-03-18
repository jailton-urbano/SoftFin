using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SoftFin.API
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EmailController : BaseApi
    {
        // GET api/<controller>
        public void Get()
        {

            DateTime primeiroDia = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime ultimoDia = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));


            var establocal = 3;

            var estab = new Estabelecimento().ObterPorId(establocal, _paramBase);
            
            
            var cpags = new DocumentoPagarParcela().ObterEntreData(primeiroDia, ultimoDia, _paramBase, establocal).OrderBy(p => p.vencimentoPrevisto);

            string trpagar = "";
            decimal valor = 0;
            foreach (var item in cpags)
            {
                trpagar += "<tr>";
				trpagar += "    <td>";
                trpagar += item.vencimentoPrevisto.ToShortDateString();
				trpagar += "    </td>";
				trpagar += "   <td style='padding-left: 5px;'>";

                if (item.DocumentoPagarMestre.StatusPagamento == 1)
				    trpagar += "1 - Em Aberto";
                else if (item.DocumentoPagarMestre.StatusPagamento == 1)
                    trpagar += "2 - Pago Parcialmente";
                else
                    trpagar += "3 -Pago Integralmente";

				trpagar += "    </td>";
				trpagar += "    <td style='padding-left: 5px;'>";
                trpagar += item.DocumentoPagarMestre.numeroDocumento;
                trpagar += "    </td>";
				trpagar += "    <td style='padding-left: 5px;'>";
                trpagar += item.DocumentoPagarMestre.Pessoa.nome;
                trpagar += "    </td>";
                trpagar += "    <td style='text-align:right'>";
                trpagar += item.DocumentoPagarMestre.valorBruto.ToString("n");
				trpagar += "    </td>";
				trpagar += "</tr>";
                valor += item.DocumentoPagarMestre.valorBruto;
            }


            var recebes = new ParcelaContrato().ObterEntreData(primeiroDia, ultimoDia, _paramBase, establocal).OrderBy(p=>p.data);

            string trreceber = "";
            decimal valorreceber = 0;
            foreach (var item in recebes)
            {
                trreceber += "<tr>";
                trreceber += "    <td>";
                trreceber += item.data.ToShortDateString();
                trreceber += "    </td>";
                trreceber += "   <td style='padding-left: 5px;'>";

                trreceber += item.statusParcela.status;

                trreceber += "    </td>";
                trreceber += "    <td style='padding-left: 5px;'>";
                trreceber += item.descricao;
                trreceber += "    </td>";
                trreceber += "    <td style='padding-left: 5px;'>";
                trreceber += item.ContratoItem.Contrato.Pessoa.nome;
                trreceber += "    </td>";
                trreceber += "    <td style='text-align:right'>";
                trreceber += item.valor.ToString("n");
                trreceber += "    </td>";
                trreceber += "</tr>";
                valorreceber += item.valor;
            }





            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplate/");
            var arquivohmtl = Path.Combine(path, "Email01.html");
            string readText = File.ReadAllText(arquivohmtl);


            readText = readText.Replace("{DataInicial}", primeiroDia.ToString("dd/MM/yyyy"));
            readText = readText.Replace("{DataFinal}", ultimoDia.ToString("dd/MM/yyyy"));
            readText = readText.Replace("{trPagar}", trpagar);
            readText = readText.Replace("{trPagarTotal}", valor.ToString());
            readText = readText.Replace("{trReceber}", trreceber);
            readText = readText.Replace("{trReceberTotal}", valorreceber.ToString());
            readText = readText.Replace("{nomeestab}", estab.NomeCompleto);
            
            
            
            
            var email = new Email();
            email.EnviarMensagem("ricardo.santos@softfin.com.br", "Resumo Semanal", readText, true);
        }


    }
}
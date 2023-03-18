using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.IO;
using System.Text;
using SoftFin.Web.Models;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using Lib.Web.Mvc.JQuery.JqGrid;
using System.Globalization;


namespace SoftFin.Web.Controllers
{
    public class CnabController : BaseController
    {
        [HttpPost]
        public JsonResult ConsultaCNAB(string dataInicial, string dataFinal, int banco)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);


            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);


            var cpgs = new DocumentoPagarParcela().ObterTodosAutorizado(DataInicial, DataFinal, _paramBase).Where(p => p.DocumentoPagarMestre.banco_id == banco) ;
            List<DocumentoPagarMestreAutorizacao> rets = new List<DocumentoPagarMestreAutorizacao>();

            foreach (var item in cpgs)
            {
                rets.Add(new DocumentoPagarMestreAutorizacao()
                {
                    banco = item.DocumentoPagarMestre.Banco.nomeBanco,
                    dataCompetencia = item.DocumentoPagarMestre.dataCompetencia.ToString(),
                    dataLancamento = item.DocumentoPagarMestre.dataLancamento.ToShortDateString(),
                    dataVencimento = item.vencimentoPrevisto.ToShortDateString(),
                    id = item.id,
                    pessoa = item.DocumentoPagarMestre.Pessoa.nome,
                    tipodocumento = item.DocumentoPagarMestre.tipoDocumento.descricao,
                    valorBruto = item.valor.ToString("n"),
                    erro = PreValidacao(item.DocumentoPagarMestre)
                });
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(rets);

            return Json(rets, JsonRequestBehavior.AllowGet);
        }

        private string PreValidacao(DocumentoPagarMestre documentoPagarMestre)
        {
            if (documentoPagarMestre.tipoDocumento.descricao.Contains("Boleto"))
            {
                if (string.IsNullOrEmpty(documentoPagarMestre.LinhaDigitavel))
                    return "Linha Digitável não informado"; 
            }
            else
            {
                if (string.IsNullOrEmpty(documentoPagarMestre.Pessoa.bancoConta))
                {
                    return "Pessoa - Conta não informado";
                }
                if (string.IsNullOrEmpty(documentoPagarMestre.Pessoa.agenciaConta))
                {
                    return "Pessoa - Agência não informado";
                }
                if (string.IsNullOrEmpty(documentoPagarMestre.Pessoa.agenciaDigito))
                {
                    return "Pessoa - Digito Agência não informado";
                }
                if (string.IsNullOrEmpty(documentoPagarMestre.Pessoa.nome))
                {
                    return "Pessoa - nome não informado";
                }
                if (string.IsNullOrEmpty(documentoPagarMestre.Pessoa.cnpj))
                {
                    return "Pessoa - cnpj/cpf não informado";
                }
            }
            return "";

        }

        public ActionResult GeracaoArquivoCnab()
        {
            @ViewData["DataInicial"] = DateTime.Now.AddDays(-30).Date.ToString("dd/MM/yyyy");
            @ViewData["DataFinal"] = DateTime.Now.Date.ToString("dd/MM/yyyy");

            var con1 = new Banco().ObterTodos(_paramBase);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            ViewData["banco"] = listret;

            return View();
        }

        class variables
        {
            public static string tipoPagamento = "";
            public static int codigoLote = 0;
            //Arquivo
            public static int qtdLotesArquivo = 0;
            public static int qtdRegistrosArquivo = 0;
            //CC - Crédito em Conta
            public static int qtdRegistrosCC = 0;
            public static decimal valorTotalCC = 0;
            //DOC
            public static int qtdRegistrosDOC = 0;
            public static decimal valorTotalDOC = 0;
            //TED
            public static int qtdRegistrosTED = 0;
            public static decimal valorTotalTED = 0;
            //BOLETO ITAÚ
            public static int qtdRegistrosBOLETOITAU = 0;
            public static decimal valorTotalBOLETOITAU = 0;
            //BOLETO OUTROS
            public static int qtdRegistrosBOLETOOUTROS = 0;
            public static decimal valorTotalBOLETOOUTROS = 0;
            //ISS
            public static int qtdRegistrosISS = 0;
            public static decimal valorTotalISS = 0;
            //CONCESSIONÁRIA
            public static int qtdRegistrosCONCESSIONARIA = 0;
            public static decimal valorTotalCONCESSIONARIA = 0;
            //FGTS
            public static int qtdRegistrosGPS = 0;
            public static decimal valorTributoGPS = 0;
            public static decimal valorOutrasEntidadesGPS = 0;
            public static decimal valorAtualizacaoGPS = 0;
            public static decimal valorArrecadadoGPS = 0;
            //DARF
            public static int qtdRegistrosDARF = 0;
            public static decimal valorTotalDARF = 0;
            public static decimal valorPrincipalDARF = 0;
            public static decimal valorMultaJurosDARF = 0;
            //FGTS
            public static int qtdRegistrosFGTS = 0;
            public static decimal valorTotalFGTS = 0;
            public static decimal valorPagamentoFGTS = 0;
        }

        [HttpPost]
        public FileStreamResult GeracaoArquivoCnab(string dataInicial, string dataFinal, int banco)
        {
            //Zera variáveis
            variables.tipoPagamento = "";
            variables.codigoLote = 0;
            variables.qtdLotesArquivo = 0;
            variables.qtdRegistrosArquivo = 0;
            variables.qtdRegistrosCC = 0;
            variables.valorTotalCC = 0;
            variables.qtdRegistrosDOC = 0;
            variables.valorTotalDOC = 0;
            variables.qtdRegistrosTED = 0;
            variables.valorTotalTED = 0;
            variables.qtdRegistrosBOLETOITAU = 0;
            variables.valorTotalBOLETOITAU = 0;
            variables.qtdRegistrosBOLETOOUTROS = 0;
            variables.valorTotalBOLETOOUTROS = 0;
            variables.qtdRegistrosISS = 0;
            variables.valorTotalISS = 0;
            variables.qtdRegistrosCONCESSIONARIA = 0;
            variables.valorTotalCONCESSIONARIA = 0;
            variables.qtdRegistrosGPS = 0;
            variables.valorTributoGPS = 0;
            variables.valorOutrasEntidadesGPS = 0;
            variables.valorAtualizacaoGPS = 0;
            variables.valorArrecadadoGPS = 0;
            variables.qtdRegistrosDARF = 0;
            variables.valorTotalDARF = 0;
            variables.valorPrincipalDARF = 0;
            variables.valorMultaJurosDARF = 0;
            variables.qtdRegistrosFGTS = 0;
            variables.valorTotalFGTS = 0;
            variables.valorPagamentoFGTS = 0;

            var bc = new Banco().ObterPorId(banco,_paramBase);

            variables.codigoLote = bc.SequencialLoteCNAB + 1;
            


            StringBuilder sb = new StringBuilder();
            DateTime dtInicial = Convert.ToDateTime(dataInicial);
            DateTime dtFinal = Convert.ToDateTime(dataFinal);

            List<int> cpgs = new List<int>();
            foreach (var item in Request.Form)
            {
                if (item.ToString().Substring(0, 3) == "cpg")
                {
                    cpgs.Add(int.Parse(item.ToString().Substring(3)));
                }

            }
            //Header do Arquivo
            headerArquivo(sb, dataInicial, dataFinal);

            //Segmento A - Pagamentos através de cheque, OP, DOC, TED e crédito em conta corrente
            var pagar = new DocumentoPagarParcela();
            List<int> docs = new List<int>();
            List<int> teds = new List<int>();
            List<int> ccs = new List<int>();


            foreach (var itemCPG in cpgs)
            {
                var item = pagar.ObterPorId(itemCPG,_paramBase);


                if (item.DocumentoPagarMestre.tipoDocumento.codigo == "DOC")
                {
                    docs.Add(item.id);
                    variables.qtdRegistrosDOC += 1;
                    variables.valorTotalDOC += item.valor;
                }
                if (item.DocumentoPagarMestre.tipoDocumento.codigo == "TED")
                {
                    teds.Add(item.id);
                    variables.qtdRegistrosTED += 1;
                    variables.valorTotalTED += item.valor;
                }
                if (item.DocumentoPagarMestre.tipoDocumento.codigo == "CC")
                {
                    ccs.Add(item.id);
                    variables.qtdRegistrosCC += 1;
                    variables.valorTotalCC += item.valor;
                }



            }
            //Lote de DOC's
            if (docs.Count() > 0)
            {
                variables.tipoPagamento = "03";
                variables.codigoLote += 1;
                headerLoteA(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoA(sb, dataInicial, dataFinal, docs, variables.codigoLote);
                traillerLoteA(sb, variables.codigoLote, variables.qtdRegistrosDOC, variables.valorTotalDOC);
            }
            //Lote de TED's
            if (teds.Count() > 0)
            {
                variables.tipoPagamento = "41";
                variables.codigoLote += 1;
                headerLoteA(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoA(sb, dataInicial, dataFinal, teds, variables.codigoLote);
                traillerLoteA(sb, variables.codigoLote, variables.qtdRegistrosTED, variables.valorTotalTED);
            }
            //Lote de CC's
            if (ccs.Count() > 0)
            {
                variables.tipoPagamento = "01";
                variables.codigoLote += 1;
                headerLoteA(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoA(sb, dataInicial, dataFinal, ccs, variables.codigoLote);
                traillerLoteA(sb, variables.codigoLote, variables.qtdRegistrosCC, variables.valorTotalCC);
            }

            //Segmento J - Liquidação de títulos (bloquetos) em cobrança no Itaú e em outros Bancos
            List<int> boletosItau = new List<int>();
            List<int> boletosOutros = new List<int>();

            foreach (var itemCPG in cpgs)
            {
                foreach (var item in pagar.ObterTodos(_paramBase).ToList().Where(x => x.id == itemCPG && x.DocumentoPagarMestre.tipoDocumento.codigo == "BOLETO"))
                {
                    if (item.DocumentoPagarMestre.LinhaDigitavel != null)
                        if (item.DocumentoPagarMestre.LinhaDigitavel.Length > 0)
                        {
                            if (item.DocumentoPagarMestre.LinhaDigitavel.Substring(0, 3) == "341")
                            {
                                boletosItau.Add(item.id);
                                variables.qtdRegistrosBOLETOITAU += 1;
                                variables.valorTotalBOLETOITAU += item.valor;
                            }
                            else
                            {
                                boletosOutros.Add(item.id);
                                variables.qtdRegistrosBOLETOOUTROS += 1;
                                variables.valorTotalBOLETOOUTROS += item.valor;
                            }
                        }
                }
            }
            //Lote de BOLETOS ITAÚ
            if (boletosItau.Count() > 0)
            {
                //30 - PAGAMENTO DE TÍTULOS EM COBRANÇA NO ITAÚ
                variables.tipoPagamento = "30";
                variables.codigoLote += 1;
                headerLoteJ(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoJ(sb, dataInicial, dataFinal, boletosItau, variables.codigoLote);
                traillerLoteJ(sb, variables.codigoLote, variables.qtdRegistrosBOLETOITAU, variables.valorTotalBOLETOITAU);
            }

            //Lote de BOLETOS OUTROS
            if (boletosOutros.Count() > 0)
            {
                //31 - PAGAMENTO DE TÍTULOS EM COBRANÇA EM OUTROS BANCOS
                variables.tipoPagamento = "31";
                variables.codigoLote += 1;
                headerLoteJ(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoJ(sb, dataInicial, dataFinal, boletosOutros, variables.codigoLote);
                traillerLoteJ(sb, variables.codigoLote, variables.qtdRegistrosBOLETOOUTROS, variables.valorTotalBOLETOOUTROS);
            }

            //Segmento O - Pagamento de Contas de Concessionárias e Tributos com código de barras
            List<int> iss = new List<int>();
            List<int> concessionaria = new List<int>();

            foreach (var itemCPG in cpgs)
            {

                foreach (var item in pagar.ObterTodos(_paramBase).ToList().Where(x => x.id == itemCPG &&
                    (x.DocumentoPagarMestre.tipoDocumento.codigo == "ISS" ||
                     x.DocumentoPagarMestre.tipoDocumento.codigo == "CONCESSIONARIA")))
                {
                    if (item.DocumentoPagarMestre.tipoDocumento.codigo == "ISS")
                    {
                        iss.Add(item.id);
                        variables.qtdRegistrosISS += 1;
                        variables.valorTotalISS += item.valor;
                    }
                    if (item.DocumentoPagarMestre.tipoDocumento.codigo == "CONCESSIONARIA")
                    {
                        concessionaria.Add(item.id);
                        variables.qtdRegistrosCONCESSIONARIA += 1;
                        variables.valorTotalCONCESSIONARIA += item.valor;
                    }

                }
            }
            //Lote de iss
            if (iss.Count() > 0)
            {
                variables.tipoPagamento = "19";
                variables.codigoLote += 1;
                headerLoteO(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoO(sb, dataInicial, dataFinal, iss, variables.codigoLote);
                traillerLoteO(sb, variables.codigoLote, variables.qtdRegistrosCC, variables.valorTotalCC);
            }
            //Lote de pagamentos concessionárias
            if (concessionaria.Count() > 0)
            {
                variables.tipoPagamento = "13";
                variables.codigoLote += 1;
                headerLoteO(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoO(sb, dataInicial, dataFinal, concessionaria, variables.codigoLote);
                traillerLoteO(sb, variables.codigoLote, variables.qtdRegistrosCC, variables.valorTotalCC);
            }

            //headerLoteO(sb,dataInicial,dataFinal);
            //segmentoO(sb,numDoc);
            //traillerLoteO(sb);

            //Segmento N - Pagamento de Tributos sem código de barras e FGTS-GRF/GRRF/GRDE com código de barras
            GPS gps = new GPS();
            List<int> _gps = new List<int>();
            DARF darf = new DARF();
            List<int> _darf = new List<int>();
            FGTS fgts = new FGTS();
            List<int> _fgts = new List<int>();

            //Carrega GPS's
            foreach (var itemCPG in cpgs)
            {
                foreach (var item in gps.ObterTodos(_paramBase).ToList().Where(x => x.DocumentoPagarMestre_id == itemCPG))
                {
                    _gps.Add(item.id);
                    variables.qtdRegistrosGPS += 1;
                    variables.valorTributoGPS += item.valorTributo;
                    variables.valorOutrasEntidadesGPS += item.valorOutrasEntidades;
                    variables.valorAtualizacaoGPS += item.valorAtualizacaoMonetaria;
                    variables.valorArrecadadoGPS += item.valorArrecadado;
                }
                //Carrega DARF's
                foreach (var item in darf.ObterTodos(_paramBase).ToList().Where(x => x.DocumentoPagarMestre_id == itemCPG))
                {
                    _darf.Add(item.ID);
                    variables.qtdRegistrosDARF += 1;
                    variables.valorTotalDARF += item.valorTotal;
                    variables.valorPrincipalDARF += item.valorPrincipal;
                    variables.valorMultaJurosDARF += (item.multa + item.jurosEncargos);
                }
                //Carrega FGTS's
                foreach (var item in fgts.ObterTodos(_paramBase).ToList().Where(x => x.DocumentoPagarMestre_id == itemCPG))
                {
                    _fgts.Add(item.ID);
                    variables.qtdRegistrosFGTS += 1;
                    variables.valorPagamentoFGTS += item.valorPagamento;
                    variables.valorTotalFGTS += item.valorPagamento;
                }
            }

            //Lote de GPS's
            if (_gps.Count() > 0)
            {
                variables.tipoPagamento = "17";
                variables.codigoLote += 1;
                headerLoteN(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoN(sb, dataInicial, dataFinal, _gps, variables.codigoLote, variables.tipoPagamento);
                traillerLoteN(sb, variables.codigoLote, variables.qtdRegistrosGPS, variables.valorArrecadadoGPS, variables.tipoPagamento);
            }
            //Lote de DARF's
            if (_darf.Count() > 0)
            {
                variables.tipoPagamento = "16";
                variables.codigoLote += 1;
                headerLoteN(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoN(sb, dataInicial, dataFinal, _darf, variables.codigoLote, variables.tipoPagamento);
                traillerLoteN(sb, variables.codigoLote, variables.qtdRegistrosDARF, variables.valorTotalDARF, variables.tipoPagamento);
            }
            //Lote de FGTS's
            if (_fgts.Count() > 0)
            {
                variables.tipoPagamento = "35";
                variables.codigoLote += 1;
                headerLoteN(sb, dataInicial, dataFinal, variables.codigoLote, variables.tipoPagamento);
                segmentoN(sb, dataInicial, dataFinal, _fgts, variables.codigoLote, variables.tipoPagamento);
                traillerLoteN(sb, variables.codigoLote, variables.qtdRegistrosFGTS, variables.valorTotalFGTS, variables.tipoPagamento);
            }
            //Fim do Segmento N - Pagamento de Tributos sem código de barras e FGTS-GRF/GRRF/GRDE com código de barras

            //Trailler Arquivo
            variables.qtdRegistrosArquivo = (variables.qtdLotesArquivo * 2) + variables.qtdRegistrosArquivo + 2;
            traillerArquivo(sb, variables.qtdLotesArquivo, variables.qtdRegistrosArquivo);

            var db = new DbControle();
            bc = new Banco().ObterPorId(banco, db, _paramBase);
            bc.SequencialLoteCNAB = variables.codigoLote;
            bc.Alterar(bc, _paramBase,db);

            var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "CNAB" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".txt");
        }

        //Header do Arquivo
        private void headerArquivo(StringBuilder sb, string DataInicia, string DataFinal)
        {
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id,_paramBase);
            var banco = new Banco().ObterTodos(_paramBase).Where(x => x.principal == true && x.estabelecimento_id == _paramBase.estab_id).FirstOrDefault();
            sb.Append("341"); //CÓDIGO DO BANCO
            sb.Append("0000"); //CÓDIGO DO LOTE
            sb.Append("0"); //TIPO DE REGISTRO
            sb.Append(' ', 6); //BRANCOS
            sb.Append("081"); //LAYOUT DE ARQUIVO
            sb.Append("2"); //EMPRESA – INSCRIÇÃO
            sb.AppendFormat("{0,-14}", prestador.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "")); //INSCRIÇÃO NÚMERO
            sb.Append(' ', 20); //BRANCOS
            sb.AppendFormat("{0,-5}", banco.agencia.PadLeft(5, '0')); //AGÊNCIA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-12}", banco.contaCorrente.PadLeft(12, '0')); //CONTA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-1}", banco.contaCorrenteDigito); //DAC

            if (prestador.NomeCompleto.Length > 30)
            {
                sb.AppendFormat("{0,-30}", prestador.NomeCompleto.Substring(0,30)); //NOME DA EMPRESA
            }
            else
            {
                sb.AppendFormat("{0,-30}", prestador.NomeCompleto); //NOME DA EMPRESA
            }

            sb.AppendFormat("{0,-30}", banco.nomeBanco); //NOME DO BANCO
            sb.Append(' ', 10); //BRANCOS
            sb.Append("1"); //ARQUIVO-CÓDIGO
            sb.Append(DateTime.Now.ToString("ddMMyyyy")); //DATA DE GERAÇÃO
            sb.Append(DateTime.Now.ToString("hhmmss")); //HORA DA GERAÇÃO
            sb.Append("000000000"); //ZEROS
            sb.Append("00000"); //UNIDADE DE DENSIDADE
            sb.Append(' ', 69); //BRANCOS
            sb.AppendLine();
        }

        //Segmento A Header do Lote
        private void headerLoteA(StringBuilder sb, string DataInicia, string DataFinal, int codigoLote, string tipoPagamento)
        {
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);
            var banco = new Banco().ObterTodos(_paramBase).Where(x => x.principal == true && x.estabelecimento_id == _paramBase.estab_id).FirstOrDefault();
            var municipio = new Municipio().ObterPorId(prestador.Municipio_id);
            variables.qtdLotesArquivo += 1;

            sb.Append("341"); //CÓDIGO DO BANCO
            sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
            sb.Append("1"); //TIPO DE REGISTRO
            sb.Append("C"); //TIPO DE OPERAÇÃO
            sb.Append("20"); //TIPO DE PAGAMENTO
            sb.Append(tipoPagamento); //FORMA DE PAGAMENTO
            sb.Append("040"); //LAYOUT DO LOTE
            sb.Append(' '); //BRANCOS
            sb.Append("2"); //EMPRESA – TIPO NSCRIÇÃO
            sb.AppendFormat("{0,-14}", prestador.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "")); //INSCRIÇÃO NÚMERO
            sb.Append(' ', 4); //IDENTIFICAÇÃO DO LANÇAMENTO
            sb.Append(' ', 16); //BRANCOS
            sb.AppendFormat("{0,-5}", banco.agencia.PadLeft(5, '0')); //AGÊNCIA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-12}", banco.contaCorrente.PadLeft(12, '0')); //CONTA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-1}", banco.contaCorrenteDigito); //DAC
            sb.AppendFormat("{0,-30}", prestador.NomeCompleto); //NOME DA EMPRESA
            sb.Append(' ', 30); //FINALIDADE DO LOTE
            sb.Append(' ', 10); //HISTÓRICO DE C/C
            sb.AppendFormat("{0,-30}", prestador.Logradouro); //ENDEREÇO DA EMPRESA
            sb.AppendFormat("{0,-5}", prestador.NumeroLogradouro.ToString().PadLeft(5, '0')); //NÚMERO
            sb.AppendFormat("{0,-15}", prestador.Complemento); //COMPLEMENTO.
            sb.AppendFormat("{0,-20}", municipio.DESC_MUNICIPIO); //CIDADE
            sb.AppendFormat("{0,-8}", prestador.CEP.Replace("-", "").PadLeft(8, '0')); //CEP
            sb.AppendFormat("{0,-2}", prestador.UF); //ESTADO
            sb.Append(' ', 8); //BRANCOS
            sb.Append(' ', 10); //OCORRÊNCIAS
            sb.AppendLine();
        }
        //Segmento A Detalhe - Pagamentos através de cheque, OP, DOC, TED e crédito em conta corrente
        private void segmentoA(StringBuilder sb, string DataInicia, string DataFinal, List<int> docid, int codigoLote)
        {
            int estab = _paramBase.estab_id;
            var db = new DbControle();
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);

            List<DocumentoPagarParcela> listaAux = new List<DocumentoPagarParcela>();
            foreach (var item in docid)
            {
                var pag = db.DocumentoPagarParcela.Where(doc => doc.id == item && doc.DocumentoPagarMestre.estabelecimento_id == estab).FirstOrDefault();
                if (pag != null)
                    listaAux.Add(pag);
            }


            foreach (var item in listaAux)
            {
                variables.qtdRegistrosArquivo += 1;

                sb.Append("341"); //CÓDIGO DO BANCO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
                sb.Append("3"); //TIPO DE REGISTRO
                sb.Append("00001"); //NÚMERO DO REGISTRO
                sb.Append("A"); //SEGMENTO
                sb.Append("000"); //TIPO DE MOVIMENTO
                sb.Append("000"); //ZEROS
                sb.AppendFormat("{0,-3}", item.DocumentoPagarMestre.Pessoa.bancoConta); //BANCO FAVORECIDO
                //Início do Detalhamento do Campo Agência-Conta POS 24
                sb.Append("0"); //ZERO
                item.DocumentoPagarMestre.Pessoa.agenciaConta = item.DocumentoPagarMestre.Pessoa.agenciaConta.Trim();
                sb.AppendFormat("{0,-4}", item.DocumentoPagarMestre.Pessoa.agenciaConta.PadLeft(4, '0')); //AGÊNCIA FAVORECIDO
                sb.Append(' '); //BRANCOS
                sb.Append("000000"); //ZEROS
                item.DocumentoPagarMestre.Pessoa.contaBancaria = item.DocumentoPagarMestre.Pessoa.contaBancaria.Trim();
                sb.AppendFormat("{0,-6}", item.DocumentoPagarMestre.Pessoa.contaBancaria.PadLeft(6, '0')); //CONTA FAVORECIDO
                sb.Append(' '); //BRANCO
                sb.AppendFormat("{0,-1}", item.DocumentoPagarMestre.Pessoa.digitoContaBancaria); //DAC CONTA FAVORECIDO
                // Fim do Campo Agência Conta
                sb.AppendFormat("{0,-30}", item.DocumentoPagarMestre.Pessoa.nome); //NOME DO FAVORECIDO
                sb.AppendFormat("{0,-20}", item.DocumentoPagarMestre.numeroDocumento); //SEU NÚMERO
                sb.Append(item.vencimentoPrevisto.ToString("ddMMyyyy")); //DATA DE PAGTO
                sb.Append("REA"); //MOEDA - TIPO
                sb.Append("000000000000000"); //ZEROS
                sb.AppendFormat("{0,-15}", item.valor.ToString("0.00").Replace(",", "").PadLeft(15, '0')); //VALOR DO PAGTO
                sb.Append(' ', 15); //NOSSO NÚMERO
                sb.Append(' ', 5); //BRANCOS
                sb.Append("00000000"); //DATA EFETIVA
                sb.Append("000000000000000"); //VALOR EFETIVO
                sb.Append(' ', 18); //FINALIDADE DETALHE
                sb.Append(' ', 2); //BRANCOS
                sb.Append("000000"); //N  DO DOCUMENTO
                sb.AppendFormat("{0,-14}", item.DocumentoPagarMestre.Pessoa.cnpj.Replace(".", "").Replace("-", "").Replace("/", "").PadLeft(14, '0')); //N  DE INSCRIÇÃO
                sb.Append("21"); //FINALIDADE DOC E STATUS FUNCIONÁRIO
                sb.Append("00010"); //FINALIDADE TED
                sb.Append(' ', 5); //BRANCOS
                sb.Append("0"); //AVISO
                sb.Append(' ', 10); //OCORRÊNCIAS
                sb.AppendLine();
            }
        }
        //Segmento A Trailler do Lote
        private static void traillerLoteA(StringBuilder sb, int codigoLote, int qtdRegistros, decimal valorTotal)
        {
            qtdRegistros += 2;
            sb.Append("341"); //CÓDIGO DO BANCO
            sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
            sb.Append("5"); //TIPO DE REGISTRO
            sb.Append(' ', 9); //BRANCOS
            sb.AppendFormat("{0,-6}", qtdRegistros.ToString().PadLeft(6, '0')); //TOTAL QTDE REGISTROS
            sb.AppendFormat("{0,-18}", valorTotal.ToString("0.00").Replace(",", "").PadLeft(18, '0')); //TOTAL VALOR PAGTOS
            sb.Append("000000000000000000"); //ZEROS
            sb.Append(' ', 171); //BRANCOS
            sb.Append(' ', 10); //OCORRÊNCIAS
            sb.AppendLine();
        }

        //Segmento J Header do Lote
        private void headerLoteJ(StringBuilder sb, string DataInicia, string DataFinal, int codigoLote, string tipoPagamento)
        {
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);
            var banco = new Banco().ObterTodos(_paramBase).Where(x => x.principal == true && x.estabelecimento_id == _paramBase.estab_id).FirstOrDefault();
            var municipio = new Municipio().ObterPorId(prestador.Municipio_id);
            variables.qtdLotesArquivo += 1;

            sb.Append("341"); //CÓDIGO DO BANCO
            sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
            sb.Append("1"); //TIPO DE REGISTRO
            sb.Append("C"); //TIPO DE OPERAÇÃO
            sb.Append("20"); //TIPO DE PAGAMENTO
            sb.Append(tipoPagamento); //FORMA DE PAGAMENTO - *** Precisa ser dinâmico de acordo com a Nota 5!!!!!
            sb.Append("030"); //LAYOUT DO LOTE
            sb.Append(' '); //BRANCOS
            sb.Append("2"); //EMPRESA – TIPO NSCRIÇÃO
            sb.AppendFormat("{0,-14}", prestador.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "")); //INSCRIÇÃO NÚMERO
            sb.Append(' ', 4); //IDENTIFICAÇÃO DO LANÇAMENTO
            sb.Append(' ', 16); //BRANCOS
            sb.AppendFormat("{0,-5}", banco.agencia.PadLeft(5, '0')); //AGÊNCIA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-12}", banco.contaCorrente.PadLeft(12, '0')); //CONTA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-1}", banco.contaCorrenteDigito); //DAC
            if (prestador.NomeCompleto.Length > 30)
            {
                sb.AppendFormat("{0,-30}", prestador.NomeCompleto.Substring(0,30)); //NOME DA EMPRESA
            }
            else
            {
                sb.AppendFormat("{0,-30}", prestador.NomeCompleto); //NOME DA EMPRESA
            }
            sb.Append(' ', 30); //FINALIDADE DO LOTE
            sb.Append(' ', 10); //HISTÓRICO DE C/C
            sb.AppendFormat("{0,-30}", prestador.Logradouro); //ENDEREÇO DA EMPRESA
            sb.AppendFormat("{0,-5}", prestador.NumeroLogradouro.ToString().PadLeft(5, '0')); //NÚMERO
            sb.AppendFormat("{0,-15}", prestador.Complemento); //COMPLEMENTO.
            sb.AppendFormat("{0,-20}", municipio.DESC_MUNICIPIO); //CIDADE
            sb.AppendFormat("{0,-8}", prestador.CEP.Replace("-", "").PadLeft(8, '0')); //CEP
            sb.AppendFormat("{0,-2}", prestador.UF); //ESTADO
            sb.Append(' ', 8); //BRANCOS
            sb.Append(' ', 10); //OCORRÊNCIAS
            sb.AppendLine();
        }
        //Segmento J Detalhe - Liquidação de títulos (bloquetos) em cobrança no Itaú e em outros Bancos



        private void segmentoJ(StringBuilder sb, string DataInicia, string DataFinal, List<int> docid, int codigoLote)
        {
            int estab = _paramBase.estab_id;
            var db = new DbControle();
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);

            List<DocumentoPagarParcela> listaAux = new List<DocumentoPagarParcela>();
            foreach (var item in docid)
            {
                var pag = db.DocumentoPagarParcela.Where(doc => doc.id == item && doc.DocumentoPagarMestre.estabelecimento_id == estab).FirstOrDefault();
                if (pag != null)
                    listaAux.Add(pag);
            }


            foreach (var item in listaAux)
            {
                variables.qtdRegistrosArquivo += 1;

                //Retira formatação da Linha digitável do Código de Barras
                string varCodigoBarras = item.DocumentoPagarMestre.LinhaDigitavel.Replace(".", "").Replace(" ", "");
                //Monta código de barras com tamanho 44 para Cnab
                string var2CodigoBarras = varCodigoBarras.Substring(0, 3) + //Código do Banco favorecido (0, 3)
                                          varCodigoBarras.Substring(3, 1) + //Código da Moeda (3, 1)
                                          varCodigoBarras.Substring(32, 1) + //DV do Código de Barras (32, 1)
                                          varCodigoBarras.Substring(33, 4) + //Fator de Vencimento - ex.: 01/05/2002 (33, 4) - Data Base Febraban 
                                          varCodigoBarras.Substring(37, 10) +  //Valor do Título (37, 10)
                                          varCodigoBarras.Substring(4, 5) + //Campo Livre - Parte I (4, 5)
                                          varCodigoBarras.Substring(10, 10) + //Campo Livre - Parte II (10, 10)
                                          varCodigoBarras.Substring(21, 10); //Campo Livre - Parte III (21, 10)

                sb.Append("341"); //CÓDIGO BANCO NA COMPENSAÇÃO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //LOTE DE SERVIÇO
                sb.Append("3"); //REGISTRO DETALHE DE LOTE
                sb.Append("00001"); //Nº SEQUENCIAL REGISTRO NO LOTE
                sb.Append("J"); //CÓDIGO SEGMENTO REG. DETALHE
                sb.Append("000"); //TIPO DE MOVIMENTO
                sb.AppendFormat("{0,-44}", var2CodigoBarras); //CÓD. DE BARRAS
                if (item.DocumentoPagarMestre.Pessoa.razao.Length > 30)
                {
                    sb.AppendFormat("{0,-30}", item.DocumentoPagarMestre.Pessoa.razao.Substring(0, 30)); //NOME DO FAVORECIDO
                }
                else
                {
                    sb.AppendFormat("{0,-30}", item.DocumentoPagarMestre.Pessoa.razao); //NOME DO FAVORECIDO
                }
                sb.Append(item.vencimentoPrevisto.ToString("ddMMyyyy")); //DATA DO VENCIMENTO (NOMINAL)
                sb.AppendFormat("{0,-15}", item.valor.ToString("0.00").Replace(",", "").PadLeft(15, '0')); //VALOR DO TÍTULO (NOMINAL)
                sb.Append('0', 15); //VALOR DO DESCONTO + ABATIMENTO
                sb.Append('0', 15); //VALOR DA MORA + MULTA
                sb.Append(item.vencimentoPrevisto.ToString("ddMMyyyy")); //DATA DO PAGAMENTO
                sb.AppendFormat("{0,-15}", item.valor.ToString("0.00").Replace(",", "").PadLeft(15, '0')); //VALOR DO PAGAMENTO
                sb.Append('0', 15); //COMPLEMENTO DE REGISTRO
                sb.AppendFormat("{0,-20}", item.DocumentoPagarMestre.numeroDocumento); //Nº DOCTO ATRIBUÍDO PELA EMPRESA
                sb.Append(' ', 13); //COMPLEMENTO DE REGISTRO
                sb.Append(' ', 15); //NÚMERO ATRIBUÍDO PELO BANCO
                sb.Append(' ', 10); //CÓDIGO DE OCORRÊNCIAS P/ RETORNO
                sb.AppendLine();
                SegmentoJ52(sb, codigoLote, item.DocumentoPagarMestre, prestador, ref variables.qtdRegistrosArquivo);
            }
        }

        private void SegmentoJ52(StringBuilder sb, int codigoLote, DocumentoPagarMestre documentoPagarMestre, Estabelecimento prestador, ref int qtdRegistros)
        {
            qtdRegistros += 1;

            sb.Append("341"); //CÓDIGO DO BANCO
            sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
            sb.Append("3"); //TIPO DE REGISTRO
            sb.Append("00001"); //NÚMERO DO REGISTRO
            sb.Append("J"); //SEGMENTO 
            sb.Append("000"); //TIPO DE MOVIMENTO
            sb.Append("52"); //TIPO DE MOVIMENTO

            prestador.CNPJ = prestador.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "");

            //PAGAMENTO
            if (prestador.CNPJ.Length == 14)
            {
                sb.Append("20");
                sb.Append(prestador.CNPJ);
            }
            else
            {
                sb.Append("10000");
                sb.Append(prestador.CNPJ);
            }
            if (prestador.NomeCompleto.Length > 40)
            {
                sb.AppendFormat("{0,-40}", prestador.NomeCompleto.Substring(0, 40)); //NOME DO FAVORECIDO
            }
            else
            {
                sb.AppendFormat("{0,-40}", prestador.NomeCompleto); //NOME DO FAVORECIDO
            }
            //PAGAMENTO

            //BENEFICIARIO
            documentoPagarMestre.Pessoa.cnpj = documentoPagarMestre.Pessoa.cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (documentoPagarMestre.Pessoa.cnpj.Length == 14)
            {
                sb.Append("20");
                sb.Append(documentoPagarMestre.Pessoa.cnpj.Replace(".", "").Replace("-", ""));
            }
            else
            {
                sb.Append("10000");
                sb.Append(documentoPagarMestre.Pessoa.cnpj.Replace(".", "").Replace("-", ""));
            }
            if (documentoPagarMestre.Pessoa.razao.Length > 40)
            {
                sb.AppendFormat("{0,-40}", documentoPagarMestre.Pessoa.razao.Substring(0, 40)); //NOME DO FAVORECIDO
            }
            else
            {
                sb.AppendFormat("{0,-40}", documentoPagarMestre.Pessoa.razao); //NOME DO FAVORECIDO
            }
            //BENEFICIARIO

            //SACADO
            if (prestador.CNPJ.Length == 14)
            {
                sb.Append("20");
                sb.Append(prestador.CNPJ.Replace(".", "").Replace("-", ""));
            }
            else
            {
                sb.Append("10000");
                sb.Append(prestador.CNPJ.Replace(".", "").Replace("-", ""));
            }
            if (prestador.NomeCompleto.Length > 40)
            {
                sb.AppendFormat("{0,-40}", prestador.NomeCompleto.Substring(0, 40)); //NOME DO FAVORECIDO
            }
            else
            {
                sb.AppendFormat("{0,-40}", prestador.NomeCompleto); //NOME DO FAVORECIDO
            }
            //SACADO

            sb.Append(' ', 53); //BRANCOS
            sb.AppendLine();

        }

        //Segmento J Trailler do Lote
        private static void traillerLoteJ(StringBuilder sb, int codigoLote, int qtdRegistros, decimal valorTotal)
        {
            qtdRegistros += 2;
            sb.Append("341"); //CÓDIGO DO BANCO
            sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
            sb.Append("5"); //TIPO DE REGISTRO
            sb.Append(' ', 9); //BRANCOS
            sb.AppendFormat("{0,-6}", qtdRegistros.ToString().PadLeft(6, '0')); //TOTAL QTDE REGISTROS
            sb.AppendFormat("{0,-18}", valorTotal.ToString("0.00").Replace(",", "").PadLeft(18, '0')); //TOTAL VALOR PAGTOS
            sb.Append("000000000000000000"); //ZEROS
            sb.Append(' ', 171); //BRANCOS
            sb.Append(' ', 10); //OCORRÊNCIAS
            sb.AppendLine();
        }

        //Segmento O Header do Lote
        private void headerLoteO(StringBuilder sb, string DataInicia, string DataFinal, int codigoLote, string tipoPagamento)
        {
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);
            var banco = new Banco().ObterTodos(_paramBase).Where(x => x.principal == true && x.estabelecimento_id == _paramBase.estab_id).FirstOrDefault();
            var municipio = new Municipio().ObterPorId(prestador.Municipio_id);
            variables.qtdLotesArquivo += 1;

            sb.Append("341"); //CÓDIGO DO BANCO
            sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
            sb.Append("1"); //TIPO DE REGISTRO
            sb.Append("C"); //TIPO DE OPERAÇÃO
            sb.Append("20"); //TIPO DE PAGAMENTO
            sb.Append(tipoPagamento); //FORMA DE PAGAMENTO
            sb.Append("040"); //LAYOUT DO LOTE
            sb.Append(' '); //BRANCOS
            sb.Append("2"); //EMPRESA – TIPO NSCRIÇÃO
            sb.AppendFormat("{0,-14}", prestador.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "")); //INSCRIÇÃO NÚMERO
            sb.Append(' ', 4); //IDENTIFICAÇÃO DO LANÇAMENTO
            sb.Append(' ', 16); //BRANCOS
            sb.AppendFormat("{0,-5}", banco.agencia.PadLeft(5, '0')); //AGÊNCIA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-12}", banco.contaCorrente.PadLeft(12, '0')); //CONTA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-1}", banco.contaCorrenteDigito); //DAC
            sb.AppendFormat("{0,-30}", prestador.NomeCompleto); //NOME DA EMPRESA
            sb.Append(' ', 30); //FINALIDADE DO LOTE
            sb.Append(' ', 10); //HISTÓRICO DE C/C
            sb.AppendFormat("{0,-30}", prestador.Logradouro); //ENDEREÇO DA EMPRESA
            sb.AppendFormat("{0,-5}", prestador.NumeroLogradouro.ToString().PadLeft(5, '0')); //NÚMERO
            sb.AppendFormat("{0,-15}", prestador.Complemento); //COMPLEMENTO.
            sb.AppendFormat("{0,-20}", municipio.DESC_MUNICIPIO); //CIDADE
            sb.AppendFormat("{0,-8}", prestador.CEP.Replace("-", "").PadLeft(8, '0')); //CEP
            sb.AppendFormat("{0,-2}", prestador.UF); //ESTADO
            sb.Append(' ', 8); //BRANCOS
            sb.Append(' ', 10); //OCORRÊNCIAS
            sb.AppendLine();
        }
        //Segmento O Detalhe - Pagamento de Contas de Concessionárias e Tributos com código de barras
        private void segmentoO(StringBuilder sb, string DataInicia, string DataFinal, List<int> docid, int codigoLote)
        {
            int estab = _paramBase.estab_id;
            var db = new DbControle();
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);

            List<DocumentoPagarParcela> listaAux = new List<DocumentoPagarParcela>();
            foreach (var item in docid)
            {
                var pag = db.DocumentoPagarParcela.Where(doc => doc.id == item && doc.DocumentoPagarMestre.estabelecimento_id == estab).FirstOrDefault();
                if (pag != null)
                    listaAux.Add(pag);
            }


            foreach (var item in listaAux)
            {
                variables.qtdRegistrosArquivo += 1;

                //Retira formatação da Linha digitável do Código de Barras
                string varCodigoBarras = item.DocumentoPagarMestre.LinhaDigitavel.Replace(".", "").Replace(" ", "");

                sb.Append("341"); //CÓDIGO BANCO NA COMPENSAÇÃO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //LOTE DE SERVIÇO
                sb.Append("3"); //REGISTRO DETALHE DE LOTE
                sb.Append("00001"); //Nº SEQUENCIAL REGISTRO NO LOTE
                sb.Append("O"); //CÓDIGO SEGMENTO REG. DETALHE
                sb.Append("000"); //TIPO DE MOVIMENTO
                sb.AppendFormat("{0,-48}", varCodigoBarras); //CÓD. DE BARRAS
                sb.AppendFormat("{0,-30}", item.DocumentoPagarMestre.Pessoa.razao); //NOME DO FAVORECIDO
                sb.Append(item.vencimentoPrevisto.ToString("ddMMyyyy")); //DATA DO VENCIMENTO (NOMINAL)
                sb.Append("REA"); //TIPO DE MOEDA
                sb.Append('0', 15); //QTD MOEDA
                sb.AppendFormat("{0,-15}", item.valor.ToString("0.00").Replace(",", "").PadLeft(15, '0')); //VALOR DO TÍTULO (NOMINAL)
                sb.Append('0', 8); //DATA DO PAGAMENTO
                sb.Append('0', 15); //VALOR DO PAGAMENTO
                sb.Append(' ', 3); //COMPLEMENTO DE REGISTRO
                sb.AppendFormat("{0,-30}", item.DocumentoPagarMestre.numeroDocumento); //NÚMERO DA NOTA FISCAL
                sb.Append(' ', 3); //COMPLEMENTO DE REGISTRO
                sb.Append(' ', 20); //Nº DOCTO ATRIBUÍDO PELA EMPRESA
                sb.Append(' ', 21); //COMPLEMENTO DE REGISTRO
                sb.Append(' ', 15); //NÚMERO ATRIBUÍDO PELO BANCO
                sb.Append(' ', 10); //CÓDIGO DE OCORRÊNCIAS P/ RETORNO
                sb.AppendLine();
            }
        }
        //Segmento O Trailler do Lote
        private static void traillerLoteO(StringBuilder sb, int codigoLote, int qtdRegistros, decimal valorTotal)
        {
            qtdRegistros += 2;
            sb.Append("341"); //CÓDIGO DO BANCO
            sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
            sb.Append("5"); //TIPO DE REGISTRO
            sb.Append(' ', 9); //BRANCOS
            sb.AppendFormat("{0,-6}", qtdRegistros.ToString().PadLeft(6, '0')); //TOTAL QTDE REGISTROS
            sb.AppendFormat("{0,-18}", valorTotal.ToString("0.00").Replace(",", "").PadLeft(18, '0')); //TOTAL VALOR PAGTOS
            sb.Append("000000000000000000"); //ZEROS
            sb.Append(' ', 171); //BRANCOS
            sb.Append(' ', 10); //OCORRÊNCIAS
            sb.AppendLine();
        }

        //Segmento N Header do Lote
        private void headerLoteN(StringBuilder sb, string DataInicia, string DataFinal, int codigoLote, string tipoPagamento)
        {
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);
            var banco = new Banco().ObterTodos(_paramBase).Where(x => x.principal == true || x.estabelecimento_id == _paramBase.estab_id).FirstOrDefault();
            var municipio = new Municipio().ObterPorId(prestador.Municipio_id);
            variables.qtdLotesArquivo += 1;

            sb.Append("341"); //CÓDIGO DO BANCO
            sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
            sb.Append("1"); //TIPO DE REGISTRO
            sb.Append("C"); //TIPO DE OPERAÇÃO
            sb.Append("22"); //TIPO DE PAGAMENTO
            sb.Append(tipoPagamento); //FORMA DE PAGAMENTO
            sb.Append("040"); //LAYOUT DO LOTE
            sb.Append(' '); //BRANCOS
            sb.Append("2"); //EMPRESA – TIPO NSCRIÇÃO
            sb.AppendFormat("{0,-14}", prestador.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "")); //INSCRIÇÃO NÚMERO
            sb.Append(' ', 4); //IDENTIFICAÇÃO DO LANÇAMENTO
            sb.Append(' ', 16); //BRANCOS
            sb.AppendFormat("{0,-5}", banco.agencia.PadLeft(5, '0')); //AGÊNCIA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-12}", banco.contaCorrente.PadLeft(12, '0')); //CONTA
            sb.Append(' '); //BRANCOS
            sb.AppendFormat("{0,-1}", banco.contaCorrenteDigito); //DAC
            sb.AppendFormat("{0,-30}", prestador.NomeCompleto); //NOME DA EMPRESA
            sb.Append(' ', 30); //FINALIDADE DO LOTE
            sb.Append(' ', 10); //HISTÓRICO DE C/C
            sb.AppendFormat("{0,-30}", prestador.Logradouro); //ENDEREÇO DA EMPRESA
            sb.AppendFormat("{0,-5}", prestador.NumeroLogradouro.ToString().PadLeft(5, '0')); //NÚMERO
            sb.AppendFormat("{0,-15}", prestador.Complemento); //COMPLEMENTO.
            sb.AppendFormat("{0,-20}", municipio.DESC_MUNICIPIO); //CIDADE
            sb.AppendFormat("{0,-8}", prestador.CEP.Replace("-", "").PadLeft(8, '0')); //CEP
            sb.AppendFormat("{0,-2}", prestador.UF); //ESTADO
            sb.Append(' ', 8); //BRANCOS
            sb.Append(' ', 10); //OCORRÊNCIAS
            sb.AppendLine();
        }
        //Segmento N Detalhe - Pagamento de Tributos sem código de barras e FGTS-GRF/GRRF/GRDE com código de barras
        private void segmentoN(StringBuilder sb, string DataInicia, string DataFinal, List<int> docid, int codigoLote, string tipoPagamento)
        {
            int estab = _paramBase.estab_id;
            var db = new DbControle();
            var prestador = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);

            List<GPS> listaGPS = new List<GPS>();
            List<DARF> listaDARF = new List<DARF>();
            List<FGTS> listaFGTS = new List<FGTS>();

            if (tipoPagamento == "17") //GPS
            {
                foreach (var item in docid)
                {
                    var gps = db.GPS.Where(doc => doc.id == item && doc.estabelecimento_id == estab).FirstOrDefault();
                    if (gps != null)
                    {
                        listaGPS.Add(gps);
                    }
                }

            }
            if (tipoPagamento == "16") //DARF
            {
                foreach (var item in docid)
                {
                    var darf = db.DARF.Where(doc => doc.ID == item && doc.estabelecimento_id == estab).FirstOrDefault();
                    if (darf != null)
                    {
                        listaDARF.Add(darf);
                    }
                }

            }
            if (tipoPagamento == "35") //FGTS
            {
                foreach (var item in docid)
                {
                    var fgts = db.FGTS.Where(doc => doc.ID == item && doc.estabelecimento_id == estab).FirstOrDefault();
                    if (fgts != null)
                    {
                        listaFGTS.Add(fgts);
                    }
                }

            }

            foreach (var item in listaGPS)
            {
                variables.qtdRegistrosArquivo += 1;

                sb.Append("341"); //CÓDIGO DO BANCO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
                sb.Append("3"); //TIPO DE REGISTRO
                sb.Append("00001"); //NÚMERO DO REGISTRO
                sb.Append("N"); //SEGMENTO
                sb.Append("000"); //TIPO DE MOVIMENTO
                //Anexo C Inicio
                sb.Append("01"); //TRIBUTO - GPS
                sb.AppendFormat("{0,-4}", item.codigoPagamento.ToString().PadLeft(4, '0')); //CÓDIGO DE PAGTO
                sb.AppendFormat("{0,-6}", item.competencia.Replace("/", "")); //COMPETÊNCIA
                sb.AppendFormat("{0,-14}", item.identificador.Replace(".", "").Replace("-", "").Replace("/", "")); //IDENTIFICADOR
                sb.AppendFormat("{0,-14}", item.valorTributo.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //VALOR DO TRIBUTO
                sb.AppendFormat("{0,-14}", item.valorOutrasEntidades.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //VALOR OUTR.ENTIDADE
                sb.AppendFormat("{0,-14}", item.valorAtualizacaoMonetaria.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //ATUALIZ. MONETÁRIA
                sb.AppendFormat("{0,-14}", item.valorArrecadado.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //VALOR ARRECADADO
                sb.Append(item.dataArrecadacao.ToString("ddMMyyyy")); //DATA ARRECADAÇÃO
                sb.Append(' ', 8); //BRANCOS
                sb.AppendFormat("{0,-50}", item.informacoesComplementares); //USO EMPRESA
                sb.AppendFormat("{0,-30}", item.nomeContribuinte); //CONTRIBUINTE
                //Anexo C fim
                sb.Append(' ', 20); //SEU NÚMERO
                sb.Append(' ', 15); //NOSSO NÚMERO
                sb.Append(' ', 10); //OCORRÊNCIAS
                sb.AppendLine();
            }
            foreach (var item in listaDARF)
            {
                variables.qtdRegistrosArquivo += 1;

                sb.Append("341"); //CÓDIGO DO BANCO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
                sb.Append("3"); //TIPO DE REGISTRO
                sb.Append("00001"); //NÚMERO DO REGISTRO
                sb.Append("N"); //SEGMENTO
                sb.Append("000"); //TIPO DE MOVIMENTO
                //Anexo C Inicio
                sb.Append("02"); //IDENTIFICAÇÃO DO TRIBUTO - DARF
                sb.AppendFormat("{0,-4}", item.codigoReceita.ToString().PadLeft(4, '0')); //CÓDIGO DA RECEITA
                sb.Append("2"); //TIPO DE INSCRIÇÃO DO CONTRIBUINTE
                sb.AppendFormat("{0,-14}", item.cnpj.Replace(".", "").Replace("-", "").Replace("/", "")); //CPF OU CNPJ DO CONTRIBUINTE
                sb.Append(item.periodoApuracao.ToString("ddMMyyyy")); //PERÍODO DE APURAÇÃO
                sb.AppendFormat("{0,-4}", item.numeroReferencia.PadLeft(17, '0')); //NÚMERO DE REFERÊNCIA
                sb.AppendFormat("{0,-14}", item.valorPrincipal.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //VALOR PRINCIPAL
                sb.AppendFormat("{0,-14}", item.multa.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //VALOR DA MULTA
                sb.AppendFormat("{0,-14}", item.jurosEncargos.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //VALOR DOS JUROS/ENCARGOS
                sb.AppendFormat("{0,-14}", item.valorTotal.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //VALOR TOTAL A SER PAGO
                sb.Append(item.dataVencimento.ToString("ddMMyyyy")); //DATA DE VENCIMENTO
                sb.Append(item.dataVencimento.ToString("ddMMyyyy")); //DATA DO PAGAMENTO
                sb.Append(' ', 30); //COMPLEMENTO DE REGISTRO
                sb.AppendFormat("{0,-30}", item.nomeContribuinte); //NOME DO CONTRIBUINTE
                //Anexo C fim
                sb.Append(' ', 20); //SEU NÚMERO
                sb.Append(' ', 15); //NOSSO NÚMERO
                sb.Append(' ', 10); //OCORRÊNCIAS
                sb.AppendLine();
            }
            foreach (var item in listaFGTS)
            {
                variables.qtdRegistrosArquivo += 1;

                sb.Append("341"); //CÓDIGO DO BANCO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
                sb.Append("3"); //TIPO DE REGISTRO
                sb.Append("00001"); //NÚMERO DO REGISTRO
                sb.Append("N"); //SEGMENTO
                sb.Append("000"); //TIPO DE MOVIMENTO
                //Anexo C Inicio
                sb.Append("11"); //IDENTIFICAÇÃO DO TRIBUTO
                sb.AppendFormat("{0,-4}", item.codigoReceita.ToString().PadLeft(4, '0')); //CÓDIGO DA RECEITA
                sb.AppendFormat("{0,-1}", item.tipoInscricao); //TIPO DE INSCRIÇÃO DO CONTRIBUINTE
                sb.AppendFormat("{0,-14}", item.cnpj.Replace(".", "").Replace("-", "").Replace("/", "")); //CPF OU CNPJ DO CONTRIBUINTE
                sb.AppendFormat("{0,-48}", item.codigoBarras); //CODIGO DE BARRAS
                sb.AppendFormat("{0,-16}", item.identificadorFgts.ToString().PadLeft(16, '0')); //IDENTIFICADOR DO FGTS
                sb.AppendFormat("{0,-9}", item.lacreConectividadeSocial.ToString().PadLeft(9, '0')); //LACRE DE CONECTIVIDADE SOCIAL
                sb.AppendFormat("{0,-2}", item.digitoLacre.ToString().PadLeft(2, '0')); //DIGITO DO LACRE DE CONECTIVIDADE SOC.
                sb.AppendFormat("{0,-30}", item.nomeContribuinte); //NOME DO CONTRIBUINTE
                sb.Append(item.dataPagamento.ToString("ddMMyyyy")); //DATA DO PAGAMENTO
                sb.AppendFormat("{0,-14}", item.valorPagamento.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //VALOR DO PAGAMENTO
                sb.Append(' ', 30); //COMPLEMENTO DE REGISTRO
                //Anexo C fim
                sb.Append(' ', 20); //SEU NÚMERO
                sb.Append(' ', 15); //NOSSO NÚMERO
                sb.Append(' ', 10); //OCORRÊNCIAS
                sb.AppendLine();
            }
        }
        //Segmento N Trailler do Lote
        private static void traillerLoteN(StringBuilder sb, int codigoLote, int qtdRegistros, decimal valorTotal, string tipoPagamento)
        {
            qtdRegistros += 2;
            if (tipoPagamento == "17") //GPS
            {
                sb.Append("341"); //CÓDIGO DO BANCO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
                sb.Append("5"); //TIPO DE REGISTRO
                sb.Append(' ', 9); //BRANCOS
                sb.AppendFormat("{0,-6}", qtdRegistros.ToString().PadLeft(6, '0')); //TOTAL QTDE REGISTROS
                sb.AppendFormat("{0,-14}", variables.valorTributoGPS.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //TOTAL VALOR PRINCIPAL
                sb.AppendFormat("{0,-14}", variables.valorOutrasEntidadesGPS.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //TOTAL OUTRAS ENTIDAD.
                sb.AppendFormat("{0,-14}", variables.valorAtualizacaoGPS.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //TOTAL VAL. ACRESCIMOS
                sb.AppendFormat("{0,-14}", variables.valorArrecadadoGPS.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //TOTAL VALOR ARRECAD.
                sb.Append(' ', 151); //BRANCOS
                sb.Append(' ', 10); //OCORRÊNCIAS
            }
            if (tipoPagamento == "16") //DARF
            {
                sb.Append("341"); //CÓDIGO DO BANCO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
                sb.Append("5"); //TIPO DE REGISTRO
                sb.Append(' ', 9); //BRANCOS
                sb.AppendFormat("{0,-6}", qtdRegistros.ToString().PadLeft(6, '0')); //TOTAL QTDE REGISTROS
                sb.AppendFormat("{0,-14}", variables.valorPrincipalDARF.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //SOMA VALOR PRINCIPAL DOS PGTOS DO LOTE
                sb.Append('0', 14); //SOMA VALORES DE OUTRAS ENTIDADES DO LOTE
                sb.AppendFormat("{0,-14}", variables.valorMultaJurosDARF.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //SOMA VALORES ATUALIZ. MONET/MULTA/MORA
                sb.AppendFormat("{0,-14}", variables.valorTotalDARF.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //SOMA VALOR DOS PAGAMENTOS DO LOTE
                sb.Append(' ', 151); //BRANCOS
                sb.Append(' ', 10); //OCORRÊNCIAS
            }
            if (tipoPagamento == "35") //FGTS
            {
                sb.Append("341"); //CÓDIGO DO BANCO
                sb.AppendFormat("{0,-4}", codigoLote.ToString().PadLeft(4, '0')); //CÓDIGO DO LOTE
                sb.Append("5"); //TIPO DE REGISTRO
                sb.Append(' ', 9); //BRANCOS
                sb.AppendFormat("{0,-6}", qtdRegistros.ToString().PadLeft(6, '0')); //TOTAL QTDE REGISTROS
                sb.Append('0', 14); //SOMA VALOR PRINCIPAL DOS PGTOS DO LOTE
                sb.Append('0', 14); //SOMA VALORES DE OUTRAS ENTIDADES DO LOTE
                sb.Append('0', 14); //SOMA VALORES ATUALIZ. MONET/MULTA/MORA
                sb.AppendFormat("{0,-14}", variables.valorTotalFGTS.ToString("0.00").Replace(",", "").PadLeft(14, '0')); //SOMA VALOR DOS PAGAMENTOS DO LOTE
                sb.Append(' ', 151); //BRANCOS
                sb.Append(' ', 10); //OCORRÊNCIAS
            }
            sb.AppendLine();
        }

        //Trailler do Arquivo
        private static void traillerArquivo(StringBuilder sb, int qtdLotesArquivo, int qtdRegistrosArquivo)
        {

            sb.Append("341"); //CÓDIGO DO BANCO
            sb.Append("9999"); //CÓDIGO DO LOTE
            sb.Append("9"); //TIPO DE REGISTRO
            sb.Append(' ', 9); //BRANCOS
            sb.AppendFormat("{0,-6}", qtdLotesArquivo.ToString().PadLeft(6, '0')); //TOTAL QTDE DE LOTES
            sb.AppendFormat("{0,-6}", qtdRegistrosArquivo.ToString().PadLeft(6, '0')); //TOTAL QTDE REGISTROS
            sb.Append(' ', 211); //BRANCOS
            sb.AppendLine();
        }

    }
}

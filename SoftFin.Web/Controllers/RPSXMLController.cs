using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Models.NFes.Envio;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace SoftFin.Web.Controllers
{
    public class RPSController : BaseController
    {
        public ActionResult GeracaoArquivo()
        {
            @ViewData["DataInicial"] = DateTime.Now.AddDays(-30).Date.ToString("dd/MM/yyyy");
            @ViewData["DataFinal"] = DateTime.Now.Date.ToString("dd/MM/yyyy");

            return View();
        }


        [HttpPost]
        public FileStreamResult GeracaoArquivo(string dataInicial, string dataFinal)
        {
            List<int> nfs = new List<int>();
            List<NotaFiscal> listaAux = new List<NotaFiscal>();
            int estab = _paramBase.estab_id;
            var db = new DbControle();
            foreach (var item in Request.Form)
            {
                if (item.ToString().Substring(0, 2) == "nf")
                {
                    nfs.Add(int.Parse(item.ToString().Substring(2)));
                }
            }
            foreach (var item in nfs)
            {
                var nfe = db.NotaFiscal.Where(nf => nf.id == item && nf.estabelecimento_id == estab).FirstOrDefault();
                if (nfe != null)
                    listaAux.Add(nfe);
            }
            var listaRps = new ListaRps();
            foreach (var itemNF in listaAux)
	        {
                var infRps = new InfRps();
                infRps.Id = itemNF.numeroRps.ToString(); //Obrigatorio Caracter Informe um número para identificar o RPS. Pode ser um valor aleatório contendo letras e números.
                infRps.CodigoVerificacao = "5.00"; //Obrigatorio Caracter  	Informar a versão do layout do documento separado por ponto, Exemplo: 5.00
                infRps.Competencia = itemNF.dataEmissaoRps; //Obrigatorio date  Formato DD-MM-AAAA HH:mm:ss Onde: DD = 2 caracteres MM = 2 caracteres AAAA = 4 caracteres  HH = 2 caracteres mm = 2 caracteres ss = 2 caracteres
                infRps.DataEmissao = itemNF.dataEmissaoRps;//Obrigatorio date Formato DD-MM-AAAA HH:mm:ss Onde: DD = 2 caracteres MM = 2 caracteres AAAA = 4 caracteres  HH = 2 caracteres mm = 2 caracteres ss = 2 caracteres
                //infRps.NaturezaOperacao = itemNF.Operacao.NaturezaOperacao.Value; ///Obrigatorio numerico  1 - Tributação no Município;
                                                //2 – Tributação fora do Município;
                                                //3 – Isenção;
                                                //4 – Imune;
                                                //5 – Exigibilidade suspensa por decisão judicial;
                                                //6 – Exigibilidade suspensa por procedimento administrativo;
                                                //7 – Sem dedução;
                                                //8 – Com dedução / Materiais;
                                                //9 – Imune / Isenta de ISSQN;
                                                //10 – Devolução / Simples remessa;
                                                //11 – Intermediação;
                                                //12 – Prestação de serviço;
                                                //13 – Simples remessa;
                                                //14 – Anulada;
                                                //15 – Vencida;
                                                //16 – Tributada integralmente;
                                                //17 – Tributada integralmente com ISSRF;
                                                //18 – Tributada integralmente e sujeita a substituição tributária;
                                                //19 – Tributada com redução da base de cálculo;
                                                //20 – Tributada com redução da base de cálculo com ISSRF;
                                                //21 – Tributada com redução de cálculo e sujeita a substituição tributária;
                                                //22 – Não tributada – ISS regime fixo;
                                                //23 – Não tributada – ISS regime estimativa;
                                                //24 – Não tributada – ISS construção civil recolhido antecipadamente;
                                                //25 – Não tributada – Ato cooperado;
                                                //26 – Tributada no prestador;
                                                //27 – Tributada no tomador;
                                                //28 – Tributado Fixo;
                                                //29 – Isenta/Imune;
                                                //30 – Outro município.
                infRps.RegimeEspecialTributacao = 1; //Obrigatorio numerico
                                                //Informar o Regime Especial de tributação
                                                //1 - Microempresa Municipal;
                                                //2 – Estimativa;
                                                //3 – Sociedade de profissionais;
                                                //4 – Cooperativa;
                                                //5 – MEI – Simples Nacional;
                                                //6 – ME EPP – Simples Nacional;
                                                //7 – Isenta de ISS;
                                                //8 – Não incidência no município;
                                                //9 – Imune;
                                                //10 – Exigibilidade Susp.Dec J/Proc.A;
                                                //11 – Não tributável;
                                                //12 – Tributável;
                                                //13 – Tributável fixo;
                                                //14 – Tributável S.N;
                infRps.OptanteSimplesNacional = 1; //Obrigatorio numerico Informar se o prestador de serviços é ou não Optante pelo Simples Nacional.
                                                //1 – Sim
                                                //2 - Não
                infRps.IncentivoFiscal = 2; //Obrigatorio numerico 	
                                                //Informar se o Prestador de serviços tem incentivo fiscal
                                                //1 – Sim
                                                //2 – Não
                infRps.Status = 1; //Obrigatorio numerico
                                                //                Informar o Status do RPS
                                                //1 – Normal;
                                                //2 – Cancelado;
                                                //3 – Extraviada;
                                                //4 – Lote;
                infRps.TributarMunicipio = 1; //Obrigatorio numerico
                                                //Informar se o serviço será ou não tributado no município.
                                                //1 - Sim
                                                //2 - Não
                infRps.TributarPrestador = 1; //Obrigatorio numerico
                                                //Informar de o serviço será ou não tributado pelo prestador.
                                                //1 - Sim
                                                //2 - Não

                var identificacaoRps = new IdentificacaoRps();
                identificacaoRps.Numero = itemNF.numeroRps; //Obrigatorio numerico  Informar o número da RPS.
                identificacaoRps.Serie = itemNF.serieRps; //Obrigatorio caracter Informar a série da RPS.
                identificacaoRps.Tipo = itemNF.tipoRps; //Obrigatorio numerico Código de tipo de RPS
                                                //1 - RPS
                                                //2 – Nota Fiscal Conjugada (Mista)
                                                //3 – Cupom

                var valores = new Valores();
                valores.ValorServicos = itemNF.valorNfse; //Obrigatorio 0.00 Valor total dos serviços.
                valores.ValorDeducoes = itemNF.valorDeducoes; //Obrigatorio 0.00 Valor das deduções para Redução da Base de Cálculo em R$. Quando não possuir informar o valor 0,00.
                valores.ValorPis = itemNF.pisRetido; //Obrigatorio 0.00 Valor da retenção do PIS em R$. Informação declaratória.Quando não possuir informar o valor 0,00.  
                valores.ValorCofins = itemNF.cofinsRetida; //Obrigatorio 0.00 Valor da retenção do COFINS em R$. Informação declaratória.Quando não possuir informar o valor 0,00.
                valores.ValorInss = 0; //Obrigatorio 0.00 Valor da retenção do INSS em R$. Informação declaratória. Quando não possuir informar o valor 0,00.
                valores.ValorIr = itemNF.irrf; //Obrigatorio 0.00 Valor da retenção do IR em R$. Informação declaratória. Quando não possuir informar o valor 0,00.
                valores.ValorCsll = itemNF.csllRetida; // 0.00 Valor da retenção do CSLL em R$. Informação declaratória. Quando não possuir informar o valor 0,00.  
                valores.ValorIss = itemNF.valorISS; // 0.00 Valor do ISS em R$. Quando não houver o valor, poderá ser informado o valor 0,00.
                valores.ValorIssRetido = itemNF.valorISS; // 0.00 Valor do ISS a ser retido em R$. Quando não houver o valor, poderá ser informado o valor 0,00.
                valores.OutrasRetencoes = 0; // 0.00  Outras retenções na Fonte em R$. Informação Declaratória. Quando não houver o valor, poderá ser informado o valor 0,00.
                valores.BaseCalculo = itemNF.basedeCalculo; //Obrigatorio 0.00  Base de cálculo em R$. Quando não possuir, informar o valor 0,00.
                valores.Aliquota = itemNF.aliquotaISS; //Obrigatorio 0.00 Alíquota. Valor percentual.
                                                        //Formato: 0,0000
                                                        //Ex: 1% = 0,0100
                                                        //25,5% = 0,2550
                                                        //100% = 1,0000
                valores.ValorLiquidoNfse = itemNF.valorLiquido; //Obrigatorio 0.00 Valor Líquido da NFS-e em R$. Quando não houver, informar 0,00.
                valores.DescontoIncondicionado = 0;// 0.00 Valor de Desconto Incondicionado em R$. Quando não houver poderá ser informado o valor 0,00.
                valores.DescontoCondicionado = 0; // 0.00 Valor de Desconto Condicionado em R$. Quando não houver poderá ser informado o valor 0,00.


                var servicos = new Servicos();
                servicos.ItemListaServico = itemNF.codigoServico; //Informar o código do serviço prestado. Deverá ser informado o código da Item Lista Serviço conforme a LC 116/2003 considerando o separador "." (ponto) (Ex: 11.01, 14.01).
                servicos.CodigoCnae = 0; //Informar o código CNAE da atividade do RPS. Verificar junto à Prefeitura qual o código CNAE correto para a sua atividade de prestação. Verifique quais são as atividades vinculadas ao prestador.
                servicos.CodigoTributacaoMunicipio = "0"; // 	Informar o código do serviço prestado próprio do município.
                servicos.Discriminacao = itemNF.discriminacaoServico; //Discriminação dos serviços prestados.
                servicos.MunicipioIncidencia = 0; //Informar o código IBGE do municipio de incidência do serviço.
                servicos.MunicipioIncidenciaSiafi = null; //Informar o código SIAFI do município de incidência.
                servicos.NumeroProcesso = null; //Informar o número do processo em caso de irregularidade com o municipio.
                servicos.DescricaoRPS = null; //Descrição / Dados complementares da RPS.
                servicos.IssRetido = 1; //Informar se o valor de ISS será retido ou não. 1 - Sim 2 - Não
                servicos.ResponsavelRetencao = 1;//Informar o responsável pela retenção do ISS  1 - Tomador 2 - Intermediário
                listaRps.Rps.InfRps.Add(infRps);

                var dadosComplementaresServico = new DadosComplementaresServico();
                dadosComplementaresServico.TipoRecolhimento = 1; //Tipo de Recolhimento 1-A Receber 2-Retido na Fonte
                dadosComplementaresServico.MotivoRetencao = null; //Informar motivo de Retenção.
                dadosComplementaresServico.MunicipioPrestacaoDescricao = itemNF.NotaFiscalPessoaTomador.cidade;// 	Preencher com o nome município de prestação do serviço. Caso o município de prestação seja exterior, informar neste campo o nome do país.
                dadosComplementaresServico.SeriePrestacao = 99; //Número do equipamento emissor do RPS ou série de prestação. Caso não utilize a série, preencha o campo com o valor ‘99’ que indica modelo único. Caso queira utilizar o campo série para indicar o número do equipamento emissor do RPS deve-se solicitar liberação da prefeitura.Este campo deve ser informado por padrão o valor ‘99’ - Modelo único, porém quando liberado pela prefeitura o contribuinte pode utilizar este campo para indicar o número do equipamento emissor do RPS, podendo ser utilizado a numeração de 01 a 99. A numeração seqüencial do RPS é por Série de Prestação, sendo assim cada série têm uma numeração seqüencial
                dadosComplementaresServico.MotCancelamento = null; //Motivo do Cancelamento. Obs.:Obrigatório caso o RPS for cancelado

                var item = new Item();
                item.ItemListaServico = ""; // Obrigatorio numerico  	Informar o código do serviço prestado. Deverá ser informado o código da Item Lista Serviço conforme a LC 116/2003 considerando o separador "." (ponto) (Ex: 11.01, 14.01).
                item.CodigoCnae = 0;// Obrigatorio numerico   	Informar o código CNAE da atividade do RPS. Verificar junto à Prefeitura qual o código CNAE correto para a sua atividade de prestação. Verifique quais são as atividades vinculadas ao prestador.
                item.DiscriminacaoServico = itemNF.discriminacaoServico;// Obrigatorio caracter  Informar discriminação do serviço prestado
                item.Quantidade = 1;// Obrigatorio numerico Informar a quantidade de itens do serviço prestado. 
                item.ValorUnitario = itemNF.valorLiquido;// Obrigatorio 0.00 Informar o valor unitário do item de serviço.
                item.ValorDesconto = 0;// Obrigatorio 0.00  // 	Valor desconto do item de serviço. Quando não houver informar 0,00.
                item.ValorTotal = itemNF.valorNfse;// Obrigatorio 0.00 Informar o valor total o item de serviço.
                item.ServicoTributavel = 1;// Obrigatorio numerico Informar se o serviço é tributavel ou não. 1- Sim; 2 - Não

                var prestador = new Prestador();
                prestador.Cnpj = itemNF.NotaFiscalPessoaTomador.cnpjCpf; // Obrigatorio numerico  	Informar o CNPJ do prestador de serviços.
                prestador.InscricaoMunicipal = itemNF.NotaFiscalPessoaTomador.inscricaoMunicipal; // Obrigatorio CARACTER Informar a inscrição municipal do prestador de serviços


            }

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.NFes.Envio.ListaRps));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            StringBuilder sb = new StringBuilder();

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, listaRps);
                }
                sb = textWriter.GetStringBuilder(); //This is the output as a string
            }


            var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            return File(stream, "xml/application", "RPS" + DateTime.Now.ToString("ddMMyyyhhmmss") + "_envi.xml");
        }
        [HttpPost]
        public JsonResult ConsultaNota(string dataInicial, string dataFinal)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);


            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);



            var nfs = new NotaFiscal().ObterTodos(DataInicial, DataFinal, _paramBase).Where(p => p.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA
                                        || p.situacaoPrefeitura_id == Models.NotaFiscal.NFCANCELADAEMCONF);
            List<RetornoNota> rets = new List<RetornoNota>();

            foreach (var item in nfs)
            {
                rets.Add(new RetornoNota
                {
                    Cliente = item.NotaFiscalPessoaTomador.razao,
                    id = item.id,
                    RPS = item.numeroRps.ToString(),
                    Valor = item.valorNfse.ToString("0.00"),
                    Data = item.dataEmissaoRps.ToString("dd/MM/yyyy"),
                    Situacao = Models.NotaFiscal.CarregaSituacao(item.situacaoPrefeitura_id)
                });
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(rets);

            return Json(rets, JsonRequestBehavior.AllowGet);
        }

    }
}

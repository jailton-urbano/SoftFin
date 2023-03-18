using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class BarueriTXTController : BaseController
    {
        //
        // GET: /BarueriTXT/
        public ActionResult Index()
        {
            @ViewData["DataInicial"] = DateTime.Now.AddDays(-30).Date.ToString("dd/MM/yyyy");
            @ViewData["DataFinal"] = DateTime.Now.Date.ToString("dd/MM/yyyy");

            return View();
        }


        [HttpPost]
        public FileStreamResult Index(string dataInicial, string dataFinal)
        {
            StringBuilder sb = new StringBuilder();

            List<int> nfs = new List<int>();

            foreach (var item in Request.Form)
            {
                if (item.ToString().Substring(0, 2) == "nf")
                {
                    nfs.Add(int.Parse(item.ToString().Substring(2)));
                }

            }

            var nfsBarueri = new List<SoftFin.NFSe.Business.DTO.Cidade.Barueri.NFTexto>();

            Convert(nfs, nfsBarueri);

            sb = new SoftFin.NFSe.Business.GeracaoArquivoTexto.Cidades.Barueri().MakeFileRPS(nfsBarueri, _estabobj.InscricaoMunicipal, "");
            
            var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            return File(stream, "text/plain", "RPSBaurueri_" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".txt");
        }

        private void Convert(List<int> nfs, List<SoftFin.NFSe.Business.DTO.Cidade.Barueri.NFTexto> nfsBarueri)
        {
            foreach (var item in nfs)
            {
                var nf = new NotaFiscal().ObterPorId(item);
                var nfbarueri = new SoftFin.NFSe.Business.DTO.Cidade.Barueri.NFTexto();

                nfbarueri.serieRPS = nf.serieRps ;
                nfbarueri.numeroRPS = nf.numeroRps;
                nfbarueri.dataEmissaoRps = nf.dataEmissaoRps;
                if (nf.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA)
                    nfbarueri.situacaoRPS = "E";
                else
                {
                    nfbarueri.situacaoRPS = "C";
                    nfbarueri.codigomotivocancelamento = "02";
                    nfbarueri.numeroNFE = nf.numeroNfse.Value.ToString();
                    nfbarueri.serieNFE = nf.serieRps.ToString();
                    nfbarueri.dataEmissaoNFe = nf.dataEmissaoNfse;
                    nfbarueri.descricaoCancelamento = "Cancelada por motivo de falha de preenchimento.";
                }
                nfbarueri.codigoServico = nf.codigoServico;
                nfbarueri.localservicoprestado = "1";
                nfbarueri.prestadoViasPublicas = "2";
                nfbarueri.QuantidadeServico = "1";
                nfbarueri.ValorServico = nf.valorNfse;
                nfbarueri.ValorTotalRetencoes = nf.valorDeducoes;
                nfbarueri.TomadorEstrangeiro = "2";
                nfbarueri.indicadorCPFCNPJ = nf.NotaFiscalPessoaTomador.indicadorCnpjCpf.ToString();

                nfbarueri.cnpjCpf = nf.NotaFiscalPessoaTomador.cnpjCpf;
                nfbarueri.razaoSocial = nf.NotaFiscalPessoaTomador.razao;
                nfbarueri.tomadorEndereco = nf.NotaFiscalPessoaTomador.endereco;
                nfbarueri.tomadorNumero = nf.NotaFiscalPessoaTomador.numero;
                nfbarueri.tomadorComplemento = nf.NotaFiscalPessoaTomador.complemento;
                nfbarueri.tomadorBairro = nf.NotaFiscalPessoaTomador.bairro;
                nfbarueri.tomadorCidade = nf.NotaFiscalPessoaTomador.cidade;
                nfbarueri.tomadorUF = nf.NotaFiscalPessoaTomador.uf;
                nfbarueri.tomadorCep = nf.NotaFiscalPessoaTomador.cep;
                nfbarueri.tomadorEmail = nf.NotaFiscalPessoaTomador.email;
                nfbarueri.descricaoCancelamento = nf.discriminacaoServico;

                if (nf.irrf != 0)
                    nfbarueri.impostos.Add(new SoftFin.NFSe.Business.DTO.Cidade.Barueri.imposto { codigoOutrosValores = "01", valor = nf.irrf });
                if (nf.pisRetido != 0)
                    nfbarueri.impostos.Add(new SoftFin.NFSe.Business.DTO.Cidade.Barueri.imposto { codigoOutrosValores = "02", valor = nf.pisRetido });
                if (nf.cofinsRetida != 0)
                    nfbarueri.impostos.Add(new SoftFin.NFSe.Business.DTO.Cidade.Barueri.imposto { codigoOutrosValores = "03", valor = nf.cofinsRetida });
                if (nf.csllRetida != 0)
                    nfbarueri.impostos.Add(new SoftFin.NFSe.Business.DTO.Cidade.Barueri.imposto { codigoOutrosValores = "04", valor = nf.csllRetida });

                nfsBarueri.Add(nfbarueri);
            }
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

using OFX;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace SoftFin.Web.Controllers
{
    public class BarueriTXTImpController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            int estab = _paramBase.estab_id;

            var arquivo = Request.Files[0];

            if (arquivo != null && arquivo.ContentLength > 0)
            {
                var fileName = Path.GetFileName(arquivo.FileName);


                if (fileName.ToUpper().IndexOf(".TXT") == -1)
                {
                    ViewBag.msg = "Arquivo invalido.";
                    return View();
                }

                var path = Path.Combine(Server.MapPath("~/TXTTemp/"), fileName);
                arquivo.SaveAs(path);


                var lines = System.IO.File.ReadAllLines(path).ToList();

                foreach (string line in lines)
                {
                    if (line.Substring(0, 1).Equals("2"))
                    {
                        var numeroNFE = line.Substring(6, 12);
                        string dataNFE = line.Substring(12, 8);
                        string codigoVerificacao = line.Substring(27, 24);
                        string tipo = line.Substring(66, 1);
                        int numRPS = int.Parse(line.Substring(54, 10));

                        DbControle bc = new DbControle();
                        var nf = bc.NotaFiscal.Where(p => p.numeroRps == numRPS && p.estabelecimento_id == estab).FirstOrDefault();
                        if (nf != null)
                        {

                            nf.dataEmissaoNfse = DateTime.Parse(dataNFE.Substring(6, 2) + "/" //5,2
                                                                + dataNFE.Substring(4, 2) + "/" //4,2
                                                                + dataNFE.Substring(0, 4)); //0,4
                            nf.codigoVerificacao = codigoVerificacao;
                            nf.numeroNfse = int.Parse(numeroNFE);
                            if (tipo.Equals("C"))
                                nf.situacaoPrefeitura_id = Models.NotaFiscal.NFCANCELADACCONF;
                            else
                            {
                                nf.situacaoPrefeitura_id = Models.NotaFiscal.NFGERADAENVIADA;
                                
                                int? idNf = nf.id;
                                BancoMovimento bancoMovimento = new BancoMovimento().ObterTodos(_paramBase).Where(p => p.notafiscal_id == idNf).FirstOrDefault();
                                OrigemMovimento origemMovimento = new OrigemMovimento().ObterTodos(_paramBase).Where(x => x.Modulo == "Faturamento").FirstOrDefault();

                                if (bancoMovimento == null)
                                {
                                    BancoMovimento bancoMovimentoNew = new BancoMovimento();
                                    bancoMovimentoNew.banco_id = nf.banco_id.Value;
                                    bancoMovimentoNew.data = nf.dataVencimentoNfse;
                                    bancoMovimentoNew.valor = nf.valorNfse;
                                    bancoMovimentoNew.planoDeConta_id = 4;
                                    bancoMovimentoNew.tipoDeMovimento_id = new TipoMovimento().TipoEntrada(_paramBase);
                                    bancoMovimentoNew.tipoDeDocumento_id = new TipoDocumento().TipoNota();
                                    bancoMovimentoNew.origemmovimento_id = origemMovimento.id;
                                    bancoMovimentoNew.planoDeConta_id = new PlanoDeConta().ObterPorCodigo("01.01", bc).id;
                                    bancoMovimentoNew.historico = "NF Nº" + nf.numeroNfse.ToString();
                                    bancoMovimentoNew.notafiscal_id = nf.id;

                                    //Inclui novo banco movimento
                                    bancoMovimentoNew.Incluir(bancoMovimentoNew, _paramBase);
                                }
                                else
                                {

                                }
                            }
                            bc.SaveChanges();
                        }

                    }
                }


                ViewBag.msg = "Importação Finalizada com sucesso";
            }


            return View();
        }
    }
}
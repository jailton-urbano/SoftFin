using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SoftFin.Web.Models;
using SoftFin.Web.Classes;
using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using BoletoNet;

using System.Text;

namespace SoftFin.Web.Controllers
{
    public class RecebimentoController : BaseController
    {


        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var AReceber = new Recebimento().ObterEntreData(DataInicial, DataFinal, _paramBase).Sum(x => x.valorRecebimento).ToString("n");
            return Json(new { CDStatus = "OK", Result = "R$ " + AReceber }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Decide(int ID)
        {
            try
            {
                Recebimento recebimento = new Recebimento().ObterPorNFSeId(ID, null);

                return RedirectToAction("Create", new { @id = ID });
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        private void CarregaBanco()
        {
            var con1 = new SoftFin.Web.Models.Banco().ObterTodos(_paramBase).Where(p => p.EmiteBoleto == true).OrderBy(p => p.principal);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1} - {2} - {3}", item.codigoBanco, item.nomeBanco, item.agencia, item.contaCorrente), Selected = item.principal });
            }
            var listret = new SelectList(items, "Value", "Text");
            ViewData["banco"] = listret;
        }

        public ActionResult SeletorBanco(int id)
        {
            ViewData["notafiscal_id"] = id.ToString();
            CarregaBanco();
            var nf = new NotaFiscal().ObterPorId(id);
            ViewData["valor"] = nf.valorNfse.ToString("0.00");
            ViewData["data_vencimento"] = nf.dataVencimentoNfse.ToString("dd/MM/yyyy");
            return View();
        }

        [HttpPost]
        public ActionResult SeletorBanco(string notafiscal_id, string valor, string banco_id, string data_vencimento, string instrucao01, string instrucao02, string instrucao03)
        {

            return RedirectToAction("Boleto", new { notafiscal_id = notafiscal_id, 
                banco_id = banco_id, 
                data_vencimento = DateTime.Parse(data_vencimento),
                valor = decimal.Parse(valor),
                instrucao01 = instrucao01,
                instrucao02 = instrucao02,
                instrucao03 = instrucao03

            });
        }


        public ActionResult Boleto(int notafiscal_id, decimal valor, int banco_id, DateTime data_vencimento, string instrucao01, string instrucao02, string instrucao03)
        {
            try
            {
                var bancoAux = new SoftFin.Web.Models.Banco().ObterPorId(banco_id, _paramBase);
                int numBoleto = bancoAux.numeroDocumento + 1;
                var estab = new Estabelecimento().ObterPorId(_paramBase.estab_id, _paramBase);
                var nf = new NotaFiscal().ObterPorId(notafiscal_id);

                ParamNotas paramNotas = new ParamNotas();

                paramNotas.Banco = bancoAux;
                paramNotas.Estabelecimento = estab;
                paramNotas.numBoleto = numBoleto;
                paramNotas.NotaFiscal = nf;
                paramNotas.datVencimento = data_vencimento;
                paramNotas.instrucao01 = instrucao01;
                paramNotas.instrucao02 = instrucao02;
                paramNotas.instrucao03 = instrucao03;
                paramNotas.valor = valor;

                switch (bancoAux.codigoBanco)
                {
                    case "237":
                        ViewBag.Boleto = Bradesco(paramNotas);
                        break;
                    case "341":
                        ViewBag.Boleto = Itau(paramNotas);
                        break;
                    case "033":
                        ViewBag.Boleto = Santander(paramNotas);
                        break;
                    case "33":
                        ViewBag.Boleto = Santander(paramNotas);
                        break;
                    case "473":
                        ViewBag.Boleto = Caixa(paramNotas);
                        break;
                    case "001":
                    case "01":
                    case "1":
                        ViewBag.Boleto = BB(paramNotas);
                        break;
                    default:
                        ViewBag.Boleto = "Boleto Banco não implementado";
                        break;
                }

                AtualizaNumeroBoleto(paramNotas);

                return View();
            }
            catch(Exception ex)
            {
                _eventos.Error(ex.ToString());
                Response.Write(ex.ToString());
                return View();
            }
        }

        private void AtualizaNumeroBoleto(ParamNotas paramNotas)
        {
            DbControle bancoaux = new DbControle();
            var banco = bancoaux.Bancos.Where(x => x.id == paramNotas.Banco.id).FirstOrDefault();
            banco.numeroDocumento = paramNotas.numBoleto;
            bancoaux.SaveChanges();
        }

        private dynamic Santander(ParamNotas paramNotas)
        {
 	        DateTime vencimento = paramNotas.datVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ, 
                    paramNotas.Estabelecimento.NomeCompleto, 
                    paramNotas.Banco.agencia, 
                    paramNotas.Banco.agenciaDigito, 
                    paramNotas.Banco.contaCorrente, 
                    paramNotas.Banco.contaCorrenteDigito);
            
            c.Codigo = "13000";
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.Banco.carteira.ToString(), paramNotas.Banco.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numBoleto).ToString();
            b.NossoNumero = paramNotas.Banco.nossoNumero;
            b.Sacado = new Sacado(paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf, paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao);
            b.Sacado.Endereco.End = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao;
            b.Sacado.Endereco.Bairro = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cidade;
            b.Sacado.Endereco.CEP = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cep;
            b.Sacado.Endereco.UF = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.uf;
            b.ValorBoleto = paramNotas.valor;
           
            Instrucao i = new Instrucao(33);
            i.Descricao = paramNotas.instrucao01;
            b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(33);
            i2.Descricao = paramNotas.instrucao02;
            b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(33);
            i3.Descricao = paramNotas.instrucao03;
            b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 33;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = true;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        private dynamic Caixa(ParamNotas paramNotas)
        {
            DateTime vencimento = paramNotas.datVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ,
                    paramNotas.Estabelecimento.NomeCompleto,
                    paramNotas.Banco.agencia,
                    paramNotas.Banco.agenciaDigito,
                    paramNotas.Banco.contaCorrente,
                    paramNotas.Banco.contaCorrenteDigito);

            c.Codigo = "13000";
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.Banco.carteira.ToString(), paramNotas.Banco.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numBoleto).ToString();
            b.NossoNumero = paramNotas.Banco.nossoNumero;
            b.Sacado = new Sacado(paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf, paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao);
            b.Sacado.Endereco.End = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao;
            b.Sacado.Endereco.Bairro = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cidade;
            b.Sacado.Endereco.CEP = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cep;
            b.Sacado.Endereco.UF = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.uf;
            b.ValorBoleto = paramNotas.valor;

            Instrucao i = new Instrucao(33);
            i.Descricao = paramNotas.instrucao01;
            b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(33);
            i2.Descricao = paramNotas.instrucao02;
            b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(33);
            i3.Descricao = paramNotas.instrucao03;
            b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 473;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = true;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        private dynamic BB(ParamNotas paramNotas)
        {
            DateTime vencimento = paramNotas.datVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ,
                    paramNotas.Estabelecimento.NomeCompleto,
                    paramNotas.Banco.agencia,
                    paramNotas.Banco.agenciaDigito,
                    paramNotas.Banco.contaCorrente,
                    paramNotas.Banco.contaCorrenteDigito);

            c.Codigo = "13000";
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.Banco.carteira.ToString(), paramNotas.Banco.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numBoleto).ToString();
            b.NossoNumero = paramNotas.Banco.nossoNumero;
            b.Sacado = new Sacado(paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf, paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao);
            b.Sacado.Endereco.End = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao;
            b.Sacado.Endereco.Bairro = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cidade;
            b.Sacado.Endereco.CEP = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cep;
            b.Sacado.Endereco.UF = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.uf;
            b.ValorBoleto = paramNotas.valor;

            Instrucao i = new Instrucao(33);
            i.Descricao = paramNotas.instrucao01;
            b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(33);
            i2.Descricao = paramNotas.instrucao02;
            b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(33);
            i3.Descricao = paramNotas.instrucao03;
            b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 1;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = true;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

        private dynamic Itau(ParamNotas paramNotas)
        {
 	        DateTime vencimento = paramNotas.datVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ, 
                    paramNotas.Estabelecimento.NomeCompleto, 
                    paramNotas.Banco.agencia, 
                    paramNotas.Banco.agenciaDigito, 
                    paramNotas.Banco.contaCorrente, 
                    paramNotas.Banco.contaCorrenteDigito);
            
            c.Codigo = "13000";
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.Banco.carteira.ToString(), paramNotas.Banco.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numBoleto).ToString();
            b.NossoNumero = paramNotas.Banco.nossoNumero;
            b.Sacado = new Sacado(paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf, paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao);
            b.Sacado.Endereco.End = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao;
            b.Sacado.Endereco.Bairro = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cidade;
            b.Sacado.Endereco.CEP = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cep;
            b.Sacado.Endereco.UF = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.uf;
            b.ValorBoleto = paramNotas.valor;
            

            Instrucao i = new Instrucao(341);
            i.Descricao = paramNotas.instrucao01;
            b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(341);
            i2.Descricao = paramNotas.instrucao02;
            b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(341);
            i3.Descricao = paramNotas.instrucao03;
            b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 341;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = true;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml(System.Web.Hosting.HostingEnvironment.MapPath("/Content/Boletos/"), "");
        }

        public class ParamNotas
        {
            public int numBoleto { get; set; }
            public int notafiscal_id { get; set; }
            public SoftFin.Web.Models.Banco Banco { get; set; }
            public NotaFiscal NotaFiscal { get; set; }
            public Estabelecimento Estabelecimento { get; set; }
            public DateTime datVencimento { get; set; }
            public string instrucao01 { get; set; }
            public string instrucao02 { get; set; }
            public string instrucao03 { get; set; }
            public decimal valor { get; set;}
        }



        public string Bradesco( ParamNotas paramNotas )
        {
            DateTime vencimento = paramNotas.datVencimento;
            Cedente c = new Cedente(paramNotas.Estabelecimento.CNPJ, 
                    paramNotas.Estabelecimento.NomeCompleto, 
                    paramNotas.Banco.agencia, 
                    paramNotas.Banco.agenciaDigito, 
                    paramNotas.Banco.contaCorrente, 
                    paramNotas.Banco.contaCorrenteDigito);
            
            c.Codigo = "13000";
            //Carteiras 
            BoletoNet.Boleto b = new BoletoNet.Boleto(vencimento, 1.01m, paramNotas.Banco.carteira.ToString("00"), paramNotas.Banco.contaCorrente, c);
            b.NumeroDocumento = (paramNotas.numBoleto).ToString();
            b.NossoNumero = paramNotas.Banco.nossoNumero;
            b.Sacado = new Sacado(paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cnpjCpf, paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao);
            b.Sacado.Endereco.End = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.razao;
            b.Sacado.Endereco.Bairro = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.bairro;
            b.Sacado.Endereco.Cidade = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cidade;
            b.Sacado.Endereco.CEP = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.cep;
            b.Sacado.Endereco.UF = paramNotas.NotaFiscal.NotaFiscalPessoaTomador.uf;
            b.ValorBoleto = paramNotas.valor;
            

            Instrucao i = new Instrucao(237);
            i.Descricao = paramNotas.instrucao01;
            b.Instrucoes.Add(i);

            Instrucao i2 = new Instrucao(237);
            i2.Descricao = paramNotas.instrucao02;
            b.Instrucoes.Add(i2);

            Instrucao i3 = new Instrucao(237);
            i3.Descricao = paramNotas.instrucao03;
            b.Instrucoes.Add(i3);


            var boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 237;

            boletoBancario.MostrarContraApresentacaoNaDataVencimento = true;

            boletoBancario.Boleto = b;
            boletoBancario.Boleto.Valida();

            return boletoBancario.MontaHtml("/Content/Boletos/", "");
        }

       
        private void CarregaViewData()
        {
            ViewData["notafiscal"] = new SelectList(new NotaFiscal().ObterTodos(_paramBase), "id", "numeroNfse");
            ViewData["banco"] = new SoftFin.Web.Models.Banco().CarregaBancoGeral(_paramBase);
            ViewData["tipodocumento"] = new SoftFin.Web.Models.TipoDocumento().CarregaCboGeral();
        }

 



    }
}

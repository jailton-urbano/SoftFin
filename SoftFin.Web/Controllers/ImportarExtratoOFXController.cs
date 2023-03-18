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
    public class ImportarExtratoOFXController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {

            var arquivo = Request.Files[0];

            if (arquivo != null && arquivo.ContentLength > 0)
            {
                var fileName = Path.GetFileName(arquivo.FileName);


                if (fileName.ToUpper().IndexOf(".OFX") == -1)
                {
                    ViewBag.msg = "Arquivo inválido.";
                    return View();
                }

                var uploadPath = Server.MapPath("~/OFXTemp/");
                Directory.CreateDirectory(uploadPath);

                var path = Path.Combine(Server.MapPath("~/OFXTemp/"), fileName);
                arquivo.SaveAs(path);

                
                FileStream SourceStream = System.IO.File.Open(path, FileMode.Open);
                var ofx = new OfxDocument();

                var extrato = ofx.CarregaOFX(SourceStream);

                var codBanco = "";
                var codAgencia = "";
                var codConta = "";
                var codDigito = "";
                DbControle db = new DbControle();

                int estab = _paramBase.estab_id;
                var banco = new Banco();

                if (ofx.BankID == "0237")
                {
                    codBanco = int.Parse(ofx.BankID).ToString();
                    codConta = ofx.AccountID.ToString();
                    banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                            x.codigoBanco == codBanco &&
                            x.contaCorrente == codConta).FirstOrDefault();

                }
                else if (ofx.BankID.Substring(0,3) == "033")
                {
                    codBanco = ofx.BankID.Substring(0, 3);
                    codAgencia = ofx.AccountID.Substring(0, 4).ToString();
                    codConta = ofx.AccountID.Substring(4, 8).ToString();
                    codDigito = ofx.AccountID.Substring(12, 1).ToString();
                    banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                            x.agencia == codAgencia &&
                            x.contaCorrenteDigito == codDigito &&
                            x.contaCorrente == codConta &&
                            x.codigoBanco == codBanco).FirstOrDefault();

                }
                else if (ofx.BankID.IndexOf("104") >= 0)
                {
                    codBanco = int.Parse(ofx.BankID).ToString();
                    //codAgencia = ofx.AccountID.Substring(0, 4).ToString();
                    codConta = ofx.AccountID.Substring(4, 4).ToString();
                    codDigito = ofx.AccountID.Substring(8, 1).ToString();
                    banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                            x.contaCorrenteDigito == codDigito &&
                            x.contaCorrente == codConta &&
                            x.codigoBanco == codBanco).FirstOrDefault();

                }
                else
                {
                    
                    codBanco = int.Parse(ofx.BankID).ToString();
                    if (codBanco == "1") //Banco do Brasil
                    {
                        //codAgencia = ofx.AccountID.Substring(0, 4).ToString();
                        codConta = ofx.AccountID.Substring(0, 4).ToString();
                        codDigito = ofx.AccountID.Substring(5, 1).ToString();
                        banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                                x.contaCorrenteDigito == codDigito &&
                                x.contaCorrente == codConta &&
                                x.codigoBanco == "001").FirstOrDefault();
                    }
                    else
                    {
                        codAgencia = ofx.AccountID.Substring(0, 4).ToString();
                        codConta = ofx.AccountID.Substring(4, 5).ToString();
                        codDigito = ofx.AccountID.Substring(9, 1).ToString();
                        banco = db.Bancos.Where(x => x.estabelecimento_id == estab &&
                            x.agencia == codAgencia &&
                            x.contaCorrenteDigito == codDigito &&
                            x.contaCorrente == codConta &&
                            x.codigoBanco == codBanco).FirstOrDefault();

                    }


                }
                    




                if (banco == null)
                {
                    ViewBag.msg = "Configure o banco corretamente, acesse o cadastro de banco";
                    return View();
                }
                
                
                foreach (var item in extrato)
                {
                    var vData = DateTime.Parse(item.DatePosted.Substring(0, 4) + "-" + item.DatePosted.Substring(4, 2) + "-" + item.DatePosted.Substring(6, 2));
                    var vValor = decimal.Parse(item.TransAmount.Replace(".", ","));

                    LanctoExtrato lanctoExtrato = db.LanctoExtrato.Where(p => p.banco_id == banco.id 
                                                                           && p.idLancto == item.FITID
                                                                           && p.data == vData
                                                                           && p.Valor == vValor).FirstOrDefault();

                    if (lanctoExtrato == null)
                    {
                        
                        lanctoExtrato = new LanctoExtrato();
                        db.LanctoExtrato.Add(lanctoExtrato);
                    }

                    lanctoExtrato.idLancto = item.FITID.Trim();
                    lanctoExtrato.banco_id = banco.id;
                    lanctoExtrato.data = vData;
                    lanctoExtrato.descricao = item.Memo.Trim();
                    if (item.TransType == TransTypes.Credit)
                        lanctoExtrato.Tipo = "C";
                    else
                        lanctoExtrato.Tipo = "D";

                    lanctoExtrato.Valor = vValor;

                    db.SaveChanges();
                }

                ViewBag.msg = "Importação Finalizada com sucesso";
            }


            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class CodigoBarrasController : BaseController
    {

	//BOLETO TAMANHO: Linha Digitável: 47 e Código de Barras: 43
	//CONCESSIONÁRIAS E TRIBUTOS COM CÓDIGO DE BARRAS: Linha Digitável: 48 e Código de Barras: 44

        public ActionResult codigoBarras()
        {
            return View();
        }

        [HttpPost]
        public ActionResult codigoBarras(string LinhaDigitavel, string Vencimento, string Valor)
        {
            DateTime dataInicial = Convert.ToDateTime("07/10/1997");
            string varLinhaDigitavel = "";
            string varCodigoBarras = "";

            varLinhaDigitavel = LinhaDigitavel.Replace(".", "").Replace(" ", "");

            if (LinhaDigitavel.Length == 47 || LinhaDigitavel.Length == 48)
            {
                if (LinhaDigitavel.Substring(0, 1) == "8") //Concessionárias e IPTU
                {
                    varCodigoBarras = varLinhaDigitavel.Substring(0, 11) +
                                varLinhaDigitavel.Substring(12, 11) +
                                varLinhaDigitavel.Substring(24, 11) +
                                varLinhaDigitavel.Substring(36, 11);

                    if (varLinhaDigitavel.Length == 48 && calculaDV2(varCodigoBarras) == true)
                    {
                        ViewBag.LinhaDigitavel = varLinhaDigitavel.Substring(0, 12) + " " +
                                                    varLinhaDigitavel.Substring(12, 12) + " " +
                                                    varLinhaDigitavel.Substring(24, 12) + " " +
                                                    varLinhaDigitavel.Substring(36, 12);

                        ViewBag.Vencimento = "";
                        ViewBag.Valor = varCodigoBarras.Substring(4, 11);
                    }
                    else
                    {
                        ViewBag.LinhaDigitavel = "Linha do Boleto inválida!";
                    }
                }
                else //Bloqueto de Cobrança
                {

                    varCodigoBarras = varLinhaDigitavel.Substring(0, 3) + //Código do Banco favorecido (0, 3)
                              varLinhaDigitavel.Substring(3, 1) + //Código da Moeda (3, 1)
                              varLinhaDigitavel.Substring(32, 1) + //DV do Código de Barras (32, 1)
                              varLinhaDigitavel.Substring(33, 4) + //Fator de Vencimento - ex.: 01/05/2002 (33, 4) - Data Base Febraban 
                              varLinhaDigitavel.Substring(37, 10) +  //Valor do Título (37, 10)
                              varLinhaDigitavel.Substring(4, 5) + //Campo Livre - Parte I (4, 5)
                              varLinhaDigitavel.Substring(10, 10) + //Campo Livre - Parte II (10, 10)
                              varLinhaDigitavel.Substring(21, 10); //Campo Livre - Parte III (21, 10)

                    if (varLinhaDigitavel.Replace(".", "").Replace(" ", "").Length == 47 && calculaDV(varCodigoBarras) == true)
                    {
                        ViewBag.LinhaDigitavel = varLinhaDigitavel.Substring(0, 5) + "." +
                                                  varLinhaDigitavel.Substring(5, 5) + " " +
                                                  varLinhaDigitavel.Substring(10, 5) + "." +
                                                  varLinhaDigitavel.Substring(15, 6) + " " +
                                                  varLinhaDigitavel.Substring(21, 5) + "." +
                                                  varLinhaDigitavel.Substring(26, 6) + " " +
                                                  varLinhaDigitavel.Substring(32, 1) + " " +
                                                  varLinhaDigitavel.Substring(33, 14);

                        ViewBag.Vencimento = dataInicial.AddDays(Convert.ToInt32(varLinhaDigitavel.Substring(33, 4)));
                        ViewBag.Valor = varLinhaDigitavel.Substring(37, 10);
                    }
                    else
                    {
                        ViewBag.LinhaDigitavel = "Linha do Boleto inválida!";
                    }
                }
            }
            else
            {
                ViewBag.LinhaDigitavel = "Linha do Boleto inválida!";
            }

            return View();
        }

        public bool calculaDV(string codigoBarras) //Valida Código de Barras do Boleto
        {
            string codigoSemDv = codigoBarras.Substring(0, 4) + codigoBarras.Substring(5, 39);

            Int32 DvCodigoBarras = Convert.ToInt32(codigoBarras.Substring(4, 1));
            Int32 DvCalculado = 0;
            Int32 Acumulador = 0;
            Int32 mult = 2;
            for (int i = 42; i >= 0; i--)
            {
                Acumulador += (Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult);
                if (mult < 9)
                {
                    mult += 1;
                }
                else
                {
                    mult = 2;
                }
            }

            DvCalculado = 11 - (Acumulador % 11);
            if (DvCalculado == 10)
            {
                DvCalculado = 0;
            }
            if (DvCalculado == DvCodigoBarras)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool calculaDV2(string codigoBarras) //Valida Código de Barras Concessionárias e IPTU/ISS
        {
            string codigoSemDv = codigoBarras.Substring(0, 3) + codigoBarras.Substring(4, 40);

            Int32 DvCodigoBarras = Convert.ToInt32(codigoBarras.Substring(3, 1));
            Int32 DvCalculado = 0;
            Int32 Acumulador = 0;
            Int32 mult = 2;
            for (int i = 42; i >= 0; i--)
            {

                if ((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult) > 9)
                {
                    Acumulador += Convert.ToInt32((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult).ToString().Substring(0, 1));
                    Acumulador += Convert.ToInt32((Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult).ToString().Substring(0, 2));
                }
                else
                {
                    Acumulador += (Convert.ToInt32(codigoSemDv.Substring(i, 1)) * mult);
                }
                if (mult == 2)
                {
                    mult -= 1;
                }
                else
                {
                    mult = 2;
                }
            }

            DvCalculado = 10 - (Acumulador % 10);
            if (DvCalculado==10)
            {
                DvCalculado = 0;
            }

            if (DvCalculado == DvCodigoBarras)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

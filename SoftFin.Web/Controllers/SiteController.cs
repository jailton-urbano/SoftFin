using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoftFin.Web.CallApi.CallApi;

namespace SoftFin.Web.Controllers
{
    public class SiteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tools()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Email(string nome, string email, string message)
        {
            new Email().EnviarMensagem(ConfigurationManager.AppSettings["EmailComercial"].ToString(),
                "SoftFin Contato : " + nome 
                + " - " 
                + email, 
                message);

            
            new Log().PostComercialAsync(new Infrastructure.DTO.DTOLogComercial
            {
                IP = Request.UserHostAddress,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Email = email
            });

            return Json("OK", JsonRequestBehavior.AllowGet);
        }



        public JsonResult EmailPedido(string email)
        {
            new Email().EnviarMensagem(email,
                "Retorno de contato Softfin",
                "Obrigado por entrar em contato com a Softfin. Em breve, um de nossos especialistas retornará o contato.",
                false);


            new Log().PostComercialAsync(new Infrastructure.DTO.DTOLogComercial
            {
                IP = Request.UserHostAddress,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Email = email
            });

            return Json("OK", JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult LeadEmail(string email)
        {
            new Email().EnviarMensagem(ConfigurationManager.AppSettings["EmailComercial"].ToString(),
                "SoftFin Lead Email : " + email,
                email);


            new Log().PostComercialAsync(new Infrastructure.DTO.DTOLogComercial
            {
                IP = Request.UserHostAddress,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Email = email
            });

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LeadContato(string nome, string email, string telefone, string assunto, string mensagem, string empresa)
        {

            new Email().EnviarMensagem(ConfigurationManager.AppSettings["EmailComercial"].ToString(),
                "SoftFin Lead Contato: " + nome,
                "Nome: " + nome + " </br>" +
                "Email: " + email + " </br>" +
                "Empresa: " + empresa + " </br>" +
                "Telefone: " + telefone + " </br>" +
                "Assunto: " + assunto + " </br>" +
                "Mensagem: " + mensagem + " </br>" ,
                true
                );


            new Log().PostComercialAsync(new Infrastructure.DTO.DTOLogComercial
            {
                IP = Request.UserHostAddress,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Email = email,
                Nome = nome,
                Telefone = telefone,
                Assunto = assunto,
                Mensagem = mensagem, 
                NomeEmpresa = empresa,
                Local = "LeadContato"

            });

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult LeadExperimente(string nome, string email, string telefone, string senha, string temempresa)
        {

            new Email().EnviarMensagem(ConfigurationManager.AppSettings["EmailComercial"].ToString(),
                "SoftFin Lead Contato : " + nome,
                "Nome: " + nome + " </br>" +
                "Email: " + email + " </br>" +
                "Senha: " + senha + " </br>" +
                "Telefone: " + telefone + " </br>" +
                "TemEmpresa: " + temempresa + " </br>" ,
                true
                );


            new Log().PostComercialAsync(new Infrastructure.DTO.DTOLogComercial
            {
                IP = Request.UserHostAddress,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Email = email,
                Nome = nome,
                Telefone = telefone,
                Senha = senha,
                TemEmpresa = temempresa,
                Local = "LeadExperimente"

            });

            return Json("OK", JsonRequestBehavior.AllowGet);
        }


        public JsonResult LeadPrincipal(string nome, string email, string telefone, string empresa)
        {

            new Email().EnviarMensagem(ConfigurationManager.AppSettings["EmailComercial"].ToString(),
                "SoftFin Lead Principal : " + nome,
                "Nome: " + nome + " </br>" +
                "Email: " + email + " </br>" +
                "Empresa: " + empresa + " </br>" +
                "Telefone: " + telefone + " </br>" ,
                true
                );


            new Log().PostComercialAsync(new Infrastructure.DTO.DTOLogComercial
            {
                IP = Request.UserHostAddress,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Email = email,
                Nome = nome,
                Telefone = telefone,
                NomeEmpresa = empresa,
                Local = "LeadPrincipal"

            });

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public JsonResult CalcularNota(int empresas, int estabelecimentos, int bancomes, int cpagmes, int nfsmes, int nfemes)
        {

            decimal valorempresas = 50 * empresas;
            decimal valorestabelecimentos = 15 * estabelecimentos;
            decimal valorbancomes = Decimal.Parse("0,55") * bancomes;
            decimal valorcpagmes = Decimal.Parse("1,50") * cpagmes;
            decimal valornfsmes = Decimal.Parse("2,75") *  nfsmes;
            decimal valornfemes = Decimal.Parse("3,50") *  nfemes;
            //cloud + suporte + setup + 99.99
            decimal valoroutros = 30 + 30 + 50;

            decimal soma = (valorempresas +
                            valorestabelecimentos +
                            valorbancomes +
                            valorcpagmes +
                            valornfsmes +
                            valornfemes +
                            valoroutros);
            decimal somaTotal = soma + 350;

            var qtd = 1;
            if (soma < (1080))
            {
                soma = 1080 ;
                somaTotal = 1080 + 350;
            }
            else
            {
                decimal porA = (Decimal.Parse("3639,38") * Decimal.Parse("0,80"));

                var porB = porA;

                while (porB < somaTotal)
                {
                    qtd += 1;
                    porB = porA * qtd;
                }
            }


            decimal assistente = 1810;
            decimal refeicao = 440;
            decimal transporte = 176;
            decimal custos = Decimal.Parse("863,38");
            decimal total = ((assistente + refeicao + transporte + custos) * qtd) + Decimal.Parse("350");

            return Json(new
            {
                ladoA = new {
                    Assistente = ((assistente) * qtd).ToString("n"),
                    Refeicao = ((refeicao) * qtd).ToString("n"),
                    Transporte = ((transporte) * qtd).ToString("n"),
                    Custos = ((custos) * qtd).ToString("n"),
                    Total = (total).ToString("n")
                },
                orcamento = soma.ToString("n"),
                orcamentoTotal = somaTotal.ToString("n")
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult login()
        {
            Acesso.Deslogar();
            return View();
        }

        
        public ActionResult contato()
        {
            return View();
        }

        public ActionResult funcionalidade()
        {
            return View();
        }

        public ActionResult noticias()
        {
            return View();
        }

        public ActionResult parceiros()
        {
            return View();
        }

        public ActionResult quem_somos()
        {
            return View();
        }
        public ActionResult simplicidade()
        {
            return View();
        }
        public ActionResult simule()
        {
            return View();
        }



    }
}

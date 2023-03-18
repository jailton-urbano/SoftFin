
using SoftFin.Infrastructure.DTO;
using SoftFin.Infrastructure.Models;
using SoftFin.InfrastructureHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SoftFin.Infrastructure.Controllers
{
    public class ComercialController : ApiController
    {
        [Route("Comercial/Incluir")]
        [HttpPost]
        public string Incluir(DTOLogComercial dtoLogComercial)
        {
            try
            {
                var erro = "";

                if (string.IsNullOrWhiteSpace(dtoLogComercial.CodigoSistema))
                {
                    erro += "(EXC001) Informe o Código do Sistema;";
                }
                if (string.IsNullOrWhiteSpace(dtoLogComercial.Estabelecimento))
                {
                    erro += "(EXC002) Informe o Estabelecimento;";
                }

                if (erro != "")
                {
                    return erro;
                }

                var logComercial = new LogComercial();
                logComercial.Data = dtoLogComercial.Data;
                logComercial.DataRecebida = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                logComercial.CodigoSistema = dtoLogComercial.CodigoSistema;
                logComercial.Estabelecimento = dtoLogComercial.Estabelecimento;
                logComercial.IP = dtoLogComercial.IP;
                logComercial.IPLogado = GetClientIp();
                logComercial.Nome = dtoLogComercial.Nome;
                logComercial.NomeEmpresa = dtoLogComercial.NomeEmpresa;
                logComercial.Senha = dtoLogComercial.Senha;
                logComercial.Telefone = dtoLogComercial.Telefone;
                logComercial.TemEmpresa = dtoLogComercial.TemEmpresa;
                logComercial.Email =  dtoLogComercial.Email;
                logComercial.Assunto = dtoLogComercial.Assunto;
                logComercial.Mensagem = dtoLogComercial.Mensagem;
                logComercial.Local = dtoLogComercial.Local;
                logComercial.Incluir();
                return "OK";
            }
            catch (Exception ex)
            {
                return "NOK;" + ex.Message;
            }


        }


        private string GetClientIp()
        {
            var request = Request;


            return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;


        }



    }
}
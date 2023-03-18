using SoftFin.Infrastructure.DTO;
using SoftFin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace SoftFin.Infrastructure.Controllers
{
    public class ImportarController : ApiController
    {
        [Route("Importacao/OK")]
        [HttpGet]
        public string Incluir()
        {
            return "OK";
        }

        [Route("Importacao/Incluir")]
        [HttpPost]
        public string Incluir(DTOLogImportar dTOLogImporta)
        {
            try
            {
                var erro = "";

                if (string.IsNullOrWhiteSpace(dTOLogImporta.CodigoSistema))
                {
                    erro += "(EXC001) Informe o Código do Sistema;";
                }
                if (string.IsNullOrWhiteSpace(dTOLogImporta.Estabelecimento))
                {
                    erro += "(EXC002) Informe o Estabelecimento;";
                }
                if (string.IsNullOrWhiteSpace(dTOLogImporta.Usuario))
                {
                    erro += "(EXC003) Informe o Usuário;";
                }

                if (erro != "")
                {
                    return erro;
                }

                var log = new LogImportar();
                log.Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                log.Agrupador = dTOLogImporta.Agrupador;
                log.CodigoSistema = dTOLogImporta.CodigoSistema;
                log.Descricao = dTOLogImporta.Descricao;
                log.Estabelecimento = dTOLogImporta.Estabelecimento;
                log.Registro = dTOLogImporta.Registro;
                log.Ip = dTOLogImporta.Ip;
                log.Json = dTOLogImporta.Json;
                log.Tipo = dTOLogImporta.Tipo;
                log.Mensagem = dTOLogImporta.Mensagem;
                log.IPLogado = GetClientIp();

                log.Usuario = dTOLogImporta.Usuario;
                log.Incluir();

                return "OK";

            }
            catch(Exception ex)
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
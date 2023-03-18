
using SoftFin.Infrastructure.DTO;
using SoftFin.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SoftFin.Infrastructure.Controllers
{
    public class EventoController : ApiController
    {
        [Route("Envento/Incluir")]
        [HttpPost]
        public string Incluir(DTOLogEvento dtoLogEvento)
        {
            try
            {
                var erro = "";

                if (string.IsNullOrWhiteSpace(dtoLogEvento.CodigoSistema))
                {
                    erro += "(EXC001) Informe o Código do Sistema;";
                }
                if (string.IsNullOrWhiteSpace(dtoLogEvento.Estabelecimento))
                {
                    erro += "(EXC002) Informe o Estabelecimento;";
                }
                if (string.IsNullOrWhiteSpace(dtoLogEvento.Usuario))
                {
                    erro += "(EXC003) Informe o Usuário;";
                }

                if (erro != "")
                {
                    return erro;
                }

                var logEvento = new LogEvento();
                logEvento.Data = dtoLogEvento.Data;
                logEvento.DataRecebida = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                logEvento.Agrupador = dtoLogEvento.Agrupador;
                logEvento.CodigoSistema = dtoLogEvento.CodigoSistema;
                logEvento.Descricao = dtoLogEvento.Descricao;
                logEvento.Estabelecimento = dtoLogEvento.Estabelecimento;
                logEvento.Exception = dtoLogEvento.Exception;
                logEvento.Ip = dtoLogEvento.Ip;
                logEvento.Json = dtoLogEvento.Json;
                logEvento.IPLogado = GetClientIp();
                logEvento.Rotina = dtoLogEvento.Rotina;
                logEvento.Tipo = dtoLogEvento.Tipo;
                logEvento.CadeiaMetodo = dtoLogEvento.CadeiaMetodo;
                logEvento.Usuario = dtoLogEvento.Usuario;
                logEvento.Incluir();
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
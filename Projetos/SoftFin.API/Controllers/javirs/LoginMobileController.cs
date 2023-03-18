using SoftFin.API.DTO;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SoftFin.API.Controllers.javirs
{
    public class dtoLogin: BaseDTORetorno
    {
        public string token { get; set; }
    }

    public class LoginMobileController : BaseApi
    {


        public dtoLogin login(string usuario_name, string usuario_password)
        {
            dtoLogin obj = new dtoLogin();
            try
            {
                DbControle db = new DbControle();
                var usuario = new Usuario().ObterPorCodigoAtivo(usuario_name, db);
                _paramBase.estab_id = new Estabelecimento().ObterPorCodigoPrimeiroEstab(_paramBase.usuario_name).id;

                if (usuario.senha != usuario_password)
                {
                    obj.status = "NOK";
                    var item = new API.DTO.DTOException();
                    item.codigo = "LMC0002";
                    item.descricao = "Password or User Name is invalid";
                    item.tipo = "ERROR";
                    obj.exceptions.Add(item);
                }
                else
                {
                    if (usuario.usuarioBloqueado)
                    {
                        obj.status = "NOK";
                        var item = new API.DTO.DTOException();
                        item.codigo = "LMC0003";
                        item.descricao = "User account is locked";
                        item.tipo = "ERROR";
                        obj.exceptions.Add(item);
                    }

                }
                if (obj.exceptions.Count() == 0)
                {
                    if (string.IsNullOrEmpty(usuario.tokenApi))
                    {
                        usuario.tokenApi = SoftFin.Utils.Crypto.Encryption(
                                    usuario.id.ToString() + ";" + DateTime.Now.ToString("o"));

                        usuario.Alterar(usuario, _paramBase, db);
                    }
                    obj.token = usuario.tokenApi;
                    obj.status = "OK";
                }
            }
            catch (Exception ex)
            {
                
                obj.status = "NOK";
                var item = new API.DTO.DTOException();
                item.codigo = "LMC0001";
                item.descricao = ex.Message;
                item.tipo = "ERROR";
                obj.exceptions.Add(item);
            }

            return obj;

        }
    }
}

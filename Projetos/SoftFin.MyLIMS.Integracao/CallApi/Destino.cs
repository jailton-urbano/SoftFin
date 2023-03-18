using Newtonsoft.Json;
using SoftFin.API.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.MyLIMS.Integracao.CallApi
{
    public class Destino: BaseMyLIMS
    {
        string _uriService = "";
        public Destino()
        {
            _uriService = ConfigurationManager.AppSettings["uriApiSoftFin"];
        }

        public bool GetContratoPorCodigo(string codigo)
        {
            var ret = base.GetSync<DTOGenericoRetorno<DTOContrato>>(_uriService + "/Contrato/GetContratoPorCodigo?codigo=" +
                codigo + "&" + Identificador(), 
                false);
            return ret.objs.Codigo != null;
        }

        private string Identificador()
        {
            return "codigoEstab=" + ConfigurationManager.AppSettings["estab"].ToString(); ;
        }

        public DTOGenericoRetorno<DTOContrato> PostContrato(DTOContrato dTOContrato)
        {
            dTOContrato.CodigoEstab = ConfigurationManager.AppSettings["estab"].ToString(); ;
            string objSerializado = JsonConvert.SerializeObject(dTOContrato);
            var ret = base.PostSync<DTOGenericoRetorno<DTOContrato>>(_uriService + "/Contrato/Incluir", objSerializado, false);
            return ret;
        }
        public DTOGenericoRetorno<DTOPessoa> PostPessoa(DTOPessoa dTOPessoa)
        {
            dTOPessoa.CodigoEstab = ConfigurationManager.AppSettings["estab"].ToString(); ;
            string objSerializado = JsonConvert.SerializeObject(dTOPessoa);
            var ret = base.PostSync<DTOGenericoRetorno<DTOPessoa>>(_uriService + "/Pessoa/Incluir", objSerializado, false);
            return ret;
        }

        internal DTOGenericoRetorno<List<DTOUnidadeNegocio>> GetUnidadeNegocio()
        {
            var codigoEstab = ConfigurationManager.AppSettings["estab"].ToString(); 

            var ret = base.GetSync<DTOGenericoRetorno<List<DTOUnidadeNegocio>>>(_uriService + "/UnidadeNegocio/ObterTodos?CodigoEstab=" + codigoEstab, false);
            return ret; ;
        }
        internal DTOGenericoRetorno<List<DTODeParaItem>> GetDeParaMyLimsAmostras()
        {
            var codigoEstab = ConfigurationManager.AppSettings["estab"].ToString();

            var ret = base.GetSync<DTOGenericoRetorno<List<DTODeParaItem>>>(_uriService + "/DeParaItem/ObterTodos?CodigoEstab=" + codigoEstab + "&CodigoDePara='MyLimsAmostras'", false);
            return ret; ;
        }
        internal DTOGenericoRetorno<List<DTODeParaItem>> GetDeParaMyLimsPrecos()
        {
            var codigoEstab = ConfigurationManager.AppSettings["estab"].ToString();

            var ret = base.GetSync<DTOGenericoRetorno<List<DTODeParaItem>>>(_uriService + "/DeParaItem/ObterTodos?CodigoEstab=" + codigoEstab + "&CodigoDePara='MyLimsPrecos'", false);
            return ret; ;
        }
    }
}

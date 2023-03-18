using Newtonsoft.Json;
using SoftFin.API.DTO;
using SoftFin.APIDTO;
using SoftFin.MyLIMS.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoftFin.MyLIMS.DTO.Works;

namespace SoftFin.MyLIMS.CallApi
{
    public class IntegracaoController : BaseMyLIMS
    {

        public void Integracao()
        {
            var processoIntegrado = Guid.NewGuid();
            try
            {
                var wks = new CallApi.Origem().GetWorks();
                APIDTO.DTOGenericoRetorno<List<API.DTO.DTOContrato>> DTOContratos = ConvertMyLimsToSoftFin(wks, processoIntegrado);
                ImportaSoftFin(DTOContratos);

            }
            finally
            {
                GeraEmailResultado(processoIntegrado);
            }
        }

        private void GeraEmailResultado(Guid processoIntegrado)
        {
            throw new NotImplementedException();
        }

        private void ImportaSoftFin(DTOGenericoRetorno<List<DTOContrato>> dTOContratos)
        {
            throw new NotImplementedException();
        }

        private DTOGenericoRetorno<List<DTOContrato>> ConvertMyLimsToSoftFin(Works.RootObject wks, Guid processoIntegrado)
        {
            var retorno = new DTOGenericoRetorno<List<DTOContrato>>();
            retorno.objs = new List<DTOContrato>();
            retorno.status = "NOK";

            foreach (var item in wks.Result)
            {
                var dtoContrato = new DTOContrato();
                dtoContrato.ProcessoIntegrado = processoIntegrado;
                dtoContrato.Codigo = item.Identification;
                retorno.objs.Add(dtoContrato);
            }
            retorno.status = "OK";
            return retorno;
        }

        //public APIDTO.DTOGenericoRetorno<List<API.DTO.DTOContrato>> ConsultaNaoIntegrados(int QtdDias)
        //{
        //    var contratos = new CallApi.Negocio().GetContratos();
        //    List<API.DTO.DTOContrato> DTONaoImportados = ConvertMyLimToSoftFin(contratos);
        //    APIDTO.DTOGenericoRetorno<List<API.DTO.DTOContrato>> retorno = new APIDTO.DTOGenericoRetorno<API.DTO.DTOContrato>();

        //    retorno.objs = DTONaoImportados;
        //    return retorno;
        //}

        //public void IntegracaoManual(string CodigoContrato)
        //{
        //}

    }
}

using Newtonsoft.Json;
using SoftFin.MyLIMS.Integracao.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoftFin.MyLIMS.Integracao.DTO.Works;

namespace SoftFin.MyLIMS.Integracao.CallApi
{
    public class Origem: BaseMyLIMS
    {
        string _uriService = "";
        public Origem()
        {
            _uriService = ConfigurationManager.AppSettings["uriApiMyLims"];
        }

        public String CheckConnection()
        {
            var ret = base.GetSync<string>(_uriService + "api/v2/checkConnection", true);
            return ret; 
        }

        public Works.RootObject GetWorks()
        {
            var ret = base.GetSync<Works.RootObject>(_uriService + "api/v2/works", true);
            return ret;
        }

        public Works.RootObject GetWorksRegraCliente()
        {
            var ret = base.GetSync<Works.RootObject>(_uriService + "api/v2/works?$filter=ReferenceKey eq null and WorkConclusion/Identification eq 'Aprovada'", true);
            return ret;
        }
        public Works.RootObject GetWorksRegra()
        {
            var ret = base.GetSync<Works.RootObject>(_uriService + "api/v2/works?$filter=ControlNumber eq 'PC488/2018'", true);
            return ret;
        }

        public Works.RootObject GetWorksfilterControlNumber(string controlNumber)
        {
            var ret = base.GetSync<Works.RootObject>(_uriService + "api/v2/works?$filter=ControlNumber eq '" + controlNumber +  "'", true);
            return ret;
        }

        public void PutMarcaReferenceKey(string workId, string referenceKey)
        {
            var url = _uriService + "api/v2/works/{workId}/UpdateReferenceKey";
            var ret = base.PutSync<Works.RootObject>(url.Replace("workId", workId),"{ReferenceKey:'"+ referenceKey + "'}", true);
            //return ret;
        }


        public RootObject WorkClasses()
        {
            var ret = base.GetSync<RootObject>(_uriService + "api/v2/workclasses",true);
            return ret;
        }

        public workitem WorksInfo(string Idwork)
        {
            var servico = "api/v2/works/{Idwork}/infos".Replace("{Idwork}", Idwork);
            var ret = base.GetSync<workitem>(_uriService + servico, true);
            return ret;
        }

        public DTO.WorksSamples.RootObject WorksSamples(string Idwork)
        {
            var servico = "api/v2/works/{Idwork}/Samples?$filter=Sample/CurrentStatus/SampleStatus/Id eq 8".Replace("{Idwork}", Idwork);
            var ret = base.GetSync<DTO.WorksSamples.RootObject>(_uriService + servico, true);
            return ret;
        }
        public string WorkTypesClasses(string workTypeId)
        {
            var servico = "api/v2/WorkTypes/{workTypeId}/Classes".Replace("{workTypeId}", workTypeId);
            var ret = base.GetSync<string>(_uriService + servico, true);
            return ret;
        }

        public string WorkTypes()
        {
            var servico = "api/v2/WorkTypes";
            var ret = base.GetSync<string>(_uriService + servico, true);
            return ret;
        }

        public string WorkSubclasses()
        {
            var servico = "api/v2/worksubclasses";
            var ret = base.GetSync<string>(_uriService + servico, true);
            return ret;
        }

        public DTO.workprices.workprice WorksPriceItens(string Idwork)
        {
            var servico = "api/v2/works/{Idwork}/priceitems".Replace("{Idwork}", Idwork);
            var ret = base.GetSync<DTO.workprices.workprice>(_uriService + servico, true);
            return ret;
        }

        public DTO.workprices.workprice RelationShipAccounts(string Idwork)
        {
            var servico = "api/v2/relationshipaccounts".Replace("{Idwork}", Idwork);
            var ret = base.GetSync<DTO.workprices.workprice>(_uriService + servico, true);
            return ret;
        }

        internal DTO.CollectorPoint.RootObject GetCollectionPoint(string collectionpointId)
        {
            var servico = "api/v2/collectionpoints/{collectionpointId}".Replace("{collectionpointId}", collectionpointId);
            var ret = base.GetSync<DTO.CollectorPoint.RootObject>(_uriService + servico, true);
            return ret;
        }

        internal DTO.Address.RootObject GetAccountAddresses(string accountId)
        {
            var servico = "api/v2/accounts/{accountId}/addresses".Replace("{accountId}", accountId);
            var ret = base.GetSync<DTO.Address.RootObject>(_uriService + servico, true);
            return ret;
        }

        internal DTO.SumPrice.RootObject SamplesSummaryPrices(string idAmostra)
        {
            var servico = "api/v2/Samples/{idAmostra}/summaryprices".Replace("{idAmostra}", idAmostra);
            var ret = base.GetSync<DTO.SumPrice.RootObject>(_uriService + servico, true);
            return ret;
        }
        public DTO.Account.RootObject GetAccounts(string Idwork)
        {
            var servico = "api/v2/works/{Idwork}/accounts".Replace("{Idwork}", Idwork);
            var ret = base.GetSync<DTO.Account.RootObject>(_uriService + servico, true);
            return ret;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Adianta.CallApi
{
    public class Negocio: BaseAdianta
    {
        string uriAdianta = "https://api-demo.adianta.com.br/v1/";
        public Task<String> token(string cnpj)
        {
            var ret = base.Get<string>(uriAdianta + "cadastro/token/" + cnpj );
            return ret;
        }
        public Task<String> register(string cnpj, DTO.DTOEmpresa dto)
        {
            var jsonstr =  JsonConvert.SerializeObject(dto);
            var ret = base.Post<string>(uriAdianta + "cadastro/register/" + cnpj, jsonstr);
            return ret;
        }
        public Task<bool> docsUpload(string token, string tipoDocumento, FileInfo fileInfo)
        {

            var ret = base.PostFile("cadastro/docs/" + tipoDocumento, token, fileInfo);
            return ret;
        }
        public Task<Stream> usageTermsDownload(string token)
        {
            var ret = base.GetFile("cadastro/usageTerms", token);
            return ret;
        }
    }
}

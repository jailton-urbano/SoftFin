using Newtonsoft.Json;
using SoftFin.GestorProcessos.Helper;
using SoftFin.GestorProcessos.Comum.DTO;
using SoftFin.GestorProcessos.Comum.Param;
using SoftFin.GestorProcessos.Models;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SoftFin.GestorProcessos.API.Controllers
{
    [RoutePrefix("api/ProcessoArquivo")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProcessoArquivoController : ApiController
    {

        [System.Web.Http.HttpPost, Route("Obter")]
        public string Obter(ParamProcesso paramProcesso)
        {
            var db = new DBGPControle();
            var aux = paramProcesso.CodigoProcessoAtual;
            var listaAux = db.ProcessoArquivo
                .Where(p => p.CodigoEmpresa == paramProcesso.CodigoEmpresa && p.Processo.Codigo == aux);

            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            var registroNovo = JsonConvert.SerializeObject(listaAux, settings);
            return registroNovo;
        }

        [System.Web.Http.HttpPost, Route("Salvar")]
        public string Salvar(ParamProcesso param)
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count == 0)
                return "Arquivo não envidado";

            for (int i = 0; i < httpRequest.Files.Count; i++)
            {
                var arquivo = httpRequest.Files[i];
                string[] extensionarquivos = new string[] { ".doc", ".docx", ".pdf", ".txt", ".jpeg", ".jpg", ".png", ".xls", ".xlsx" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return "Extenção não permitida (.doc, .docx, .pdf, .txt, .jpeg, .jpg, .png, .xls, .xlsx)";
                    }

                    // var db = new DBGPControle();

                    // var sistemaArquivo = new ProcessoArquivo();

                    // if (sistemaArquivo.ObterPorNomeDeArquivo("Proposta", codigo, arquivo.FileName).Count() > 0)
                    // {
                    //     return "Arquivo já incluido";
                    //}

                }
            }

            for (int i = 0; i < httpRequest.Files.Count; i++)
            {
                var arquivo = httpRequest.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var uploadPath = HttpContext.Current.Server.MapPath("~/TXTTemp/");
                    Directory.CreateDirectory(uploadPath);

                    var nomearquivonovo = arquivo.FileName;

                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                    arquivo.SaveAs(caminhoArquivo);
                    var sistemaArquivo = new ProcessoArquivo();
                    sistemaArquivo.Codigo = Guid.NewGuid().ToString();
                    AzureStorage.UploadFile(caminhoArquivo,
                                "ProcessoExecucao/" + param.CodigoEmpresa + "/" + sistemaArquivo.Codigo + "/" + nomearquivonovo,
                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                    var db = new DBGPControle();


                    sistemaArquivo.Codigo = Guid.NewGuid().ToString();
                    sistemaArquivo.ArquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                "ProcessoExecucao/" + param.CodigoEmpresa + "/" + sistemaArquivo.Codigo + "/" + nomearquivonovo;
                    sistemaArquivo.ArquivoOriginal = nomearquivonovo;
                    sistemaArquivo.RotinaOwner = "Proposta";
                    sistemaArquivo.Codigo = Guid.NewGuid().ToString();
                    sistemaArquivo.Tamanho = arquivo.ContentLength;
                    sistemaArquivo.ArquivoExtensao = nomearquivonovo.Substring(nomearquivonovo.IndexOf("."));
                    sistemaArquivo.CodigoEmpresa = param.CodigoEmpresa;
                    sistemaArquivo.Descricao = param.Resultado;
                    db.ProcessoArquivo.Add(sistemaArquivo);
                    db.SaveChanges();

                }
            }
            return "OK";

        }
    }
}
using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class AtendimentoController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new Atendimento();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["usuario"] = item.usuario;
                myExport["codigoAtendimento"] = item.codigoAtendimento;
                myExport["dataAbertura"] = item.dataAbertura;
                myExport["dataFechamento"] = item.dataFechamento;
                myExport["descricao"] = item.descricao;
                myExport["Pessoa"] = item.Pessoa.nome;
                myExport["AtendimentoCategoria"] = item.AtendimentoCategoria.descricao;
                myExport["AtendimentoStatus"] = item.AtendimentoStatus.descricao;
                myExport["NomeArquivoAnexo"] = item.NomeArquivoAnexo;
                myExport["NomeArquivoAnexoSistema"] = item.NomeArquivoAnexoSistema;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "Atendimento.csv");
        }

        public ActionResult Index()
        {
            CarregaViewData();
            var obj = new Atendimento();
            return View(obj);
        }
        public ActionResult Listas(JqGridRequest request)
        {
              string Valorid = Request.QueryString["id"]; 
              string Valorusuario = Request.QueryString["usuario"]; 
              string ValorcodigoAtendimento = Request.QueryString["codigoAtendimento"]; 
              string ValordataAbertura = Request.QueryString["dataAbertura"]; 
              string ValordataFechamento = Request.QueryString["dataFechamento"]; 
              string Valordescricao = Request.QueryString["descricao"]; 
              string Valorestabelecimento_id = Request.QueryString["estabelecimento_id"]; 
              string ValorEstabelecimento = Request.QueryString["Estabelecimento"]; 
              string Valorpessoas_ID = Request.QueryString["pessoas_ID"]; 
              string ValorPessoa = Request.QueryString["Pessoa"]; 
              string Valoratendimentocategoria_id = Request.QueryString["atendimentocategoria_id"]; 
              string ValorAtendimentoCategoria = Request.QueryString["AtendimentoCategoria"]; 
              string Valoratendimentostatus_id = Request.QueryString["atendimentostatus_id"]; 
              string ValorAtendimentoStatus = Request.QueryString["AtendimentoStatus"]; 
              string ValorNomeArquivoAnexo = Request.QueryString["NomeArquivoAnexo"]; 
              string ValorNomeArquivoAnexoSistema = Request.QueryString["NomeArquivoAnexoSistema"]; 
            int totalRecords = 0;
            Atendimento obj = new Atendimento();
            var objs = new Atendimento().ObterTodos(_paramBase);
            if (!String.IsNullOrEmpty(Valorid)) 
            {
              int aux ;
              int.TryParse(Valorid, out aux);
              objs = objs.Where(p => p.id == aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valorusuario)) 
            {
              objs = objs.Where(p => p.usuario.Contains(Valorusuario)).ToList();
            }
            if (!String.IsNullOrEmpty(ValorcodigoAtendimento)) 
            {
                int aux;
                int.TryParse(ValorcodigoAtendimento, out aux);
                objs = objs.Where(p => p.codigoAtendimento.Equals(aux)).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataAbertura)) 
            {
              DateTime aux ;
              DateTime.TryParse(ValordataAbertura, out aux);
              objs = objs.Where(p => p.dataAbertura == aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValordataFechamento)) 
            {
              DateTime aux ;
              DateTime.TryParse(ValordataFechamento, out aux);
              objs = objs.Where(p => p.dataFechamento == aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valordescricao)) 
            {
              objs = objs.Where(p => p.descricao.Contains(Valordescricao)).ToList();
            }
            if (!String.IsNullOrEmpty(Valorestabelecimento_id)) 
            {
              int aux ;
              int.TryParse(Valorestabelecimento_id, out aux);
              objs = objs.Where(p => p.estabelecimento_id == aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valorpessoas_ID)) 
            {
              int aux ;
              int.TryParse(Valorpessoas_ID, out aux);
              objs = objs.Where(p => p.pessoas_ID == aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valoratendimentocategoria_id)) 
            {
              int aux ;
              int.TryParse(Valoratendimentocategoria_id, out aux);
              objs = objs.Where(p => p.atendimentocategoria_id == aux).ToList();
            }
            if (!String.IsNullOrEmpty(Valoratendimentostatus_id)) 
            {
              int aux ;
              int.TryParse(Valoratendimentostatus_id, out aux);
              objs = objs.Where(p => p.atendimentostatus_id == aux).ToList();
            }
            if (!String.IsNullOrEmpty(ValorNomeArquivoAnexo)) 
            {
              objs = objs.Where(p => p.NomeArquivoAnexo.Contains(ValorNomeArquivoAnexo)).ToList();
            }
            if (!String.IsNullOrEmpty(ValorNomeArquivoAnexoSistema)) 
            {
              objs = objs.Where(p => p.NomeArquivoAnexoSistema.Contains(ValorNomeArquivoAnexoSistema)).ToList();
            }
            objs = Organiza(request, objs);
            totalRecords = objs.Count();
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
            foreach (var item in objs)
            {
                string dataFechamento = "";
                if (item.dataFechamento != null)
                    dataFechamento = item.dataFechamento.ToString();

                string pessoa = "";
                if (item.Pessoa != null)
                    pessoa = item.Pessoa.nome;
                    
                    

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    pessoa,
                    item.AtendimentoCategoria.descricao,
                    item.dataAbertura.ToShortDateString(),
                    item.AtendimentoStatus.descricao,
                    dataFechamento,
                    item.titulo,

                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        private static List<Atendimento> Organiza(JqGridRequest request, List<Atendimento> objs)
        {
            switch (request.SortingName)
            {
            case "usuario":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.usuario).ToList();
               else
                   objs = objs.OrderBy(p => p.usuario).ToList();
               break;
            case "codigoAtendimento":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.codigoAtendimento).ToList();
               else
                   objs = objs.OrderBy(p => p.codigoAtendimento).ToList();
               break;
            case "dataAbertura":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.dataAbertura).ToList();
               else
                   objs = objs.OrderBy(p => p.dataAbertura).ToList();
               break;
            case "dataFechamento":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.dataFechamento).ToList();
               else
                   objs = objs.OrderBy(p => p.dataFechamento).ToList();
               break;
            case "descricao":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.descricao).ToList();
               else
                   objs = objs.OrderBy(p => p.descricao).ToList();
               break;
            case "estabelecimento_id":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.estabelecimento_id).ToList();
               else
                   objs = objs.OrderBy(p => p.estabelecimento_id).ToList();
               break;
            case "Estabelecimento":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.Estabelecimento).ToList();
               else
                   objs = objs.OrderBy(p => p.Estabelecimento).ToList();
               break;
            case "pessoas_ID":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.Pessoa.nome).ToList();
               else
                   objs = objs.OrderBy(p => p.Pessoa.nome).ToList();
               break;
            case "Pessoa":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.Pessoa  ).ToList();
               else
                   objs = objs.OrderBy(p => p.Pessoa).ToList();
               break;
            case "atendimentocategoria_id":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.atendimentocategoria_id).ToList();
               else
                   objs = objs.OrderBy(p => p.atendimentocategoria_id).ToList();
               break;
            case "AtendimentoCategoria":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.AtendimentoCategoria).ToList();
               else
                   objs = objs.OrderBy(p => p.AtendimentoCategoria).ToList();
               break;
            case "atendimentostatus_id":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.AtendimentoStatus.descricao).ToList();
               else
                   objs = objs.OrderBy(p => p.AtendimentoStatus.descricao).ToList();
               break;
            case "AtendimentoStatus":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.AtendimentoStatus.descricao).ToList();
               else
                   objs = objs.OrderBy(p => p.AtendimentoStatus.descricao).ToList();
               break;
            case "NomeArquivoAnexo":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.NomeArquivoAnexo).ToList();
               else
                   objs = objs.OrderBy(p => p.NomeArquivoAnexo).ToList();
               break;
            case "NomeArquivoAnexoSistema":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.NomeArquivoAnexoSistema).ToList();
               else
                   objs = objs.OrderBy(p => p.NomeArquivoAnexoSistema).ToList();
               break;
            case "titulo":
               if (request.SortingOrder == JqGridSortingOrders.Desc)
                   objs = objs.OrderByDescending(p => p.titulo).ToList();
               else
                   objs = objs.OrderBy(p => p.titulo).ToList();
               break;
             }
             return objs;
          }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Atendimento obj)
        {
            try
            {
                int estab = _paramBase.estab_id;
                obj.estabelecimento_id = estab;
                obj.NomeArquivoAnexo = "";
                obj.codigoAtendimento = obj.ObterUltimoCodigo(_paramBase);
                obj.usuario = Acesso.UsuarioLogado();
                obj.atendimentostatus_id = new AtendimentoStatus().StatusEmAberto(_paramBase);
                CarregaViewData();

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase arquivo = Request.Files[i];
                    if (arquivo.FileName != "")
                    {
                        string[] extensionarquivos = new string[] { ".jpg", ".jpeg", ".xls", ".xlsx", ".doc", ".docx", ".rar", ".zip", ".pdf", ".txt" };
                        if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                        {
                            ViewBag.msg = "Impossivel salvar, extenção não permitida";
                            return View(obj);
                        }
                        obj.NomeArquivoAnexo = arquivo.FileName;
                    }
                }
                
                if (ModelState.IsValid)
                {

                    if (obj.Incluir(_paramBase))
                    {
                        if (obj.NomeArquivoAnexo != "")
                        {
                            HttpPostedFileBase arquivo = Request.Files[0];

                            if (arquivo.ContentLength > 0)
                            {
                                var uploadPath = Server.MapPath("~/AtendimentoArquivos/");
                                Directory.CreateDirectory(uploadPath);
                                uploadPath = Server.MapPath("~/AtendimentoArquivos/" + obj.id.ToString() + "/");
                                Directory.CreateDirectory(uploadPath);

                                string caminhoArquivo = Path.Combine(@uploadPath, Path.GetFileName(arquivo.FileName));

                                arquivo.SaveAs(caminhoArquivo);
                                AzureStorage.UploadFile(caminhoArquivo, RetornaS3Atendimento(obj.id, arquivo.FileName), ConfigurationManager.AppSettings["StorageAtendimento"].ToString());

                            }

                        }
                        ViewBag.msg = "Incluído com sucesso codigo: " + obj.codigoAtendimento.ToString();

                        //new Email().EnviarMensagem("xricardosantos@gmail.com", "Novo Atendimento", ViewBag.msg);

                    }
                    else
                    {
                        ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                throw ex;
            }
        }

        private static string RetornaS3Atendimento(int id, string arquivo)
        {
            return "Atendimento/" + id.ToString() + "/" + arquivo;
        }

        private static string RetornaS3AtendimentoItem(int idAtendimento, int iditem, string arquivo)
        {
            var x = "Atendimento/" + idAtendimento.ToString() + "/" + iditem.ToString() + "/" + arquivo;
            Debug.Print(x);
            return x;
        }

        

        public ActionResult Create()
        {
            try
            {
            CarregaViewData();
            var obj = new Atendimento();
            obj.dataAbertura = DateTime.Now;
            obj.usuario = Acesso.UsuarioLogado();


            return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Atendimento obj)
        {
            try
            {
                CarregaViewData();



                
                if (obj.Save(obj, _paramBase))
                {
                    ViewBag.msg = "Alterado com sucesso";
                    return Edit(obj.id);
                }
                else
                {
                    ViewBag.msg = "Impossivel alterar, registro excluído";
                    return Edit(obj.id);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Edit(int ID)
        {
            try
            {
                CarregaViewData();
                Atendimento obj = new Atendimento();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);

                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        public ActionResult Detail(int ID, ParamBase pb)
        {
            try
            {

                Atendimento obj = new Atendimento();
                obj = obj.ObterPorId(ID, _paramBase);
                
                ViewData["Historicos"] = new AtendimentoHistorico().ObterPorAtendimento(ID,pb);

                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Atendimento obj)
        {
            try
            {
            CarregaViewData();
            string erro = "";
            if (obj.Excluir(_paramBase))
            {
                ViewBag.msg = "Excluido com sucesso";
                return RedirectToAction("/Index");
            }
            else
            {
                ViewBag.msg = erro;
                return View(obj);
            }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        public ActionResult Delete(int ID)
        {
            try
            {
            CarregaViewData();
            Atendimento obj = new Atendimento();
            obj = obj.ObterPorId(ID, _paramBase);
            SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);

            return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }
        private void CarregaViewData()
        {
            ViewData["pessoas"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "razao");
            ViewData["atendimentocategoria"] = new SelectList(new AtendimentoCategoria().ObterTodos(_paramBase), "id", "descricao");
            ViewData["atendimentostatus"] = new SelectList(new AtendimentoStatus().ObterTodos(_paramBase), "id", "descricao");
        }
        private void CarregaViewDataHistorico()
        {
            ViewData["atendimentohistorico"] = new SelectList(new AtendimentoTipoHistorico().ObterTodos(_paramBase), "id", "descricao");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Historico(AtendimentoHistorico obj)
        {
            try
            {
                CarregaViewDataHistorico();
                int estab = _paramBase.estab_id;
                obj.estabelecimento_id = estab;
                obj.usuario = _paramBase.usuario_name;
                obj.NomeArquivoAnexo = "";

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase arquivo = Request.Files[i];
                    string[] extensionarquivos = new string[] { ".jpg", ".jpeg", ".xls", ".xlsx", ".doc", ".docx", ".rar", ".zip", ".pdf", ".txt" };
                    if (arquivo.FileName != "")
                    {
                        if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                        {
                            ViewBag.msg = "Impossivel salvar, extenção não permitida";
                            return View(obj);
                        }
                        obj.NomeArquivoAnexo = Request.Files[i].FileName.ToString();
                    }
                }

                if (ModelState.IsValid)
                {

                    if (obj.Incluir(_paramBase))
                    {
                        int arquivosSalvos = 0;
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            HttpPostedFileBase arquivo = Request.Files[i];

                            if (arquivo.ContentLength > 0)
                            {
                                var uploadPath = Server.MapPath("~/AtendimentoArquivos/");
                                Directory.CreateDirectory(uploadPath);
                                uploadPath = Server.MapPath(RetornaRootAtendimento(obj));
                                Directory.CreateDirectory(uploadPath);
                                uploadPath = Server.MapPath("~/AtendimentoArquivos/" + obj.estabelecimento_id.ToString() + "/" + obj.id.ToString() + "/");
                                Directory.CreateDirectory(uploadPath);

                                string caminhoArquivo = Path.Combine(@uploadPath, Path.GetFileName(arquivo.FileName));

                                arquivo.SaveAs(caminhoArquivo);

                                AzureStorage.UploadFile(caminhoArquivo, RetornaS3AtendimentoItem(obj.atendimento_id, obj.id, arquivo.FileName), ConfigurationManager.AppSettings["StorageAtendimento"].ToString());

                                arquivosSalvos++;
                            }
                        }

                        ViewBag.msg = "Incluído com sucesso " ;
                    }
                    else
                    {
                        ViewBag.msg = "Impossivel salvar, já este registro já esta cadastrado";
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        private static string RetornaRootAtendimento(AtendimentoHistorico obj)
        {
            return "~/AtendimentoArquivos/" + obj.estabelecimento_id.ToString() + "/";
        }


        public ActionResult Historico(int id)
        {
            try
            {
                CarregaViewDataHistorico();
                var obj = new AtendimentoHistorico();
                obj.atendimento_id = id;
                obj.usuario = Acesso.UsuarioLogado();
                obj.data = DateTime.Now;
                

                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }
        }

        public ActionResult Download(int id)
        {
            try
            {
                Atendimento obj = new Atendimento();
                obj = obj.ObterPorId(id, _paramBase);

                if (obj.NomeArquivoAnexo != "")
                {
                    var arqst = AzureStorage.DownloadFile(RetornaS3Atendimento(id, obj.NomeArquivoAnexo), ConfigurationManager.AppSettings["StorageAtendimento"]);
                    return File(arqst, "application/force-download", obj.NomeArquivoAnexo);
                }
                else
                {
                    return RedirectToAction("/Index", "Erros");

                }
               
                
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        public ActionResult DownloadItem(int id)
        {
            try
            {
                var obj = new AtendimentoHistorico();
                obj = obj.ObterPorId(id,_paramBase);

                if (obj.NomeArquivoAnexo != "")
                {
                    var arqst = AzureStorage.DownloadFile(RetornaS3AtendimentoItem(obj.atendimento_id, obj.id, obj.NomeArquivoAnexo),ConfigurationManager.AppSettings["StorageAtendimento"].ToString());
                    return File(arqst, "application/force-download", obj.NomeArquivoAnexo);
                }
                else
                {
                    return RedirectToAction("/Index", "Erros");

                }

                
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("/Index", "Erros");
            }

        }

        
    }
}

//using Lib.Web.Mvc.JQuery.JqGrid;
//using SoftFin.Web.Classes;
//using SoftFin.Web.Models;
//using SoftFin.Web.Negocios;
//using SoftFin.Utils;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace SoftFin.Web.Controllers
//{
//    public class ContratoNewController : BaseController
//    {
//        int contrato_id = 0;
//        public ActionResult Index()
//        {
//            return View();
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Listas(JqGridRequest request)
//        {

//            int totalRecords = 0;


//            string Contrato = Request.QueryString["Contrato"];
//            int estab = pb.estab_id;

//            if (string.IsNullOrEmpty(Contrato))
//                totalRecords = new Contrato().ObterTodos().Count();
//            else
//                totalRecords = new Contrato().ObterTodos().Where(x => x.contrato == Contrato || x.descricao.Contains(Contrato)).Count();

//            //Fix for grouping, because it adds column name instead of index to SortingName
//            //string sortingName = "cdg_perfil";

//            //Prepare JqGridData instance
//            JqGridResponse response = new JqGridResponse()
//            {
//                //Total pages count
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                //Page number
//                PageIndex = request.PageIndex,
//                //Total records count
//                TotalRecordsCount = totalRecords
//            };
//            //Table with rows data

//            List<Contrato> lista = new List<Models.Contrato>();

//            if (string.IsNullOrEmpty(Contrato))
//                lista = new Contrato().ObterTodos().OrderBy(p => p.Pessoa.nome).Skip(12 * request.PageIndex).Take(12).ToList();
//            else
//                lista = new Contrato().ObterTodos().Where(p => p.estabelecimento_id == estab && (p.contrato == Contrato || p.descricao.Contains(Contrato))).OrderBy(p => p.Pessoa.nome).Skip(12 * request.PageIndex).Take(12).ToList();

//            foreach (var item in lista)
//            {


//                string nome = "";
//                if (item.Pessoa != null)
//                    nome = item.Pessoa.nome;

//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.contrato,
//                    item.descricao,
//                    nome,
//                    item.prazo,
//                    item.valortotal
//                }));
//            }


//            //Return data as json
//            return new JqGridJsonResult() { Data = response };
//        }

//        public ActionResult Excel()
//        {

//            Contrato obj = new Contrato();
//            List<Contrato> lista = new Contrato().ObterTodos();

//            CsvExport myExport = new CsvExport();
//            foreach (var contrato in lista)
//            {
//                myExport.AddRow();
//                myExport["Contrato"] = contrato.contrato;
//                myExport["Descricao"] = contrato.descricao;
//                myExport["Emissao"] = contrato.emissao;
//                myExport["Cliente"] = contrato.Pessoa.nome;
//                //myExport["Pedido"] = contrato.pedido;
//                myExport["Prazo"] = contrato.prazo;
//                //myExport["Status"] = contrato.ParcelaContratos;
//                //myExport["Tipo"] = contrato.TipoContrato.tipo;
//                //myExport["Unidade"] = contrato.UnidadeNegocio.unidade;
//                myExport["Valor Total"] = contrato.valortotal;

//            }


//            //Exporta os dados para o Excel
//            string myCsv = myExport.Export();
//            byte[] myCsvData = myExport.ExportToBytes();

//            //Abri o arquivo de retorno no Excel
//            return File(myCsvData, "application/vnd.ms-excel", "Contrato.csv");
//        }

//        [HttpPost]
//        public ActionResult Create(Contrato obj)
//        {
//            try
//            {
//                CarregaViewData();
//                DateTime dat = new DateTime();

//                if (DateTime.TryParse(obj.emissao.ToString(), out dat) == false)
//                {
//                    ModelState.AddModelError("", "Data Inválida");
//                    return View(obj);
//                }

//                if (ModelState.IsValid)
//                {
//                    obj.estabelecimento_id = pb.estab_id;
//                    Contrato contrato = new Contrato();

//                    int estab = pb.estab_id;
//                    obj.estabelecimento_id = estab;

//                    if (obj.Incluir() == true)
//                    {
//                        ViewBag.msg = "Contrato incluído com sucesso";
//                        return RedirectToAction("Edit", new { id = obj.id });
//                    }
//                    else
//                    {


//                        ViewBag.msg = "Contrato já cadastrado";
//                        return View(obj);
//                    }
//                }
            

//                String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
//                                                            .Select(v => v.ErrorMessage + " " + v.Exception));

//                ModelState.AddModelError("", messages);
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Create()
//        {
//            try
//            {
//                CarregaViewData();
//                var obj = new Contrato();
//                obj.emissao = DateTime.Now;
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        private void CarregaViewData()
//        {
//            if (contrato_id !=0 )
//                ViewData["ContratoItem"] = new  ContratoItem().ObterTodos().Where(p => p.contrato_id == contrato_id).ToList();
            
//            ViewData["TipoContrato"] = new SelectList(new TipoContrato().ObterTodos(), "id", "tipo");
//            ViewData["ClienteContrato"] = new SelectList(new Pessoa().ObterTodos(_paramBase), "id", "nome");
//            ViewData["UnidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(), "id", "unidade");
//            ViewData["Contrato"] = new SelectList(new Contrato().ObterTodos(), "id", "contrato");
            
//        }

//        [HttpPost]
//        public ActionResult Edit(Contrato obj)
//        {
//            try
//            {
//                DateTime dat = new DateTime();
//                contrato_id = obj.id;

//                if (DateTime.TryParse(obj.emissao.ToString(), out dat) == false)
//                {
//                    ModelState.AddModelError("", "Data Inválida");
//                    return View(obj);
//                }
//                if (ModelState.IsValid)
//                {
//                    CarregaViewData();
//                    obj.estabelecimento_id = pb.estab_id;
//                    Contrato contr = new Contrato();
//                    int estab = pb.estab_id;
//                    obj.estabelecimento_id = estab;
//                    if (contr.Alterar(obj))
//                    {

//                        ViewBag.msg = "Contrato alterado com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Não foi possivel alterar o contrato";
//                        return View(obj);
//                    }

//                }
//                else
//                {
//                    ModelState.AddModelError("", "Dados Invalidos");
//                    return View(obj);

//                }


//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Edit(int ID)
//        {
//            try
//            {
//                contrato_id = ID;

//                Contrato obj = new Contrato();
//                obj = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//                CarregaViewData();

//                return View(obj);

//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        [HttpPost]
//        public ActionResult Delete(int id, Contrato obj, )
//        {
//            CarregaViewData();

//            try
//            {
//                string Erro = "";
//                if (!obj.Excluir(obj.id, ref Erro, _paramBase))
//                {
//                    ViewBag.msg = Erro;
//                    obj = obj.ObterPorId(obj.id);
//                    return View(obj);
//                }
//                else
//                {
//                    return RedirectToAction("/Index");
//                }
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }
//        public ActionResult Delete(int ID)
//        {
//            try
//            {
//                CarregaViewData();
//                Contrato obj = new Contrato();
//                Contrato ct = obj.ObterPorId(ID);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(ct);
//                return View(ct);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        public ActionResult Detail(int ID)
//        {
//            try
//            {
//                contrato_id = ID;
//                ViewData["objs"] = new ContratoArquivo().ObterPorContrato(ID);
//                Contrato obj = new Contrato();
//                obj = obj.ObterPorId(ID);

//                CarregaViewData();

//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult ListasDetail(JqGridRequest request)
//        {

//            int totalRecords = 0;

//            int ident = int.Parse(Request.QueryString["id"]);

//            totalRecords = new ParcelaContrato().ObterTodos().Where(p => p.ContratoItem.contrato_id== ident).Count();

//            //Fix for grouping, because it adds column name instead of index to SortingName
//            //string sortingName = "cdg_perfil";

//            //Prepare JqGridData instance
//            JqGridResponse response = new JqGridResponse()
//            {
//                //Total pages count
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                //Page number
//                PageIndex = request.PageIndex,
//                //Total records count
//                TotalRecordsCount = totalRecords
//            };
//            //Table with rows data

//            List<ParcelaContrato> lista = new List<ParcelaContrato>();

//            lista = new ParcelaContrato().ObterTodos().Where(p => p.ContratoItem.contrato_id == ident).OrderBy(p => p.data).Skip(12 * request.PageIndex).Take(12).ToList();

//            foreach (var item in lista)
//            {
//                string numContrato = "";
//                if (item.ContratoItem != null)
//                    numContrato = item.ContratoItem.Contrato.contrato;

//                string status = "";
//                if (item.statusParcela != null)
//                    status = item.statusParcela.status;

//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    numContrato,
//                    item.parcela,
//                    item.descricao,
//                    item.data.Date.ToString("dd/MM/yyyy"),
//                    item.valor.ToString("0.00"),
//                    status
//                }));
//            }


//            //Return data as json
//            return new JqGridJsonResult() { Data = response };
//        }


//        public ActionResult Arquivo(int id)
//        {
//            ViewData["id"] = id;
//             ViewData["objs"] = new ContratoArquivo().ObterPorContrato(id);
//             return View();
//        }


//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Arquivo(int id, FormCollection formCollection)
//        {
//            for (int i = 0; i < Request.Files.Count; i++)
//            {
//                HttpPostedFileBase arquivo = Request.Files[i];
//                string[] extensionarquivos = new string[] { ".doc", ".docx", ".pdf", ".txt", ".jpeg", ".jpg", ".png" };
//                if (arquivo.FileName != "")
//                {
//                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
//                    {
//                        ViewBag.msg = "Impossivel salvar, extenção não permitida";
//                        return RedirectToAction("Arquivo", new { id = id });
//                    }
//                }
//            }

//            var pessoa = new Pessoa();

//            for (int i = 0; i < Request.Files.Count; i++)
//            {
//                HttpPostedFileBase arquivo = Request.Files[i];

//                if (arquivo.ContentLength > 0)
//                {
//                    var uploadPath = Server.MapPath("~/TXTTemp/");
//                    Directory.CreateDirectory(uploadPath);

//                    var nomearquivonovo =  arquivo.FileName;

//                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

//                    arquivo.SaveAs(caminhoArquivo);

//                    AzureStorage.UploadFile(caminhoArquivo,
//                                "ContratoArquivo/" + id.ToString() + "/" + nomearquivonovo,
//                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

//                    var db = new DbControle();

//                    var contratoArquivo = new ContratoArquivo();
//                    contratoArquivo.arquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
//                                "ContratoArquivo/" + id.ToString() + "/" + nomearquivonovo;
//                    contratoArquivo.arquivoOriginal = nomearquivonovo;
//                    contratoArquivo.contrato_id = id;
//                    contratoArquivo.Salvar();
//                }
//            }
//            ViewBag.msg = "Alterado com sucesso ";
//            return RedirectToAction("Arquivo", new { id = id });
//        }


//        public ActionResult RemoveArquivo(int id)
//        {
//            var contratoArquivo = new ContratoArquivo().ObterPorId(id);


//            AzureStorage.DeleteFile(contratoArquivo.arquivoReal,
//                                "ContratoArquivo/" + contratoArquivo.contrato_id.ToString() + "/" + contratoArquivo.arquivoOriginal,
//                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

//            new ContratoArquivo().Excluir(id);

//            return RedirectToAction("Arquivo", new { id = contratoArquivo.contrato_id });
//        }
//    }
//}

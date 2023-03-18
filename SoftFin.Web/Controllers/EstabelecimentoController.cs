//using Lib.Web.Mvc.JQuery.JqGrid;
//using Sistema1.Classes;
//using Sistema1.Models;
//using SoftFin.Utils;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.IO;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Sistema1.Controllers
//{
//    public class EstabelecimentoController : BaseController
//    {

//        public ActionResult Excel()
//        {

//            var obj = new Estabelecimento();
//            var lista = obj.ObterTodos();

//            CsvExport myExport = new CsvExport();
//            foreach (var item in lista)
//            {
//                myExport.AddRow();

//                myExport["id"] = item.id;
//                myExport["Apelido"] = item.Apelido;
//                myExport["Codigo"] = item.Codigo;
//                myExport["NomeCompleto"] = item.NomeCompleto;
//                myExport["Logradouro"] = item.Logradouro;
//                myExport["NumeroLogradouro"] = item.NumeroLogradouro;
//                myExport["Complemento"] = item.Complemento;
//                myExport["BAIRRO"] = item.BAIRRO;
//                myExport["Municipio_id"] = item.Municipio_id;
//                myExport["CEP"] = item.CEP;
//                myExport["Empresa_id"] = item.Empresa_id;
//                myExport["CNPJ"] = item.CNPJ;
//                myExport["InscricaoEstadual"] = item.InscricaoEstadual;
//                myExport["InscricaoMunicipal"] = item.InscricaoMunicipal;

//            }


//            //Exporta os dados para o Excel
//            string myCsv = myExport.Export();
//            byte[] myCsvData = myExport.ExportToBytes();

//            return File(myCsvData, "application/vnd.ms-excel", "Estabelecimento.csv");
//        }

//        public ActionResult Index()
//        {
//            CarregaViewData();
//            return View(new Estabelecimento());

//        }

//        public ActionResult Listas(JqGridRequest request)
//        {
//            string Valorid = Request.QueryString["id"]; 
//            string ValorApelido = Request.QueryString["Apelido"]; 
//            string ValorCodigo = Request.QueryString["Codigo"]; 
//            string ValorNomeCompleto = Request.QueryString["NomeCompleto"]; 
//            string ValorLogradouro = Request.QueryString["Logradouro"]; 
//            string ValorNumeroLogradouro = Request.QueryString["NumeroLogradouro"]; 
//            string ValorComplemento = Request.QueryString["Complemento"]; 
//            string ValorBAIRRO = Request.QueryString["BAIRRO"]; 
//            string ValorMunicipio_id = Request.QueryString["Municipio_id"]; 
//            string ValorCEP = Request.QueryString["CEP"]; 
//            string ValorEmpresa_id = Request.QueryString["Empresa_id"]; 
//            string ValorCNPJ = Request.QueryString["CNPJ"]; 
//            string ValorInscricaoEstadual = Request.QueryString["InscricaoEstadual"]; 
//            string ValorInscricaoMunicipal = Request.QueryString["InscricaoMunicipal"]; 


//            int totalRecords = 0;
//            Estabelecimento obj = new Estabelecimento();
//            var objs = new Estabelecimento().ObterTodos();

//            if (!String.IsNullOrEmpty(ValorApelido)) 
//            {
//              objs = objs.Where(p => p.Apelido.Contains(ValorApelido)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorCodigo)) 
//            {
//              objs = objs.Where(p => p.Codigo.Contains(ValorCodigo)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorNomeCompleto)) 
//            {
//              objs = objs.Where(p => p.NomeCompleto.Contains(ValorNomeCompleto)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorLogradouro)) 
//            {
//              objs = objs.Where(p => p.Logradouro.Contains(ValorLogradouro)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorComplemento)) 
//            {
//              objs = objs.Where(p => p.Complemento.Contains(ValorComplemento)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorBAIRRO)) 
//            {
//              objs = objs.Where(p => p.BAIRRO.Contains(ValorBAIRRO)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorMunicipio_id)) 
//            {
//              int aux ;
//              int.TryParse(ValorMunicipio_id, out aux);
//              objs = objs.Where(p => p.Municipio_id.Equals(aux)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorCEP)) 
//            {
//              objs = objs.Where(p => p.CEP.Contains(ValorCEP)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorEmpresa_id)) 
//            {
//              int aux ;
//              int.TryParse(ValorEmpresa_id, out aux);
//              objs = objs.Where(p => p.Empresa_id.Equals(aux)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorCNPJ)) 
//            {
//              objs = objs.Where(p => p.CNPJ.Contains(ValorCNPJ)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorInscricaoEstadual)) 
//            {
//              objs = objs.Where(p => p.InscricaoEstadual.Contains(ValorInscricaoEstadual)).ToList();
//            }
//            if (!String.IsNullOrEmpty(ValorInscricaoMunicipal)) 
//            {
//              int aux ;
//              int.TryParse(ValorInscricaoMunicipal, out aux);
//              if (aux != 0)
//              objs = objs.Where(p => p.InscricaoMunicipal.Equals(aux)).ToList();
//            }



//            totalRecords = objs.Count();
//            JqGridResponse response = new JqGridResponse()
//            {
//                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
//                PageIndex = request.PageIndex,
//                TotalRecordsCount = totalRecords
//            };
//            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
//            foreach (var item in objs)
//            {
//                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
//                {
//                    item.id,
//                    item.Apelido,
//                    item.NomeCompleto
//                }));
//            }
//            return new JqGridJsonResult() { Data = response };
//        }

//        public ActionResult Create()
//        {
//            CarregaViewData();
//            return View(new Estabelecimento());
//        }

//        private void CarregaViewData()
//        {
//            ViewData["Municipio"] = new SelectList(new Municipio().ObterTodos(), "ID_MUNICIPIO", "DESC_MUNICIPIO");
//            ViewData["Empresa"] = new SelectList(new Empresa().ObterTodos(), "id", "Apelido");
//        }

//        [HttpPost]
//        public ActionResult Create(Estabelecimento obj)
//        {
//            try
//            {
//                CarregaViewData();
//                if (ModelState.IsValid)
//                {
//                    if (obj.Incluir())
//                    {
//                        ViewBag.msg = "Incluído com sucesso";
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Já cadastrado";
//                    }
//                }
//                return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }
                
//        public ActionResult Edit(int id = 0)
//        {
//            try
//            {
//                Estabelecimento estabelecimento = new Estabelecimento().ObterPorId(id);
//                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(estabelecimento);
//                if (estabelecimento == null)
//                {
//                    return HttpNotFound();
//                }

//                CarregaViewData();
//                return View(estabelecimento);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }

//        }


//        [HttpPost]
//        public ActionResult Edit(Estabelecimento obj)
//        {
//            try
//            {
//                CarregaViewData();
//                if (ModelState.IsValid)
//                {

//                    if (obj.Alterar())
//                    {
//                        ViewBag.msg = "Alterado com sucesso";
//                        return View(obj);
//                    }
//                    else
//                    {
//                        ViewBag.msg = "Impossivel alterar, registro excluído";
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

//        [HttpPost]
//        public ActionResult Delete(Estabelecimento obj)
//        {
//            try
//            {
//            CarregaViewData();
//            string erro = "";
//            if (obj.Excluir(ref erro))
//            {
//                ViewBag.msg = "Excluido com sucesso";
//                return RedirectToAction("/Index");
//            }
//            else
//            {
//                ViewBag.msg = erro;
//                return View(obj);
//            }
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
//            CarregaViewData();
//            Estabelecimento obj = new Estabelecimento();
//            obj = obj.ObterPorId(ID);
//            SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
//            return View(obj);
//            }
//            catch (Exception ex)
//            {
//                _eventos.Error(ex);
//                return RedirectToAction("/Index", "Erros");
//            }
//        }

//        public ActionResult Logo(int id)
//        {
//            ViewData["id"] = id;
//            var db = new DbControle();

//            var estabelecimento = new Estabelecimento().ObterPorId(id, db);
//            if (string.IsNullOrEmpty(estabelecimento.Logo))
//            {
//                ViewData["imagem"] = null;
//            }
//            else
//            {
//                ViewData["imagem"] = estabelecimento.Logo;
//            }

//            return View();
//        }


//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Logo(int id, string logopadrao)
//        {
//            if (logopadrao == "on")
//            {
//                var db = new DbControle();

//                var estabelecimento = new Estabelecimento().ObterPorId(id, db);
//                estabelecimento.Logo = null;
//                estabelecimento.Alterar(db);
//            }
//            else
//            {

//                for (int i = 0; i < Request.Files.Count; i++)
//                {
//                    HttpPostedFileBase arquivo = Request.Files[i];
//                    string[] extensionarquivos = new string[] { ".jpg", ".jpeg", ".png" };
//                    if (arquivo.FileName != "")
//                    {
//                        if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
//                        {
//                            ViewBag.msg = "Impossivel salvar, extenção não permitida";
//                            return RedirectToAction("Logo", new { id = id });
//                        }
//                    }
//                }

//                var estabelecimento = new Estabelecimento();

//                for (int i = 0; i < Request.Files.Count; i++)
//                {
//                    HttpPostedFileBase arquivo = Request.Files[i];

//                    if (arquivo.ContentLength > 0)
//                    {
//                        var uploadPath = Server.MapPath("~/Logo/");
//                        Directory.CreateDirectory(uploadPath);

//                        var nomearquivonovo = "Logo_" + id.ToString() + Path.GetExtension(arquivo.FileName);

//                        string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

//                        arquivo.SaveAs(caminhoArquivo);

//                        AzureStorage.UploadFile(caminhoArquivo,
//                                    "Logo/" + nomearquivonovo,
//                                    ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

//                        var db = new DbControle();

//                        estabelecimento = new Estabelecimento().ObterPorId(id, db);
//                        estabelecimento.Logo = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
//                                    "Logo/" + nomearquivonovo;
//                        estabelecimento.Alterar(db);

//                    }
//                }
//            }
            

            
            
//            ViewBag.msg = "Alterado com sucesso ";


//            return RedirectToAction("Logo", new { id = id });
//        }


//    }
//}
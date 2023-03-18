using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ContaCController : BaseController
    {
        // GET: ContaContabil


        public ActionResult Index()
        {
            return View("~/Views/ContaC/Index.cshtml");
            
        }



        public JsonResult ObterTodos()
        {
            var objs = new ContaContabil().ObterTodos(_paramBase);
            return Json(objs.Select(obj => new
            {
                obj.empresa_id,
                obj.id,
                obj.codigo,
                obj.descricao,
                obj.codigoSuperior,
                codsup = obj.codigoSuperior,
                obj.Tipo, 
                obj.CategoriaGeral,
                obj.SubCategoria,
                obj.IndicacaoPublicacao
            }), JsonRequestBehavior.AllowGet);
        }


        

        private string BuscaDescricaoConta(string conta)
        {
            if (conta == null)
                return "";
            var obj = new ContaContabil().ObterPorIdCodigo(conta, _paramBase, null);
            if (obj != null)
                return string.Format("{0} - {1}", obj.codigo, obj.descricao);
            else
                return "";

        }

        public JsonResult ObterPorId(int id)
        {
            var obj = new ContaContabil().ObterPorId(id, _paramBase);
            if (obj == null)
            {
                obj = new ContaContabil();
                obj.empresa_id = _empresa;
            }

            return Json(new
            {
                obj.id,
                obj.codigo,
                obj.empresa_id,
                obj.descricao,
                obj.codigoSuperior,
                obj.Tipo,
                obj.CategoriaGeral,
                obj.SubCategoria,
                obj.IndicacaoPublicacao
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Salvar(ContaContabil obj)
        {
            try
            {
                if (obj.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);

                var objErros = obj.Validar(ModelState);

                if (string.IsNullOrWhiteSpace(obj.codigo))
                {
                    objErros.Add("informe o código");
                }
                if (string.IsNullOrWhiteSpace(obj.descricao))
                {
                    objErros.Add("informe a descrição");
                }


                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", Erros = objErros });
                }

                if (obj.id == 0)
                {

                    if (obj.Incluir(_paramBase) == true)
                    {
                        return Json(new { CDStatus = "OK", DSMessage = "Incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Registro já cadastrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    obj.Alterar(obj, _paramBase);
                    return Json(new { CDStatus = "OK", DSMessage = "Registro alterado com sucesso" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult Excluir(ContaContabil obj)
        {

            try
            {
                var objAux = new ContaContabil().ObterPorId(obj.id, _paramBase);
                if (objAux == null)
                    return Json(new { CDMessage = "NOK", DSMessage = "Não encontrado" }, JsonRequestBehavior.AllowGet);
                if (objAux.empresa_id != _empresa)
                    return Json(new { CDMessage = "NOK", DSMessage = _mensagemTrocaAba }, JsonRequestBehavior.AllowGet);



                string erro = "";
                if (new ContaContabil().Excluir(objAux.id, ref erro, _paramBase))
                {
                    if (erro != "")
                        throw new Exception(erro);

                    return Json(new { CDStatus = "OK", DSMessage = "Registro excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir Registro" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult UploadECD()
        {

            if (Request.Files.Count == 0)
                return Json(new { CDStatus = "NOK", DSMessage = "Selecione pelo menos 1 arquivo." }, JsonRequestBehavior.AllowGet);

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".txt" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Extenção não permitida (.txt)" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }

            var pessoa = new Pessoa();


            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                var ccs = new List<ContaContabil>();

                if (arquivo.ContentLength > 0)
                {

                    var uploadPath = Server.MapPath("~/OFXTemp/");
                    Directory.CreateDirectory(uploadPath);

                    var path = Path.Combine(Server.MapPath("~/OFXTemp/"), arquivo.FileName);
                    arquivo.SaveAs(path);

                    var sourceStream = new System.IO.StreamReader(
                     System.IO.File.Open(path, FileMode.Open),
                     Encoding.GetEncoding("windows-1252"));


                    ccs = CarregaECD(sourceStream);

                }

                if (ccs.Count() != 0)
                {
                    var db = new DbControle();
                    foreach (var item in ccs)
                    {
                        
                        var ccedit = item.ObterPorIdCodigo(item.codigo, _paramBase, db);

                        if (ccedit == null)
                        {
                            item.Incluir(_paramBase,db);
                        }
                        else
                        {
                            ccedit.codigo = item.codigo;
                            ccedit.descricao = item.descricao;
                            ccedit.codigoSuperior = item.codigoSuperior;
                            ccedit.Alterar(_paramBase,db);
                        }

                    }
                }
            }
            return Json(new { CDStatus = "OK", DSMessage = "Incluido com sucesso " }, JsonRequestBehavior.AllowGet);

        }

        public List<ContaContabil> CarregaECD(StreamReader stream)
        {
            var Transactions = new List<ContaContabil>();

                while (!stream.EndOfStream)
                {
                    string temp = stream.ReadLine();
                    var splitTemp  = temp.Split('|');
                    if (splitTemp.Count() > 1)
                        if (splitTemp[1] == "I050")
                        {
                            var cc = new ContaContabil();
                            cc.empresa_id = _empresa;
                            cc.codigo = splitTemp[6];
                            cc.codigoSuperior = splitTemp[7];
                            cc.descricao = splitTemp[8];
                            Transactions.Add(cc);
                        }
                }

            return Transactions;
        }


    }
}
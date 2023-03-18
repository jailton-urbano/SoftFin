using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using SoftFin.NFe.DTO;

namespace SoftFin.Web.Controllers
{
	public class PessoaController : BaseController
	{
		//Pessoas
		public override JsonResult TotalizadorDash(int? id)
		{
			//base.TotalizadorDash(id);
			var soma = new Pessoa().ObterTodos(_paramBase).Count().ToString();
			return Json(new { CDStatus = "OK", Result = soma }, JsonRequestBehavior.AllowGet);

		}

		public ActionResult Excel()
		{
			var obj = new Pessoa();
			var lista = obj.ObterTodos(_paramBase);
			CsvExport myExport = new CsvExport();

			foreach (var item in lista)
			{
				myExport.AddRow();
				myExport["codigo"] = item.codigo;
				myExport["nome"] = item.nome;
				myExport["razao"] = item.razao;
				myExport["cnpj"] = item.cnpj;
				myExport["inscricao"] = item.inscricao;
				myExport["ccm"] = item.ccm;
				myExport["endereco"] = item.endereco;
				myExport["numero"] = item.numero;
				myExport["complemento"] = item.complemento;
				myExport["bairro"] = item.bairro;
				myExport["cidade"] = item.cidade;
				myExport["uf"] = item.uf;
				myExport["cep"] = item.cep;
				myExport["eMail"] = item.eMail;
				myExport["bancoConta"] = item.bancoConta;
				myExport["agenciaConta"] = item.agenciaConta;
				myExport["contaBancaria"] = item.contaBancaria;
				myExport["digitoContaBancaria"] = item.digitoContaBancaria;


                if (item.UnidadeNegocio == null)
                    myExport["UnidadeNegocio_ID"] = "";
                else
                    myExport["UnidadeNegocio_ID"] = item.UnidadeNegocio.unidade;

				if (item.TipoEndereco == null)
					myExport["TipoEndereco_ID"] = "";
				else
					myExport["TipoEndereco_ID"] = item.TipoEndereco.Descricao;

                if (item.CategoriaPessoa == null)
                    myExport["CategoriaPessoa_ID"] = "";
                else
                    myExport["CategoriaPessoa_ID"] = item.CategoriaPessoa.Descricao;

                if (item.TipoPessoa == null)
                    myExport["TipoPessoa_ID"] = "";
                else
                    myExport["TipoPessoa_ID"] = item.TipoPessoa.Descricao;


                myExport["TelefoneFixo"] = item.TelefoneFixo;
				myExport["Celular"] = item.Celular;
			}
			string myCsv = myExport.Export();
			byte[] myCsvData = myExport.ExportToBytes();
			return File(myCsvData, "application/vnd.ms-excel", "Pessoa.csv");
		}


		public ActionResult Import()
		{
			ViewData["Importado"] = null;
			return View();
		}

		[HttpPost]
		public ActionResult Import(HttpPostedFileBase file)
		{
			var importacaoArquivo = new ImportacaoArquivo();

			try
			{
				var arquivo = Request.Files[0];


				var uploadPath = Server.MapPath("~/OFXTemp/");
				Directory.CreateDirectory(uploadPath);

				var nomeFile = "Upload_CSV_Empresa_" + _empresa + "_Estab_" + _estab + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + "_" + arquivo.FileName;
				var path = Path.Combine(Server.MapPath("~/OFXTemp/"), nomeFile);
				arquivo.SaveAs(path);
				var utf8 = System.Text.Encoding.UTF8;

				var lines = System.IO.File.ReadAllLines(path, utf8).Select(a => a.Split(';'));

				importacaoArquivo.TotalLinhas = lines.Count();

				var linhaCont = 1;
				var db = new DbControle();

				foreach (var item in lines)
				{
					if (linhaCont == 1) //Pula o cabeçalho 
					{
						linhaCont += 1;
						continue;
					}

					var erro = "";
					var obj = new Pessoa();
					if (item.Length >= 24)
					{
						erro = Convert(item, obj);

						if (erro == "")
						{

							var ExistePessoa = new Pessoa().ObterPorCNPJ(obj.cnpj, _paramBase, db);

							if (ExistePessoa == null)
							{
								obj.Incluir(_paramBase);
							}
							else
							{
								ExistePessoa.agenciaConta = obj.agenciaConta;
								ExistePessoa.agenciaDigito = obj.agenciaDigito;
								ExistePessoa.bairro = obj.bairro;
								ExistePessoa.bancoConta = obj.bancoConta;
								ExistePessoa.CategoriaPessoa_ID = obj.CategoriaPessoa_ID;
								ExistePessoa.ccm = obj.ccm;
								ExistePessoa.Celular = obj.Celular;
								ExistePessoa.cep = obj.cep;
								ExistePessoa.cidade = obj.cidade;
								ExistePessoa.cnpj = obj.cnpj;
								ExistePessoa.codigo = obj.codigo;
								ExistePessoa.complemento = obj.complemento;
								ExistePessoa.contaBancaria = obj.contaBancaria;
								ExistePessoa.digitoContaBancaria = obj.digitoContaBancaria;
								ExistePessoa.eMail = obj.eMail;
								ExistePessoa.endereco = obj.endereco;
								ExistePessoa.inscricao = obj.inscricao;
								ExistePessoa.nome = obj.nome;
								ExistePessoa.numero = obj.numero;
								ExistePessoa.razao = obj.razao;
								ExistePessoa.TelefoneFixo = obj.TelefoneFixo;
								ExistePessoa.TipoEndereco_ID = obj.TipoEndereco_ID;
								ExistePessoa.TipoPessoa_ID = obj.TipoPessoa_ID;
								ExistePessoa.uf = obj.uf;
								ExistePessoa.UnidadeNegocio_ID = obj.UnidadeNegocio_ID;
								ExistePessoa.empresa_id = _empresa;
								ExistePessoa.Alterar(_paramBase, db);
							}
							importacaoArquivo.TotalImportadas += 1;
						}

					}
					else
					{
						erro = "Falta o preenchimento de colunas, você tem que estar com os 24 colunas na planilha e na sua planilha veio com " + item.Length.ToString() + " colunas.";
					}
					if (erro != "")
					{
						importacaoArquivo.TotalErros += 1;
						importacaoArquivo.LinhasErros.Add("Linha " + linhaCont.ToString(), erro);
					}

					linhaCont += 1;
				}
			}
			catch (Exception ex)
			{
				importacaoArquivo.Situacao = "Erro durante importação";
				importacaoArquivo.Descricao = ex.Message;
			}

			if (importacaoArquivo.Situacao == "")
			{
				importacaoArquivo.Situacao = "Importado com sucesso!";
				if (importacaoArquivo.LinhasErros.Count() > 0)
				{
					importacaoArquivo.Situacao += ", mas com alguns problemas";
				}
			}

			ViewData["Importado"] = "S";
			ViewData["ImportacaoArquivo"] = importacaoArquivo;
			return View(importacaoArquivo);
		}

		private string Convert(string[] obj, Pessoa item)
		{
			var erro = "";


			item.codigo = obj[0];
			item.nome = obj[1];
			item.razao = obj[2];
            item.cnpj = obj[3];

			if (item.cnpj == "")
			{
				erro += "[CNPJ não informado inválido]";
			}

            item.cnpj = item.cnpj.Replace("/", "").Replace("-", "").Replace(".", "");

            item.inscricao = obj[4];
			item.ccm = obj[5];
            if (!string.IsNullOrEmpty(item.ccm))
            {
                item.ccm = item.ccm.Replace("/", "").Replace("-", "").Replace(".", "");
            }


            item.endereco = obj[6];
			item.numero = obj[7];
			item.complemento = obj[8];

            if (item.complemento.Length > 50)
            {
                erro += "[Complemento maior do que 50 '" + item.complemento + "']";
            }

            item.bairro = obj[9];


            if (item.bairro.Length > 50)
            {
                erro += "[Bairro maior do que 50 '" + item.bairro + "']";
            }

            item.cidade = obj[10];


			item.uf = obj[11];
            if (item.uf != null)
                item.uf = item.uf.Trim();

            if (item.uf.Length > 2)
            {
                erro += "[UF maior do que 2 '"+ item.uf +"']";
            }

            item.cep = obj[12];

            if (item.cep != null)
                item.cep = item.cep.Trim();

            if (item.cep.Length > 9 )
            {
                erro += "[Cep maior do que 9'" + item.cep + "']";
            }

            item.eMail = obj[13];

			if (item.eMail == "")
				item.eMail = null;
			else
			{
				if (!IsValidEmail(item.eMail))
				{
					erro += "[Email inválido]";
				}
			}

			item.bancoConta = obj[14];

            if (!string.IsNullOrEmpty(item.bancoConta))
                if (item.bancoConta.Length > 3)
                {
                    erro += "[Código do banco é 3 digitos]";
                }


            item.agenciaConta = obj[15];
			item.contaBancaria = obj[16];
			item.digitoContaBancaria = obj[17];

			var undstr = obj[18];
			var und = new UnidadeNegocio().ObterTodos(_paramBase).Where(p => p.unidade == undstr).FirstOrDefault();
			if (und != null)
			{
				item.UnidadeNegocio_ID = und.id;
			}
			else
			{
				erro += "[Unidade de negócio não encontrada]";
			}

			var testr = obj[19];
			if (testr == "")
			{
				item.TipoEndereco_ID = new TipoEndereco().ObterTodos(_paramBase).FirstOrDefault().id;
			}
			else
			{
				var te = new TipoEndereco().ObterTodos(_paramBase).Where(p => p.Descricao == testr).FirstOrDefault();
				if (te != null)
				{
					item.TipoEndereco_ID = te.id;
				}
				else
				{
					erro += "[Tipo endereço não encontrada]";
				}
			}

			var castr = obj[20];
			var ca = new CategoriaPessoa().ObterTodos(_paramBase).Where(p => p.Descricao.ToLower() == castr.ToLower()).FirstOrDefault();
			if (ca != null)
			{
				item.CategoriaPessoa_ID = ca.id;
			}
			else
			{
				erro += "[Categoria Pessoa não encontrada]";
			}

			var tpstr = obj[21];
			var tp = new TipoPessoa().ObterTodos(_paramBase).Where(p => p.Descricao == tpstr).FirstOrDefault();
			if (tp != null)
			{
				item.TipoPessoa_ID = tp.id;
			}
			else
			{
				erro += "[Tipo de Pessoa não encontrada]";
			}


			item.TelefoneFixo = obj[22];
			item.Celular = obj[23];
			item.empresa_id = _empresa;
			return erro;
		}



		public static bool IsValidEmail(string emailAddress)
		{
			string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
				+ @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
				+ @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

			var x = new Regex(validEmailPattern, RegexOptions.IgnoreCase);
			bool isValid = x.IsMatch(emailAddress);

			return isValid;
		}

		public ActionResult Index()
		{

			return View();
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Listas(JqGridRequest request)
		{
			string Valorcodigo = Request.QueryString["codigo"];
			string Valornome = Request.QueryString["nome"];
			string Valorrazao = Request.QueryString["razao"];

			var objs = new Pessoa().ObterTodos(_paramBase).ToList();
			//lista = obj.listaCategoriaPessoa()();

			if (!String.IsNullOrEmpty(Valorcodigo))
			{
				objs = objs.Where(p => p.codigo.Contains(Valorcodigo)).ToList();
			}
			if (!String.IsNullOrEmpty(Valornome))
			{
				objs = objs.Where(p => p.nome.ToUpper().Contains(Valornome.ToUpper())).ToList();
			}
			if (!String.IsNullOrEmpty(Valorrazao))
			{
				objs = objs.Where(p => p.razao.ToUpper().Contains(Valorrazao.ToUpper())).ToList();
			}
			int totalRecords = objs.Count();

			//Fix for grouping, because it adds column name instead of index to SortingName
			//string sortingName = "cdg_perfil";
			objs = Organiza(request, objs);
			//Prepare JqGridData instance
			JqGridResponse response = new JqGridResponse()
			{
				//Total pages count
				TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
				//Page number
				PageIndex = request.PageIndex,
				//Total records count
				TotalRecordsCount = totalRecords
			};
			objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();

			//Table with rows data
			foreach (var item in
				objs)
			{
				response.Records.Add(new JqGridRecord(item.id.ToString(), new List<object>()
				{
					item.codigo,
					item.nome,
					item.razao
				}));
			}


			//Return data as json
			return new JqGridJsonResult() { Data = response };
		}

		private static List<Pessoa> Organiza(JqGridRequest request, List<Pessoa> objs)
		{
			switch (request.SortingName)
			{

				case "nome":
					if (request.SortingOrder == JqGridSortingOrders.Desc)
						objs = objs.OrderByDescending(p => p.nome).ToList();
					else
						objs = objs.OrderBy(p => p.nome).ToList();
					break;
				case "razao":
					if (request.SortingOrder == JqGridSortingOrders.Desc)
						objs = objs.OrderByDescending(p => p.razao).ToList();
					else
						objs = objs.OrderBy(p => p.razao).ToList();
					break;
				case "codigo":
					if (request.SortingOrder == JqGridSortingOrders.Desc)
						objs = objs.OrderByDescending(p => p.codigo).ToList();
					else
						objs = objs.OrderBy(p => p.codigo).ToList();
					break;
			}
			return objs;
		}

		public ActionResult ConsultaPessoa(string pessoa)
		{
			try
			{
				Pessoa ps = new Pessoa();
				Pessoa obj = ps.ObterTodos(_paramBase).Where(x => x.codigo == pessoa).FirstOrDefault();

				return View(obj);
			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return RedirectToAction("/Index", "Erros");
			}

		}

		//[HttpPost]
		//public ActionResult Create(Pessoa obj)
		//{
		//    try
		//    {
		//        if (ModelState.IsValid)
		//        {

		//            Pessoa pessoa = new Pessoa();
		//            int idempresa  = pb.empresa_id;
		//            obj.empresa_id = idempresa;
		//            var verEmpresa = ValidaEmpresaUsuarioString(obj.empresa_id);

		//            if (verEmpresa != "")
		//            {
		//                CarregaViewData();
		//                ViewBag.msg = verEmpresa;
		//                return View(obj);

		//            }

		//            if (pessoa.Incluir(obj) == true)
		//            {
		//                CarregaViewData();

		//                ViewBag.msg = "Pessoa incluída com sucesso";
		//                return View(obj);
		//            }
		//            else
		//            {
		//                CarregaViewData();

		//                ViewBag.msg = "Pessoa já cadastrada - verifique o codigo e o nome";
		//                return View(obj);
		//            }
		//        }
		//        else
		//        {
		//            CarregaViewData();
		//            String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
		//                                                       .Select(v => v.ErrorMessage + " " + v.Exception));

		//            ModelState.AddModelError("", messages);
		//            return View(obj);

		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        _eventos.Error(ex);
		//        return RedirectToAction("/Index", "Erros");
		//    }

		//}

		private void CarregaViewData()
		{
			ViewData["TipoPessoa"] = new SelectList(new TipoPessoa().ObterTodos(_paramBase), "id", "Descricao");

			ViewData["TipoEndereco"] = new SelectList(new TipoEndereco().ObterTodos(_paramBase), "id", "Descricao");

			ViewData["CategoriaPessoa"] = new SelectList(new CategoriaPessoa().ObterTodos(_paramBase), "id", "Descricao");

			ViewData["UnidadeNegocio"] = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
		}


		//public ActionResult Create()
		//{
		//    try
		//    {
		//        CarregaViewData();


		//        return View();
		//    }
		//    catch (Exception ex)
		//    {
		//        _eventos.Error(ex);
		//        return RedirectToAction("/Index", "Erros");
		//    }

		//}

		[HttpPost]
		public ActionResult Edit(Pessoa obj)
		{
			try
			{
				CarregaViewData();
				var verEmpresa = ValidaEmpresaUsuarioString(obj.empresa_id);

				if (verEmpresa != "")
				{
					ViewBag.msg = verEmpresa;
					return View(obj);
				}

				if (ModelState.IsValid)
				{
					int idempresa  = _paramBase.empresa_id;
					obj.empresa_id = idempresa;

					Pessoa pessoa = new Pessoa();
					pessoa.Alterar(obj, _paramBase);
					ViewBag.msg = "Pessoa alterada com sucesso";
					return View(obj);
				}
				else
				{
					String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
											   .Select(v => v.ErrorMessage + " " + v.Exception));
					ModelState.AddModelError("", messages);
					ModelState.AddModelError("", "Dados Invalidos");
					return View(obj);
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
				Pessoa pessoa = new Pessoa();
				pessoa = new Pessoa().ObterPorId(ID, _paramBase);
				SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(pessoa);
				return View(pessoa);
			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return RedirectToAction("/Index", "Erros");
			}

		}

		public ActionResult Detail(int ID)
		{
			try
			{
				return Delete(ID);
			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return RedirectToAction("/Index", "Erros");
			}

		}


		[HttpPost]
		public ActionResult Delete(int id, FormCollection collection)
		{
			try
			{
				CarregaViewData();

				if (new Banco().ObterPorId(id, _paramBase) != null)
				{


					Pessoa pessoa = new Pessoa();
					Pessoa obj2 = pessoa.ObterPorId(id, _paramBase);
					ViewBag.msg = "Impossível excluir, existe um Contrato associado a essa Pessoa";
					return View(obj2);
				}
				else
				{
					Pessoa pessoa = new Pessoa();
					Pessoa obj2 = pessoa.ObterPorId(id, _paramBase);


					var verEmpresa = ValidaEmpresaUsuarioString(obj2.empresa_id);

					if (verEmpresa != "")
					{
						CarregaViewData();
						ViewBag.msg = verEmpresa;
						return View(obj2);
					}

					string Erro = "";
					pessoa.Excluir(id,_paramBase,  ref Erro);
					if (Erro != "")
					{
						ViewBag.msg = Erro;
					}
					else
					{
						ViewBag.msg = "Pessoa excluída com sucesso";
					}
					return View(obj2);
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
				Pessoa obj = new Pessoa();
				Pessoa pessoa = obj.ObterPorId(ID, _paramBase);
				SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(pessoa);
				return View(pessoa);
			}
			catch (Exception ex)
			{
				_eventos.Error(ex);
				return RedirectToAction("/Index", "Erros");
			}

		}


		public ActionResult Foto(int id)
		{
			ViewData["id"] = id;
			var db = new DbControle();

			var pessoa = new Pessoa().ObterPorId(id,_paramBase, db);
			if (string.IsNullOrEmpty(pessoa.Foto))
			{
				ViewData["imagem"] = null;
			}
			else
			{
				ViewData["imagem"] = pessoa.Foto;
			}

			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Foto(int id, string Fotopadrao)
		{
			if (Fotopadrao == "on")
			{
				var db = new DbControle();

				var pessoa = new Pessoa().ObterPorId(id, _paramBase, db);
				pessoa.Foto = "";
				pessoa.Alterar(_paramBase, db);
			}
			else
			{

				for (int i = 0; i < Request.Files.Count; i++)
				{
					HttpPostedFileBase arquivo = Request.Files[i];
					string[] extensionarquivos = new string[] { ".jpg", ".jpeg", ".png" };
					if (arquivo.FileName != "")
					{
						if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
						{
							ViewBag.msg = "Impossivel salvar, extenção não permitida";
							return RedirectToAction("Foto", new { id = id });
						}
					}
				}

				var pessoa = new Pessoa();

				for (int i = 0; i < Request.Files.Count; i++)
				{
					HttpPostedFileBase arquivo = Request.Files[i];

					if (arquivo.ContentLength > 0)
					{
						var uploadPath = Server.MapPath("~/Foto/");
						Directory.CreateDirectory(uploadPath);

						var nomearquivonovo = "Foto_" + id.ToString() + Path.GetExtension(arquivo.FileName);

						string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

						arquivo.SaveAs(caminhoArquivo);

						AzureStorage.UploadFile(caminhoArquivo,
									"Foto/" + nomearquivonovo,
									ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

						var db = new DbControle();

						pessoa = new Pessoa().ObterPorId(id,_paramBase, db);
						pessoa.Foto = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
									"Foto/" + nomearquivonovo;
						pessoa.Alterar(_paramBase,db);

					}
				}
			}




			ViewBag.msg = "Alterado com sucesso ";


			return RedirectToAction("Foto", new { id = id });
		}

		public JsonResult ListaPessoas()
		{
			var data = new Pessoa().ObterTodos(_paramBase);
			var aux = data.Select(x => new
			{
				x.agenciaConta,
				x.agenciaDigito,
				x.bairro,
				x.bancoConta,
				x.CategoriaPessoa_ID,
				x.ccm,
				x.Celular,
				x.cep,
				x.cidade,
				x.cnpj,
				x.codigo,
				x.complemento,
				x.contaBancaria,
				x.digitoContaBancaria,
				x.eMail,
				x.empresa_id,
				x.endereco,
				x.Foto,
				x.id,
				x.inscricao,
				x.nome,
				x.numero,
				x.razao,
				x.TelefoneFixo,
				x.TipoEndereco_ID,
				x.TipoPessoa_ID,
				x.uf,
				x.UnidadeNegocio_ID,
				x.flgSegurado,
				x.perfilFamiliar,
				x.perfilPessoal,
				x.perfilProfissional,
				tipoEndereco = (x.TipoEndereco == null) ? "" : x.TipoEndereco.Descricao
			});


			return Json(aux, JsonRequestBehavior.AllowGet);
		}

		public JsonResult ListaTipoPessoas()
		{
			var data = new SelectList(new TipoPessoa().ObterTodos(_paramBase), "id", "Descricao");
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult ListaCategorias()
		{
			var data = new SelectList(new CategoriaPessoa().ObterTodos(_paramBase), "id", "Descricao");
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult ListaUnidades()
		{
			var data = new SelectList(new UnidadeNegocio().ObterTodos(_paramBase), "id", "unidade");
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult ListaTipoEnderecos()
		{
			var data = new SelectList(new TipoEndereco().ObterTodos(_paramBase), "id", "Descricao");
			return Json(data, JsonRequestBehavior.AllowGet);
		}

		public JsonResult ObterPessoaPorId(int id)
		{
			var data = new Pessoa().ObterPorId(id, _paramBase);
            var pcc = new PessoaContaContabil();

			if (data == null)
				data = new Pessoa();

            if (id == 0)
            {
                var ec = new EmpresaContaContabil().ObterPorEmpresa(_paramBase);

                if (ec == null)
                {
                    ec = new EmpresaContaContabil();                }
                else
                {
                    pcc.contaContabilDespesaPadrao_id = ec.ContaContabilTitulo_id;
                    pcc.contaContabilPagarPadrao_id = ec.ContaContabilPagamento_id;
                    pcc.contaContabilReceberPadrao_id = ec.ContaContabilRecebimento_id;
                }
            }
            else
            {
                pcc = new PessoaContaContabil().ObterPorId(id,_paramBase);
                if (pcc == null)
                {
                    pcc = new PessoaContaContabil();
                    var ec = new EmpresaContaContabil().ObterPorEmpresa(_paramBase);

                    if (ec == null)
                    {
                        ec = new EmpresaContaContabil();
                    }
                    else
                    {
                        pcc = new PessoaContaContabil();
                        pcc.contaContabilDespesaPadrao_id = ec.ContaContabilTitulo_id;
                        pcc.contaContabilPagarPadrao_id = ec.ContaContabilPagamento_id;
                        pcc.contaContabilReceberPadrao_id = ec.ContaContabilRecebimento_id;

                    }
                }
            }



            return Json(
               new
               {
                   CDMessage = "OK",
                   config = new
                   { pcc.pessoa_id,
                       pcc.contaContabilDespesaPadrao_id,
                       pcc.contaContabilPagarPadrao_id,
                       pcc.contaContabilReceberPadrao_id },
                   pessoa = new
                   {
                       data.Foto,
                       data.agenciaConta,
                       data.agenciaDigito,
                       data.bairro,
                       data.bancoConta,
                       CategoriaPessoa_ID = (data.CategoriaPessoa_ID == 0) ? "" : data.CategoriaPessoa_ID.ToString(),
                       data.ccm,
                       data.Celular,
                       cep = (data.cep == null) ? "" : data.cep.Replace("-", ""),
                       data.cidade,
                       cnpj = (data.cnpj == null) ? "" : data.cnpj.Replace(".", "").Replace("/", "").Replace("-", ""),
                       data.codigo,
                       data.complemento,
                       data.contaBancaria,
                       data.digitoContaBancaria,
                       data.eMail,
                       data.empresa_id,
                       data.endereco,
                       data.id,
                       data.inscricao,
                       data.nome,
                       data.numero,
                       data.razao,
                       data.TelefoneFixo,
                       TipoEndereco_ID = (data.TipoEndereco_ID == 0) ? "" : data.TipoEndereco_ID.ToString(),
                       TipoPessoa_ID = (data.TipoPessoa_ID == 0) ? "" : data.TipoPessoa_ID.ToString(),
                       data.uf,
                       UnidadeNegocio_ID = (data.UnidadeNegocio_ID == 0) ? "" : data.UnidadeNegocio_ID.ToString(),
                       data.flgSegurado,
                       data.perfilFamiliar,
                       data.perfilPessoal,
                       data.perfilProfissional,
                       contatos = new PessoaContato().ObterPorTodos(data.id, _paramBase).
                                Select(p => new { p.cargo, p.celular, p.email, p.id, p.nome, p.observacao, p.RecebeCobranca, p.telefone })
                   }
               }
			, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Salvar(Pessoa pessoa, PessoaContaContabil pcc)
		{
			try
			{
				var erroscpag = new List<string>();

				var erros = pessoa.Validar(ModelState);

                var removeErro = erros.Where(p => p == "Campo Código obrigatório").FirstOrDefault();

                if (removeErro != null)
                {
                    erros.Remove("Campo Código obrigatório");
                    pessoa.codigo = new Pessoa().ObterUltimoCodigoDisponivel(_paramBase);
                }


				if (erros.Count() > 0)
				{
					return Json(new { CDMessage = "NOK", DSMessage = "Campos Inválidos", Erros = erros }, JsonRequestBehavior.AllowGet);
				}

				DbControle banco = new DbControle();
				using (var dbcxtransaction = banco.Database.BeginTransaction())
				{
					pessoa.empresa_id = _empresa;
					if (pessoa.id == 0)
					{
						var exec = pessoa.Incluir(_paramBase,banco);
						if (!exec)
						{
							dbcxtransaction.Rollback();
							return Json(new { CDStatus = "NOK", DSMessage = "Codigo ou Pessoa já cadastrado" }, JsonRequestBehavior.AllowGet);
						}
						if (pessoa.contatos != null)
						{
							foreach (var item in pessoa.contatos)
							{
								item.pessoa_id = pessoa.id;
								new PessoaContato().Incluir(item, _paramBase, banco);
							}
						}

                        pcc.pessoa_id = pessoa.id;
                        pcc.Incluir(_paramBase, banco);

					}
					else
					{
						pessoa.Alterar(_paramBase,banco);
						var pessoacontatosExcluir = new PessoaContato().ObterPorTodos(pessoa.id, _paramBase, banco);
                         
						foreach (var item in pessoacontatosExcluir)
						{
							if (pessoa.contatos.Where(p => p.id ==item.id).Count() == 0)
							{
								var erroexclucao = "";
								new PessoaContato().Excluir(item.id, ref erroexclucao, _paramBase, banco);
								if (erroexclucao != "")
								{
									dbcxtransaction.Rollback();
									throw new Exception("Erro ao excluir um contato"); 
								}
							}
		 
						}

						if (pessoa.contatos != null)
						{
							foreach (var item in pessoa.contatos.Where(p => p.id != 0))
							{
								item.pessoa_id = pessoa.id;

								new PessoaContato().Alterar(item, _paramBase, banco);
							}

							foreach (var item in pessoa.contatos.Where(p => p.id == 0))
							{
								item.pessoa_id = pessoa.id;
								new PessoaContato().Incluir(item, _paramBase, banco);
							}
						}

                        var pccAux = new PessoaContaContabil().ObterPorPessoa(pessoa.id, banco);
                        if (pccAux == null)
                        {
                            pcc.pessoa_id = pessoa.id;
                            pcc.Incluir(_paramBase,banco);
                        }
                        else
                        {

                            pccAux.contaContabilDespesaPadrao_id = pcc.contaContabilDespesaPadrao_id;
                            pccAux.contaContabilPagarPadrao_id = pcc.contaContabilPagarPadrao_id;
                            pccAux.contaContabilReceberPadrao_id = pcc.contaContabilReceberPadrao_id;
                            pccAux.Alterar(_paramBase, banco);

                        }

                    }
					

					dbcxtransaction.Commit();
				}
				return Json(new { CDStatus = "OK", DSMessage = "Pessoa cadastrado com sucesso" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}


		public ActionResult Excluir(int id)
		{
			string erro = "";
			try
			{
                var pessoaConta = new PessoaContaContabil().ObterPorPessoa(id, null);

                if (pessoaConta != null)
                {
                    pessoaConta.Excluir(ref erro, _paramBase);
                    if (erro != "")
                        throw new Exception(erro);
                }
                new Pessoa().Excluir(id, _paramBase, ref erro);
				if (erro != "")
					throw new Exception(erro);

				return Json(new { CDStatus = "OK", DSMessage = "Pessoa excluida com sucesso" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
			}


		}



		[HttpPost]
		public ActionResult uplodFoto(int id)
		{
			for (int i = 0; i < Request.Files.Count; i++)
			{
				HttpPostedFileBase arquivo = Request.Files[i];
				string[] extensionarquivos = new string[] { ".jpg", ".jpeg", ".png" };
				if (arquivo.FileName != "")
				{
					if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
					{
						return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, extenção não permitida" }, JsonRequestBehavior.AllowGet);
					}
				}
			}

			var pessoa = new Pessoa();

			for (int i = 0; i < Request.Files.Count; i++)
			{
				HttpPostedFileBase arquivo = Request.Files[i];

				if (arquivo.ContentLength > 0)
				{
					var uploadPath = Server.MapPath("~/Foto/");
					Directory.CreateDirectory(uploadPath);

					var nomearquivonovo = "Foto_" + id.ToString() + Path.GetExtension(arquivo.FileName);

					string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

					arquivo.SaveAs(caminhoArquivo);

					AzureStorage.UploadFile(caminhoArquivo,
								"Foto/" + nomearquivonovo,
								ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

					var db = new DbControle();

					pessoa = new Pessoa().ObterPorId(id,_paramBase, db);
					pessoa.Foto = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
								"Foto/" + nomearquivonovo;
					pessoa.Alterar(_paramBase,db);

				}
			}
		 

			return Json(new { CDStatus = "OK", DSMessage = "Upload executado com sucesso" }, JsonRequestBehavior.AllowGet);

		}


        public JsonResult ConsultaDadosSefaz(string cpfcnpj)
        {
            try
            {
                var uploadPath = Server.MapPath("~/CertTMP/");
                Directory.CreateDirectory(uploadPath);
                var nomearquivonovo = Guid.NewGuid().ToString();
                string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);
                var cert = UtilSoftFin.BuscaCert(_estab, _estabobj.senhaCertificado, caminhoArquivo, _estabobj.CNPJ);

                var regra = new SoftFin.Sefaz.NFeConsultaDestNovo();
                var dto = new DTONfe();
                DbControle db = new DbControle();

                var url = db.UrlSefazUF.Where(p => p.UF == _estabobj.UF);

                var urlServico = "";
                if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
                {
                    urlServico = url.Where(p => p.codigo == "NfeConsultaCadastro").First().UrlSefazPrincipal.url;
                }
                else
                {
                    urlServico = url.Where(p => p.codigo == "NfeConsultaCadastro").First().UrlSefazPrincipal.urlHomologacao;
                }
                var result = regra.Execute(cpfcnpj, cert, urlServico,_estabobj.UF);

               

                return Json(result , JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSStatus = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}

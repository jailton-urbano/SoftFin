using Newtonsoft.Json;
using SoftFin.GestorProcessos.Helper;
using SoftFin.GestorProcessos.Comum.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using SoftFin.Utils;
using SoftFin.GestorProcessos.Models;
using SoftFin.GestorProcessos.Comum.DTO;

namespace SoftFin.GestorProcessos.Controllers
{
    public class DinamicoController : Controller
    {
        public ActionResult Teste()
        {
            return View();
        }
        public ActionResult Index()
        {
            var db = new DBGPControle();
            var paramProcesso = new ParamProcesso();
            paramProcesso.CodigoEmpresa = Request.QueryString["CodigoEmpresa"];

            if (string.IsNullOrEmpty(paramProcesso.CodigoEmpresa))
                return View();

            paramProcesso.CodigoUsuario = Request.QueryString["CodigoUsuario"];
            paramProcesso.Tabela = Request.QueryString["Tabela"];

            StringBuilder sbCabeca = new StringBuilder();
            StringBuilder sbColuna = new StringBuilder();
            StringBuilder sbCampos = new StringBuilder();

            GeraHTMLIndex(db,
                paramProcesso.Tabela,
                paramProcesso.CodigoEmpresa,
                paramProcesso.CodigoUsuario,
                sbCabeca,
                sbColuna,
                sbCampos);

            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            ViewData["paramProcesso"] = JsonConvert.SerializeObject(paramProcesso, settings);
            ViewData["titulo"] = paramProcesso.Tabela;
            ViewData["cabeca"] = sbCabeca.ToString();
            ViewData["colunas"] = sbColuna.ToString();
            ViewData["campos"] = sbCampos.ToString();


            return View();
        }

        public ActionResult ShowTable()
        {
            var db = new DBGPControle();
            var atividadeVisao = new List<AtividadeVisao>();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbTab = new StringBuilder();

            if (Request.QueryString["tabela"] == null && Request.QueryString["visao"] == null)
                return View();

            
            if (Request.QueryString["tabela"] != null)
            {
                var idtabela = 0;
                if (int.TryParse(Request.QueryString["tabela"], out idtabela) == false)
                    return View();

                atividadeVisao.Add(new AtividadeVisao
                {
                    Id = 0,
                    Titulo = "Teste",
                    Visao = new Visao
                    {
                        IdTabela = idtabela,
                        Descricao = "Teste de Visualização de Campos",
                        TipoVisao = new TipoVisao { Id = 0, Descricao = "Teste", Ativo = true },
                        Tabela = new Tabela { Id = idtabela, Nome = "Teste" }
                    }
                });
            }


            if (Request.QueryString["visao"] != null)
            {
                var idvisao = 0;
                if (int.TryParse(Request.QueryString["visao"], out idvisao) == false)
                    return View();
                var visao = db.Visao.Where(p => p.Id == idvisao).FirstOrDefault();

                atividadeVisao.Add(new AtividadeVisao
                {
                    Id = 0,
                    Titulo = "Teste",
                    Visao = visao,
                    IdVisao = visao.Id
                });
            }



            GeraHTML2(db, atividadeVisao, sb, sbTab);

            ViewData["tabs"] = sb.ToString();


            return View();
        }

        private static void GeraHTMLIndex(
            DBGPControle db,  
            string tabela,
            string estab,
            string usuario, 
            StringBuilder sbHead, 
            StringBuilder sbColuna,
            StringBuilder sbCampos
            )
        {
            var tabelaCampos = db.TabelaCampo.Where(p => p.Tabela.Nome == tabela 
                                && p.Tabela.Empresa.Codigo == estab);
            var contaColunas = 1;

            sbCampos.AppendLine("                     <form name='frm" + tabela + "'>");
            sbCampos.AppendLine("                       <div layout-gt-sm='row'>");
            foreach (var item in tabelaCampos)
            {
                sbHead.AppendLine("<th md-column md-order-by='item." + item.Campo + "'><span>" + item.Descricao + "</span></th>");
                sbColuna.AppendLine("<td md-cell>{{item." + item.Campo + "}}</td>");


                var referenciaNgModel = "";
                var referenciaNgChange = "";
                contaColunas += int.Parse(item.TamanhoColuna);
                if (contaColunas > 12)
                {
                    sbCampos.AppendLine("                     </div>");
                    sbCampos.AppendLine("                     <div layout-gt-sm='row'>");
                    contaColunas = int.Parse(item.TamanhoColuna);
                }

                var str = item.TipoCampo.TemplateHTML;
                str = str.Replace("#Descricao", item.Descricao);
                str = str.Replace("#Tabela", "entidade");
                str = str.Replace("#Campo", item.Campo);
                str = str.Replace("#tamanho", item.TamanhoColuna);
                str = str.Replace("#numpre", item.Precisao.ToString());
                str = str.Replace("#ReferenciaNgModel", referenciaNgModel);
                str = str.Replace("#ReferenciaNgChange", referenciaNgChange);

                if (item.ChaveEstrageira != null)
                {
                    str = str.Replace("#FK", item.ChaveEstrageira.Nome.ToString());
                }
                if (item.Obrigatorio)
                {
                    str = str.Replace("#requerido", " required='' ");
                    var strreq = "";
                    strreq += " <div ng-messages='frm" + item.Tabela.Nome +".entidade." + item.Campo + ".$error' multiple='' md-auto-hide='false'>";
                    strreq += "    <div ng-message='required'>";
                    strreq += "        Campo obrigatório.";
                    strreq += "    </div>";
                    strreq += " </div>";
                    str = str.Replace("#mtrequerido", strreq);
                }
                else
                {
                    str = str.Replace("#requerido", " ng-required='False' ");
                    str = str.Replace("#mtrequerido", "");
                }
                sbCampos.AppendLine(str);
            }

            if (contaColunas != 0)
            {
                sbCampos.AppendLine("                     </div>");
            }
            sbCampos.AppendLine("                     </form>");
        }


        // GET: Dinamico
        public ActionResult Create()
        {
            var db = new DBGPControle();
            var paramProcesso = new ParamProcesso();
            var modoTeste = Request.QueryString["ModoTeste"];
            paramProcesso.CodigoEmpresa = Request.QueryString["CodigoEmpresa"];

            if (string.IsNullOrEmpty(paramProcesso.CodigoEmpresa))
                return View();

            paramProcesso.CodigoProcesso = Request.QueryString["CodigoProcesso"];
            paramProcesso.CodigoAtividade = Request.QueryString["CodigoAtividade"];
            paramProcesso.CodigoUsuario = Request.QueryString["CodigoUsuario"];
            paramProcesso.NumeroProtocolo = Request.QueryString["NumeroProtocolo"];
            paramProcesso.CodigoAtividadeExecucaoAtual = Request.QueryString["CodigoAtividadeExecucaoAtual"];
            paramProcesso.CodigoProcessoAtual = Request.QueryString["CodigoProcessoAtual"];


            ViewData["NumeroProtocolo"] = paramProcesso.NumeroProtocolo;
            var codigoAtividade = paramProcesso.CodigoAtividade;

            var atividade = db.Atividade.Where(p => p.Codigo == codigoAtividade).FirstOrDefault();
            var atividadePlano = db.AtividadePlano.Where(p => p.Atividade.Codigo == codigoAtividade).FirstOrDefault();
            string entidadeJson = MontaEntidade(db, paramProcesso, modoTeste, atividade, atividadePlano);

            var atividadeVisao = db.AtividadeVisao.Where(p => p.IdAtividade == atividade.Id).OrderBy(p => p.Ordem).ToList();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbTab = new StringBuilder();
            StringBuilder sbAngulajsInput = new StringBuilder();
            StringBuilder sbJsonEnvio = new StringBuilder();
            StringBuilder sbvariaveis = new StringBuilder();
            StringBuilder sbJSScript = new StringBuilder();



            sbvariaveis.AppendLine("$scope.ModoConsulta = false;");

            GeraVariaveis(db, atividadePlano, atividadeVisao, sbvariaveis, sbJsonEnvio, sbAngulajsInput);
            sbvariaveis.AppendLine("$scope.queryAnotacao = {order: 'Id', limit: 30, page: 1, total: 0};");
            sbvariaveis.AppendLine("$scope.queryArquivos = {order: 'Id', limit: 30, page: 1, total: 0};");

            GeraHTML2(db,  atividadeVisao, sb, sbTab);
            GeraJSScript(db, atividadePlano, atividadeVisao, sbJSScript);

            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            ViewData["jsonEnvio"] = " $scope.jsonEnvio = {" + sbJsonEnvio.ToString() + "};";
            ViewData["AngulajsInput"] = sbAngulajsInput.ToString();
            ViewData["tabs"] = sb.ToString();
            ViewData["variaveis"] = sbvariaveis.ToString();
            ViewData["entidadeJson"] = entidadeJson;
            ViewData["paramProcesso"] = JsonConvert.SerializeObject(paramProcesso, settings);
            ViewData["titulo"] = atividade.Descricao;
            ViewData["JSScript"] = sbJSScript.ToString();
            ViewData["tituloprocesso"] = atividadePlano.Processo.Descricao;
            ViewData["tituloatividade"] = atividade.Descricao;

            return View();
        }

        public ActionResult Question()
        {
            var db = new DBGPControle();
            var paramProcesso = new ParamProcesso();
            var modoTeste = Request.QueryString["ModoTeste"];
            paramProcesso.CodigoEmpresa = Request.QueryString["CodigoEmpresa"];

            if (string.IsNullOrEmpty(paramProcesso.CodigoEmpresa))
                return View();

            paramProcesso.CodigoProcesso = Request.QueryString["CodigoProcesso"];
            paramProcesso.CodigoAtividade = Request.QueryString["CodigoAtividade"];
            paramProcesso.CodigoUsuario = Request.QueryString["CodigoUsuario"];
            paramProcesso.NumeroProtocolo = Request.QueryString["NumeroProtocolo"];
            paramProcesso.CodigoAtividadeExecucaoAtual = Request.QueryString["CodigoAtividadeExecucaoAtual"];
            paramProcesso.CodigoProcessoAtual = Request.QueryString["CodigoProcessoAtual"];


            ViewData["NumeroProtocolo"] = paramProcesso.NumeroProtocolo;
            var codigoAtividade = paramProcesso.CodigoAtividade;

            var atividade = db.Atividade.Where(p => p.Codigo == codigoAtividade).FirstOrDefault();
            var atividadePlano = db.AtividadePlano.Where(p => p.Atividade.Codigo == codigoAtividade).FirstOrDefault();
            string entidadeJson = MontaEntidade(db, paramProcesso, modoTeste, atividade, atividadePlano);

            var atividadeVisao = db.AtividadeVisao.Where(p => p.IdAtividade == atividade.Id).OrderBy(p => p.Ordem).ToList();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbTab = new StringBuilder();
            StringBuilder sbAngulajsInput = new StringBuilder();
            StringBuilder sbJsonEnvio = new StringBuilder();
            StringBuilder sbvariaveis = new StringBuilder();
            StringBuilder sbJSScript = new StringBuilder();



            sbvariaveis.AppendLine("$scope.ModoConsulta = false;");

            GeraVariaveis(db, atividadePlano, atividadeVisao, sbvariaveis, sbJsonEnvio, sbAngulajsInput);
            sbvariaveis.AppendLine("$scope.queryAnotacao = {order: 'Id', limit: 30, page: 1, total: 0};");
            sbvariaveis.AppendLine("$scope.queryArquivos = {order: 'Id', limit: 30, page: 1, total: 0};");

            GeraHTML2(db,  atividadeVisao, sb, sbTab, true);
            GeraJSScript(db, atividadePlano, atividadeVisao, sbJSScript);

            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            ViewData["jsonEnvio"] = " $scope.jsonEnvio = {" + sbJsonEnvio.ToString() + "};";
            ViewData["AngulajsInput"] = sbAngulajsInput.ToString();
            ViewData["tabs"] = sb.ToString();
            ViewData["variaveis"] = sbvariaveis.ToString();
            ViewData["entidadeJson"] = entidadeJson;
            ViewData["paramProcesso"] = JsonConvert.SerializeObject(paramProcesso, settings);
            ViewData["titulo"] = atividade.Descricao;
            ViewData["JSScript"] = sbJSScript.ToString();
            ViewData["tituloprocesso"] = atividadePlano.Processo.Descricao;
            ViewData["tituloatividade"] = atividade.Descricao;

            return View();
        }

        public ActionResult Edit()
        {
            var db = new DBGPControle();
            var paramProcesso = new ParamProcesso();
            var modoTeste = Request.QueryString["ModoTeste"];
            paramProcesso.CodigoEmpresa = Request.QueryString["CodigoEmpresa"];

            if (string.IsNullOrEmpty(paramProcesso.CodigoEmpresa))
                return View();

            paramProcesso.CodigoProcesso = Request.QueryString["CodigoProcesso"];
            paramProcesso.CodigoAtividade = Request.QueryString["CodigoAtividade"];
            paramProcesso.CodigoUsuario = Request.QueryString["CodigoUsuario"];
            paramProcesso.NumeroProtocolo = Request.QueryString["NumeroProtocolo"];
            paramProcesso.CodigoAtividadeExecucaoAtual = Request.QueryString["CodigoAtividadeExecucaoAtual"];
            paramProcesso.CodigoProcessoAtual = Request.QueryString["CodigoProcessoAtual"];


            ViewData["NumeroProtocolo"] = paramProcesso.NumeroProtocolo;
            var codigoAtividade = paramProcesso.CodigoAtividade;

            var atividade = db.Atividade.Where(p => p.Codigo == codigoAtividade).FirstOrDefault();
            var atividadePlano = db.AtividadePlano.Where(p => p.Atividade.Codigo == codigoAtividade).FirstOrDefault();
            string entidadeJson = MontaEntidade(db, paramProcesso, modoTeste, atividade, atividadePlano);

            var atividadeVisao = db.AtividadeVisao.Where(p => p.IdAtividade == atividade.Id).OrderBy(p => p.Ordem).ToList();
            StringBuilder sb = new StringBuilder();
            StringBuilder sbTab = new StringBuilder();
            StringBuilder sbAngulajsInput = new StringBuilder();
            StringBuilder sbJsonEnvio = new StringBuilder();
            StringBuilder sbvariaveis = new StringBuilder();
            StringBuilder sbJSScript = new StringBuilder();



            sbvariaveis.AppendLine("$scope.ModoConsulta = false;");

            GeraVariaveis(db, atividadePlano, atividadeVisao, sbvariaveis, sbJsonEnvio, sbAngulajsInput);
            sbvariaveis.AppendLine("$scope.queryAnotacao = {order: 'Id', limit: 30, page: 1, total: 0};");
            sbvariaveis.AppendLine("$scope.queryArquivos = {order: 'Id', limit: 30, page: 1, total: 0};");

            GeraHTML2(db,  atividadeVisao, sb, sbTab, false);
            GeraJSScript(db, atividadePlano, atividadeVisao, sbJSScript);

            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            ViewData["jsonEnvio"] = " $scope.jsonEnvio = {" + sbJsonEnvio.ToString() + "};";
            ViewData["AngulajsInput"] = sbAngulajsInput.ToString();
            ViewData["tabs"] = sb.ToString();
            ViewData["variaveis"] = sbvariaveis.ToString();
            ViewData["entidadeJson"] = entidadeJson;
            ViewData["paramProcesso"] = JsonConvert.SerializeObject(paramProcesso, settings);
            ViewData["titulo"] = atividade.Descricao;
            ViewData["JSScript"] = sbJSScript.ToString();
            ViewData["tituloprocesso"] = atividadePlano.Processo.Descricao;
            ViewData["tituloatividade"] = atividade.Descricao;

            return View();
        }

        private static void GeraVariaveis(DBGPControle db, AtividadePlano atividadePlano, List<AtividadeVisao> atividadeVisao, StringBuilder sbvariaveis, StringBuilder sbJsonEnvio, StringBuilder sbAngulajsInput)
        {
            foreach (var itemAV in atividadeVisao)
            {

                if (atividadePlano.AtividadeIdEntrada == null)
                {
                    sbvariaveis.AppendLine("$scope." + itemAV.Visao.Tabela.Nome + " = $scope.entidadeJson." + itemAV.Visao.Tabela.Nome + ";");
                    sbvariaveis.AppendLine("$scope.Lista" + itemAV.Visao.Tabela.Nome + " = [];");
                    sbvariaveis.AppendLine("$scope.query" + itemAV.Visao.Tabela.Nome + " = {order: 'Id', limit: 30, page: 1, total: 0};");
                }
                else
                {
                    if (itemAV.Visao.TipoVisao.Descricao == "Collection")
                    {
                        sbvariaveis.AppendLine("$scope.Lista" + itemAV.Visao.Tabela.Nome + " = $scope.entidadeJson.Lista" + itemAV.Visao.Tabela.Nome + ";");
                    }
                    else
                    {
                        sbvariaveis.AppendLine("$scope." + itemAV.Visao.Tabela.Nome + " = $scope.entidadeJson." + itemAV.Visao.Tabela.Nome + ";");
                    }
                }

                if (itemAV.Visao.TipoVisao.Descricao == "Collection")
                {
                    if (sbJsonEnvio.Length != 0)
                        sbJsonEnvio.Append(",");
                    sbJsonEnvio.Append("'Lista" + itemAV.Visao.Tabela.Nome + "' : $scope.Lista" + itemAV.Visao.Tabela.Nome);
                }
                else
                {
                    if (sbJsonEnvio.Length != 0)
                        sbJsonEnvio.Append(",");
                    sbJsonEnvio.Append("'" + itemAV.Visao.Tabela.Nome + "' : $scope." + itemAV.Visao.Tabela.Nome);
                }

                sbAngulajsInput.AppendLine("     $scope.Adicionar" + itemAV.Visao.Tabela.Nome + " = function () {");
                sbAngulajsInput.AppendLine("         $scope.Lista" + itemAV.Visao.Tabela.Nome + ".push(angular.copy($scope." + itemAV.Visao.Tabela.Nome + "));");
                sbAngulajsInput.AppendLine("         $scope." + itemAV.Visao.Tabela.Nome + " = {};");
                sbAngulajsInput.AppendLine("     }");
                sbAngulajsInput.AppendLine("     $scope.Excluir" + itemAV.Visao.Tabela.Nome + " = function (item) {");
                sbAngulajsInput.AppendLine("         $scope.Lista" + itemAV.Visao.Tabela.Nome + ".splice(item.$index,1);");
                sbAngulajsInput.AppendLine("     }");
            }
        }

        private static void GeraHTML2(
            DBGPControle db, 
            List<AtividadeVisao> atividadeVisao, 
            StringBuilder sb, 
            StringBuilder sbTab,
            bool modoConsulta = false

            )
        {

            var contatabs = 1;
            foreach (var itemAV in atividadeVisao)
            {
                var tabelaCampos = db.TabelaCampo.Where(p => p.Tabela_Id == itemAV.Visao.IdTabela);
                var atividadeVisaoCampos = db.VisaoCampo.Where(p => p.IdVisao == itemAV.IdVisao).ToList();
                StringBuilder sbHead = new StringBuilder();
                StringBuilder sbColuna = new StringBuilder();

                sb.AppendLine("             <md-tab label='" + itemAV.Visao.Descricao + "'>");

                sb.AppendLine("                 <md-content class='md-padding'>");
                
                sb.AppendLine("                     <form name='frm" + itemAV.Ordem.ToString() + "'>");

                sb.AppendLine("                     <h1 class='md-display-" + contatabs.ToString() + "'>" + itemAV.Visao.Descricao + "</h1>");

                int contaColunas = 0;
                if (itemAV.Visao.TipoVisao.Descricao == "Collection" && modoConsulta == true)
                {
                    sb.AppendLine("                     <div layout-gt-sm='row' ng-show='false'>");
                }
                else
                {
                    sb.AppendLine("                     <div layout-gt-sm='row'>");
                }
                foreach (var item in tabelaCampos)
                {
                    var gerarhtmlCampo = true;
                    var referenciaNgModel = "";
                    var referenciaNgChange = "";
                    if (atividadeVisaoCampos.Count() > 0)
                    {
                        var visaocampo = atividadeVisaoCampos.Where(p => p.IdTabelaCampo == item.Id).FirstOrDefault();
                        if (visaocampo != null)
                        {
                            gerarhtmlCampo = visaocampo.Visivel;
                            referenciaNgModel = visaocampo.ReferenciaNgModel;
                            referenciaNgChange = visaocampo.ReferenciaNgChange;
                        }
                    }

                    if (gerarhtmlCampo)
                    {
                        sbHead.AppendLine("<th md-column md-order-by='item." + item.Campo + "'><span>" + item.Descricao + "</span></th>");
                        sbColuna.AppendLine("<td md-cell>{{item." + item.Campo + "}}</td>");

                        contaColunas += int.Parse(item.TamanhoColuna);
                        if (contaColunas > 12)
                        {
                            sb.AppendLine("                     </div>");
                            sb.AppendLine("                     <div layout-gt-sm='row'>");
                            contaColunas = int.Parse(item.TamanhoColuna);
                        }

                        var str = item.TipoCampo.TemplateHTML;

                        if (modoConsulta == true)
                            str = str.Replace("#ModoConsulta", "disabled");
                        else
                            str = str.Replace("#ModoConsulta", "");

                        str = str.Replace("#Descricao", item.Descricao);
                        str = str.Replace("#Tabela", item.Tabela.Nome);
                        str = str.Replace("#Campo", item.Campo);
                        str = str.Replace("#tamanho", item.TamanhoColuna);
                        str = str.Replace("#numpre", item.Precisao.ToString());
                        str = str.Replace("#ReferenciaNgModel", referenciaNgModel);
                        str = str.Replace("#ReferenciaNgChange", referenciaNgChange);

                        if (item.ChaveEstrageira != null)
                        {
                            str = str.Replace("#FK", item.ChaveEstrageira.Nome.ToString());
                        }
                        if (item.Obrigatorio)
                        {
                            str = str.Replace("#requerido", " required='' ");
                            var strreq = "";
                            strreq += " <div ng-messages='frm" + itemAV.Ordem.ToString() + "." + item.Campo + ".$error' multiple='' md-auto-hide='false'>";
                            strreq += "    <div ng-message='required'>";
                            strreq += "        Campo obrigatório.";
                            strreq += "    </div>";
                            strreq += " </div>";
                            str = str.Replace("#mtrequerido", strreq);

                        }
                        else
                        {
                            str = str.Replace("#requerido", " ng-required='False' ");
                            str = str.Replace("#mtrequerido", "");
                        }
                        sb.AppendLine(str);
                    }
                }
                var diferenciador = itemAV.Visao.Tabela.Nome;

                sb.AppendLine("                     </div>");


                if (itemAV.Visao.TipoVisao.Descricao == "Collection" && modoConsulta == false)
                {
                    sb.AppendLine("						<md-button class='md-primary' ng-click='Adicionar" + diferenciador + "()'>Adicionar</md-button>");
                }

                sb.AppendLine("                     </form>");




                if (itemAV.Visao.TipoVisao.Descricao == "Collection")
                {
                    

                    sb.AppendLine("<md-table-container>");
                    sb.AppendLine("		<table md-table>");
                    sb.AppendLine("			<thead md-head md-order='query" + diferenciador + ".order'>");
                    sb.AppendLine("				<tr md-row>");
                    sb.AppendLine("					<th md-column>");

                    sb.AppendLine("					</th>");
                    sb.AppendLine(sbHead.ToString());
                    sb.AppendLine("				</tr>");
                    sb.AppendLine("			</thead>");
                    sb.AppendLine("			<tbody md-body>");
                    sb.AppendLine("				<tr md-row md-select='item' md-select-id='name' md-auto-select ng-repeat='item in Lista" + diferenciador + "'>");
                    sb.AppendLine("					<td md-cell>");
                    //sb.AppendLine("						<md-button class='md-raised md-primary' ng-click='Editar" + diferenciador + "(item)'>Editar</md-button>");
                    //sb.AppendLine("						<md-button class='md-raised md-default' ng-click='Detalhar" + diferenciador + "(item)'>Detalhar</md-button>");
                    sb.AppendLine("						<md-button class='md-raised md-warn' ng-click='Excluir" + diferenciador + "(item)'>Excluir</md-button>");
                    sb.AppendLine("					</td>");
                    sb.AppendLine(sbColuna.ToString());
                    sb.AppendLine("				</tr>");
                    sb.AppendLine("			</tbody>");
                    sb.AppendLine("		</table>");
                    sb.AppendLine("</md-table-container>");
                }

                sb.AppendLine("                 </md-content>");
                sb.AppendLine("             </md-tab>");
                contatabs += 1; 
            }

        }



        private static void GeraJSScript(DBGPControle db, AtividadePlano atividadePlano, List<AtividadeVisao> atividadeVisao, StringBuilder sbJSScript)
        {
        
            foreach (var itemAV in atividadeVisao)
            {
                var tabelaCampos = db.TabelaCampo.Where(p => p.Tabela_Id == itemAV.Visao.IdTabela);
                var atividadeVisaoCampos = db.VisaoCampo.Where(p => p.IdVisao == itemAV.IdVisao).ToList();


                foreach (var item in tabelaCampos)
                {
                    var gerarhtmlCampo = true;
                    var referenciaNgModel = "";
                    var referenciaNgChange = "";

                    if (atividadeVisaoCampos.Count() > 0)
                    {
                        var visaocampo = atividadeVisaoCampos.Where(p => p.IdTabelaCampo == item.Id).FirstOrDefault();
                        if (visaocampo != null)
                        {
                            gerarhtmlCampo = visaocampo.Visivel;
                            referenciaNgModel = visaocampo.ReferenciaNgModel;
                            referenciaNgChange = visaocampo.ReferenciaNgChange;
                        }
                    }

                    if (gerarhtmlCampo)
                    {
                        var strjs = item.TipoCampo.TemplateJSScript;
                        if (strjs != null)
                        {
                            strjs = strjs.Replace("#Descricao", item.Descricao);
                            strjs = strjs.Replace("#Tabela", item.Tabela.Nome);
                            strjs = strjs.Replace("#Campo", item.Campo);
                            strjs = strjs.Replace("#tamanho", item.TamanhoColuna);
                            strjs = strjs.Replace("#numpre", item.Precisao.ToString());
                            if (item.ChaveEstrageira != null)
                                strjs = strjs.Replace("#FK", item.ChaveEstrageira.Nome.ToString());
                            strjs = strjs.Replace("#ReferenciaNgModel", referenciaNgModel);
                            strjs = strjs.Replace("#ReferenciaNgChange", referenciaNgChange);

                            sbJSScript.AppendLine(strjs);
                        }

                    }

                }

            }
        }

        private string MontaEntidade(DBGPControle db, ParamProcesso paramProcesso, string modoTeste, Atividade atividade, AtividadePlano atividadePlano)
        {
            var entidadeJson = "";
            if (string.IsNullOrWhiteSpace(modoTeste))
                if (atividadePlano.AtividadeIdEntrada == null)
                {
                    entidadeJson = ObterNovo(paramProcesso);
                    entidadeJson = entidadeJson.Substring(1);
                    entidadeJson = entidadeJson.Substring(0, entidadeJson.Length -1);
                }
                else
                {
                    var lista = db.AtividadeVisao.Where(p => p.IdAtividade == atividade.Id).OrderBy(p => p.Ordem).ToList();

                    var entidadeJsonAux = ObterExistente(paramProcesso);

                    for (int i = 0; i < entidadeJsonAux.Count(); i++)
                    {
                        var atividadeitem = lista[i];

                        if (entidadeJson.Length > 0)
                            entidadeJson += ",";

                        if (atividadeitem.Visao.TipoVisao.Descricao == "Collection")
                            if (entidadeJsonAux[i] == "")
                                entidadeJson += "Lista" + atividadeitem.Visao.Tabela.Nome.ToString() + ":[]";
                            else
                                entidadeJson += "Lista" + atividadeitem.Visao.Tabela.Nome.ToString() + ":" + entidadeJsonAux[i] + "";
                        else
                            entidadeJson += atividadeitem.Visao.Tabela.Nome.ToString() + ":{" + entidadeJsonAux[i].Replace("[{", "").Replace("}]", "") + "}";

                    }

                    entidadeJson = "{" + entidadeJson + "}";

                }

            return entidadeJson;
        }

        public string ObterNovo(ParamProcesso paramProcesso)
        {
            string retorno;
            var sQLManipulation = new SQLManipulation();


            retorno = sQLManipulation.BuscaJSONNovo(paramProcesso);

            return retorno;
        }


        public JsonResult PesquisaTabela(ParamProcesso paramProcesso, Query query, string tabela)
        {
            string retorno;
            var sQLManipulation = new SQLManipulation();

            int total = 0;
            retorno = sQLManipulation.BuscaJSONNovoTabela(paramProcesso,tabela, query.order, query.page, query.limit, ref total);
            query.total = total;

            return Json(new { Query = query, Lista = retorno }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PesquisaParametro(ParamProcesso paramProcesso, string parametro)
        {
            var sQLManipulation = new SQLManipulation();
            

            var retorno = new DBGPControle().EmpresaParametro.Where(p => p.Codigo == parametro && p.Empresa.Codigo == paramProcesso.CodigoEmpresa).FirstOrDefault();
            return Json(retorno , JsonRequestBehavior.AllowGet);
        }

        public List<string> ObterExistente(ParamProcesso paramProcesso)
        {
            List<string> retorno;
            var sQLManipulation = new SQLManipulation();


            retorno = sQLManipulation.BuscaJSONExistente(paramProcesso);

            return retorno;
        }


        public JsonResult Salvar(ParamProcesso paramProcesso, string json, string nomeView)
        {
            string retorno = "OK";

            DTOProximaAtividade proximaAtividade = new DTOProximaAtividade();
            var sQLManipulation = new SQLManipulation();
            try
            {
                if (nomeView == "Create" || nomeView == "Edit")
                    retorno = sQLManipulation.SalvaJson(paramProcesso, json);
                proximaAtividade = new API.Controllers.AtividadeExecucaoController().GerarProximaAtividade(paramProcesso);
                proximaAtividade.MostraProximaAtividade = true;
                proximaAtividade.CDStatus = "OK";
                return Json(proximaAtividade, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                proximaAtividade.DSMessage = ex.Message;
                proximaAtividade.CDStatus = "NOK";
                return Json(proximaAtividade, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SalvarTabela(ParamProcesso paramProcesso, string json)
        {
            string retorno = "OK";
            DTOProximaAtividade proximaAtividade = new DTOProximaAtividade();
            var sQLManipulation = new SQLManipulation();
            try
            {

                retorno = sQLManipulation.SalvaTabelaJson(paramProcesso, json);

                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult ExcluirTabela(ParamProcesso paramProcesso, int Id)
        {
            string retorno = "OK";
            DTOProximaAtividade proximaAtividade = new DTOProximaAtividade();
            var sQLManipulation = new SQLManipulation();
            try
            {

                retorno = sQLManipulation.ExcluiTabelaJson(paramProcesso, Id);

                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public JsonResult ExcluirArquivo(ParamProcesso paramProcesso, int id)
        {
            try
            {
                var arquivo = new Arquivos().ObterPorId(id, paramProcesso.CodigoEmpresa);


                AzureStorage.DeleteFile(arquivo.ArquivoReal,
                                    "Processos/" + paramProcesso.CodigoEmpresa + "/" + paramProcesso.CodigoProcessoAtual.ToString() + "/" + arquivo.ArquivoOriginal,
                                    ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                new Arquivos().Excluir(id, paramProcesso.CodigoEmpresa);

                return Json(new { CDStatus = "OK", DSMessage = "Excluido com sucesso", Objs = new Arquivos().ObterPorCodigo("processo", paramProcesso.CodigoEmpresa, paramProcesso.CodigoProcessoAtual.ToString(), null).Select(p => new { p.Id, p.Processo, p.Descricao, p.Usuario, DataInclucao = p.DataInclucao.ToString("o"), p.ArquivoOriginal, p.ArquivoReal }).ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message });
            }

        }


        public JsonResult AdicionaAnotacao(ParamProcesso paramProcesso, string anotacao)
        {
            try
            {
                var db = new DBGPControle();
                var item = new ProcessoAnotacao();
                item.DataInclusao = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia();
                item.Descricao = anotacao;
                item.Usuario = paramProcesso.CodigoUsuario;
                item.Empresa = paramProcesso.CodigoEmpresa;
                Guid gcpa = Guid.Parse(paramProcesso.CodigoProcessoAtual);
                item.ProcessoExecucaoId = db.ProcessoExecucao.Where(p => p.Codigo == gcpa).First().Id;
                db.ProcessoAnotacao.Add(item);
                db.SaveChanges();

                return Json(new { CDStatus = "OK", DSMessage = "Salvo com sucesso",
                    Objs = db.ProcessoAnotacao.Where(p => p.ProcessoExecucaoId == item.ProcessoExecucaoId)
                    .ToList()
                    .Select(p => new { DataInclusao = p.DataInclusao.ToString("o"), p.Descricao, p.Empresa, p.Id, p.ProcessoExecucaoId, p.Usuario })
                    .OrderByDescending(p => p.DataInclusao)
                    .ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message });
            }
        }

        public JsonResult ObterAnotacao(ParamProcesso paramProcesso)
        {
            try
            {
                var db = new DBGPControle();
                Guid gcpa = Guid.Parse(paramProcesso.CodigoProcessoAtual);
                var processoExecucaoId = db.ProcessoExecucao.Where(p => p.Codigo == gcpa).First().Id;

                return Json(new
                {
                    CDStatus = "OK",
                    Objs = db.ProcessoAnotacao.Where(p => p.ProcessoExecucaoId == processoExecucaoId)
                    .ToList()
                    .Select(p => new { DataInclusao = p.DataInclusao.ToString("o"), p.Descricao, p.Empresa, p.Id, p.ProcessoExecucaoId, p.Usuario })
                    .OrderByDescending(p => p.DataInclusao)
                    .ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Upload(string processo, string empresa, string usuario, string descricao, FormCollection formCollection)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];
                string[] extensionarquivos = new string[] { ".doc", ".docx", ".xlxs", ".xlx", ".pdf", ".txt", ".jpeg", ".jpg", ".png" };
                if (arquivo.FileName != "")
                {
                    if (!extensionarquivos.Contains(arquivo.FileName.ToLower().Substring(arquivo.FileName.LastIndexOf('.'))))
                    {
                        return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, extenção não permitida" });

                    }
                    else
                    {
                        var arqs = new Arquivos().ObterPorNomeDeArquivo("processo", arquivo.FileName, processo, empresa );
                        if (arqs.Count() >= 1)
                        {
                            return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, arquivo já gravado" });
                        }
                    }
                }
            }

            

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase arquivo = Request.Files[i];

                if (arquivo.ContentLength > 0)
                {
                    var uploadPath = Server.MapPath("~/TXTTemp/");
                    Directory.CreateDirectory(uploadPath);

                    var nomearquivonovo = arquivo.FileName;

                    string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

                    arquivo.SaveAs(caminhoArquivo);

                    AzureStorage.UploadFile(caminhoArquivo,
                                "Processos/" + empresa + "/" + processo.ToString() + "/" + nomearquivonovo,
                                ConfigurationManager.AppSettings["StorageCompartilhado"].ToString());

                    var db = new DBGPControle();

                    var processoarquivo = new Arquivos();



                    processoarquivo.ArquivoReal = ConfigurationManager.AppSettings["urlstoradecompartilhado"] +
                                "Processos/" + empresa + "/" + processo.ToString() + "/" + nomearquivonovo;
                    processoarquivo.ArquivoOriginal = nomearquivonovo;
                    processoarquivo.Descricao = descricao;
                    processoarquivo.RotinaOwner = "processo";
                    processoarquivo.Usuario = usuario;
                    processoarquivo.Processo = processo;
                    processoarquivo.Tamanho = arquivo.ContentLength;
                    processoarquivo.DataInclucao = DateTime.Now;
                    processoarquivo.ArquivoExtensao = Path.GetExtension(arquivo.FileName);
                    processoarquivo.Codigo = processo;
                    processoarquivo.Empresa = empresa;
                    processoarquivo.Salvar();
                }
            }

            return Json(new
            {
                CDStatus = "OK",
                DSMessage = "Arquivo salvo com suceso",
                Objs = new Arquivos().ObterPorCodigo("processo", empresa, processo, null).Select(p => new { p.Id, p.Processo, p.Descricao, DataInclucao = p.DataInclucao.ToString("o"), p.Usuario, p.ArquivoOriginal, p.ArquivoReal }).ToList()
            });
        }

        [HttpPost]
        public JsonResult ObterArquivos(string processo, string empresa)
        {
            return Json(new
            {
                CDStatus = "OK",
                DSMessage = "Arquivo salvo com suceso",
                Objs = new Arquivos().ObterPorCodigo("processo", empresa, processo, null).
                Select(p => new { p.Id, p.Processo, p.Descricao, DataInclucao = p.DataInclucao.ToString("o"), p.Usuario, p.ArquivoOriginal, p.ArquivoReal }).ToList()
            });
        }

        //TODO Criar Chamada de Procedure Tabela.SQL

        [HttpPost]
        public JsonResult ConsultaDropDown(ParamProcesso paramProcesso, string tabela, int idRelacional)
        {

            string retorno = "NOK";
            var sQLManipulation = new SQLManipulation();

            var db = new DBGPControle();

            var tabelaobj = db.Tabela.Where(p => p.Empresa.Codigo == paramProcesso.CodigoEmpresa && p.Nome == tabela).FirstOrDefault();

            if (tabelaobj == null)
                return Json(retorno, JsonRequestBehavior.AllowGet);
            var sqlQuery = tabelaobj.SQLCadastroAuxiliar;

            sqlQuery = sqlQuery.Replace("#IdRelacional", idRelacional.ToString());

            retorno = sQLManipulation.ConsultaDropDown(sqlQuery);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public class Query
        {
            public string order { get; set; }
            public int limit { get; set; }
            public int page { get; set; }

            public int total { get; set; }
        }

        public ActionResult mylims()
        {
            var paramProcesso = new ParamProcesso();
            var modoTeste = Request.QueryString["ModoTeste"];
            MontaParamProcesso(paramProcesso);
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            ViewData["paramProcesso"] = JsonConvert.SerializeObject(paramProcesso, settings);
            return View();
        }

        private void MontaParamProcesso(ParamProcesso paramProcesso)
        {
            paramProcesso.CodigoAtividadeExecucaoAtual = Request.QueryString["CodigoAtividadeExecucaoAtual"];
            paramProcesso.CodigoUsuario = Request.QueryString["CodigoUsuario"];
            if (string.IsNullOrEmpty(paramProcesso.CodigoAtividadeExecucaoAtual))
                throw new Exception("Não Iniciado...");

            var db = new DBGPControle();

            var atividadeExecucaoAtual = db.ProcessoExecucaoAtividade.Where(p => p.Codigo == paramProcesso.CodigoAtividadeExecucaoAtual).First();

            paramProcesso.CodigoEmpresa = atividadeExecucaoAtual.Atividade.Processo.Empresa.Codigo;
            paramProcesso.CodigoProcesso = atividadeExecucaoAtual.Atividade.Processo.Codigo;
            paramProcesso.CodigoAtividade = atividadeExecucaoAtual.Atividade.Codigo;
            paramProcesso.NumeroProtocolo = atividadeExecucaoAtual.ProcessoExecucao.Protocolo;
            paramProcesso.CodigoProcessoAtual = atividadeExecucaoAtual.ProcessoExecucao.Codigo.ToString();
        }
    }
}
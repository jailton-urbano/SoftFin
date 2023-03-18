using Lib.Web.Mvc.JQuery.JqGrid;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Xml;
using System.Text;
using System.Net;
using System.Collections.ObjectModel;
using SoftFin.Utils;
using System.Configuration;
namespace SoftFin.Web.Controllers
{
    public class RegistroPontoController : BaseController
    {
        public ActionResult Index2()
        {
            RegistroPontoView2 registroPonto2 = GeraDadosView();

            ViewData["dateTimeBrasilia"] = UtilSoftFin.DateTimeBrasilia(); ;

            return View(registroPonto2);
        }

        private RegistroPontoView2 GeraDadosView()
        {
            ViewData["dateTimeBrasilia"] = UtilSoftFin.DateTimeBrasilia(); ;

            RegistroPonto registroPonto = new RegistroPonto();
            RegistroPontoView2 registroPonto2 = new RegistroPontoView2();
            var dataatualtexto = UtilSoftFin.DateTimeBrasilia().ToShortDateString();
            var dataatual = UtilSoftFin.DateTimeBrasilia();
            var horainicial = " 00:00:00";
            var horafinal = " 23:59:59";

            var dataInicial = DateTime.Parse(dataatualtexto + horainicial);
            var dataFinal = DateTime.Parse(dataatualtexto + horafinal);
            var idusuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());

            var registros = registroPonto.ObterTodosDataUsuario(dataInicial, dataFinal, idusuario, _paramBase);

            List<string> lista = new List<string>();

            if (registros.Count > 0)
            {
                var reg = registros.First();
                if (reg.ponto1 != dataInicial)
                {
                    lista.Add(reg.ponto1.ToShortTimeString());
                }
                if (reg.ponto2 != dataInicial)
                {
                    lista.Add(reg.ponto2.ToShortTimeString());
                }
                if (reg.ponto3 != dataInicial)
                {
                    lista.Add(reg.ponto3.ToShortTimeString());
                }
                if (reg.ponto4 != dataInicial)
                {
                    lista.Add(reg.ponto4.ToShortTimeString());
                }
                if (reg.ponto5 != dataInicial)
                {
                    lista.Add(reg.ponto5.ToShortTimeString());
                }
                if (reg.ponto6 != dataInicial)
                {
                    lista.Add(reg.ponto6.ToShortTimeString());
                }
                if (reg.ponto7 != dataInicial)
                {
                    lista.Add(reg.ponto7.ToShortTimeString());
                }
                if (reg.ponto8 != dataInicial)
                {
                    lista.Add(reg.ponto8.ToShortTimeString());
                }
                registroPonto2.comentarios = reg.comentarios;
            }

            ViewData["Lista"] = lista;
            registroPonto2.data = dataatual;
            registroPonto2.ponto = dataatual.ToShortTimeString();
            return registroPonto2;
        }

        [HttpPost]
        public ActionResult Index2(RegistroPontoView2 obj)
        {

            RegistroPonto registroPonto = new RegistroPonto();

            var dataatualtexto = UtilSoftFin.DateTimeBrasilia().ToShortDateString();
            var dataatual = UtilSoftFin.DateTimeBrasilia();
            var horainicial = " 00:00:00";
            var horafinal = " 23:59:59";

            var dataInicial = DateTime.Parse(dataatualtexto + horainicial);
            var dataFinal = DateTime.Parse(dataatualtexto + horafinal);
            var idusuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());

            var registros = registroPonto.ObterTodosDataUsuario(dataInicial,dataFinal,idusuario, _paramBase);

            if (registros.Count() != 0)
                registroPonto = registros.First();

            registroPonto.apontador_id = idusuario;
            registroPonto.estabelecimento_id = _estab;
            registroPonto.comentarios = obj.comentarios;
            registroPonto.data = dataInicial;

            var msgJaLacto = "Data já lançada";
            var dataVerificacao = new DateTime(dataatual.Date.Year, dataatual.Date.Month, dataatual.Date.Day, dataatual.Hour, dataatual.Minute, 0);


            if (registros.Count() == 0)
            {
                registroPonto.ponto1 = dataVerificacao;
                registroPonto.ponto2 = dataInicial;
                registroPonto.ponto3 = dataInicial;
                registroPonto.ponto4 = dataInicial;
                registroPonto.ponto5 = dataInicial;
                registroPonto.ponto6 = dataInicial;
                registroPonto.ponto7 = dataInicial;
                registroPonto.ponto8 = dataInicial;

                if (registroPonto.Incluir(_paramBase))
                {
                    ViewBag.msg = "Salvo com sucesso";
                    return View(GeraDadosView());
                }
                else
                {
                    ViewBag.msg = "Impossivel salvar.";
                    return View(GeraDadosView());
                }
            }
            else
            {

 



                if (dataVerificacao == registroPonto.ponto1)
                {
                    ViewBag.msg = msgJaLacto;
                    return View(GeraDadosView());
                }
                if (dataVerificacao == registroPonto.ponto2)
                {
                    ViewBag.msg = msgJaLacto;
                    return View(GeraDadosView());
                }
                if (dataVerificacao == registroPonto.ponto3)
                {
                    ViewBag.msg = msgJaLacto;
                    return View(GeraDadosView());
                }
                if (dataVerificacao == registroPonto.ponto4)
                {
                    ViewBag.msg = msgJaLacto;
                    return View(GeraDadosView());
                }
                if (dataVerificacao == registroPonto.ponto5)
                {
                    ViewBag.msg = msgJaLacto;
                    return View(GeraDadosView());
                }
                if (dataVerificacao == registroPonto.ponto6)
                {
                    ViewBag.msg = msgJaLacto;
                    return View(GeraDadosView());
                }
                if (dataVerificacao == registroPonto.ponto7)
                {
                    ViewBag.msg = msgJaLacto;
                    return View(GeraDadosView());
                }
                if (dataVerificacao == registroPonto.ponto8)
                {
                    ViewBag.msg = msgJaLacto;
                    return View(GeraDadosView());
                }


                if (registroPonto.ponto2 == dataInicial)
                {
                    registroPonto.ponto2 = dataVerificacao;
                }
                else if(registroPonto.ponto3 == dataInicial)
                {
                    registroPonto.ponto3 = dataVerificacao;
                }
                else if (registroPonto.ponto4 == dataInicial)
                {
                    registroPonto.ponto4 = dataVerificacao;
                }
                else if (registroPonto.ponto5 == dataInicial)
                {
                    registroPonto.ponto5 = dataVerificacao;
                }
                else if (registroPonto.ponto6 == dataInicial)
                {
                    registroPonto.ponto6 = dataVerificacao;
                }
                else if (registroPonto.ponto7 == dataInicial)
                {
                    registroPonto.ponto7 = dataVerificacao;
                }
                else if (registroPonto.ponto8 == dataInicial)
                {
                    registroPonto.ponto8 = dataVerificacao;
                }
                var regaux = new RegistroPonto
                {
                    apontador_id = registroPonto.apontador_id,
                    aprovador_id = registroPonto.aprovador_id,
                    comentarios = registroPonto.comentarios,
                    data = registroPonto.data,
                    dataAprovado = registroPonto.dataAprovado,
                    DescricaoAprovacao = registroPonto.DescricaoAprovacao,
                    estabelecimento_id = registroPonto.estabelecimento_id,
                    id = registroPonto.id,
                    ponto1 = registroPonto.ponto1,
                    ponto2 = registroPonto.ponto2,
                    ponto3 = registroPonto.ponto3,
                    ponto4 = registroPonto.ponto4,
                    ponto5 = registroPonto.ponto5,
                    ponto6 = registroPonto.ponto6,
                    ponto7 = registroPonto.ponto7,
                    ponto8 = registroPonto.ponto8,
                    SituacaoAprovacao = registroPonto.SituacaoAprovacao
                };

                if (regaux.Alterar(_paramBase))
                {
                    ViewBag.msg = "Salvo com sucesso";
                    return View(GeraDadosView());
                }
                else
                {
                    ViewBag.msg = "Impossivel salvar.";
                    return View(GeraDadosView());
                }

            }            
        }






        public ActionResult Excel()
        {
            var obj = new RegistroPonto();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                myExport["apontador_id"] = item.apontador_id;
                myExport["Comentarios"] = item.comentarios;
                myExport["data"] = item.data;
                myExport["ponto1"] = item.ponto1;
                myExport["ponto2"] = item.ponto2;
                myExport["ponto3"] = item.ponto3;
                myExport["ponto4"] = item.ponto4;
                myExport["ponto5"] = item.ponto5;
                myExport["ponto6"] = item.ponto6;
                myExport["ponto7"] = item.ponto7;
                myExport["ponto8"] = item.ponto8;
                myExport["Descricao Aprovacao"] = item.DescricaoAprovacao;
                myExport["Situacao Aprovacao"] = item.SituacaoAprovacao;
                myExport["aprovador_id"] = item.aprovador_id;
                myExport["aprovador"] = item.aprovador;
                myExport["dataAprovado"] = item.dataAprovado;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "RegistroPonto.csv");
        }
        public ActionResult Index()
        {
            var obj = new RegistroPonto();
            return View(obj);
        }


        
        public ActionResult Listas(JqGridRequest request)
        {
            string user = Acesso.UsuarioLogado();
            int usuario = Acesso.RetornaIdUsuario(user);
            string ValordataPonto = Request.QueryString["dataPonto"];
            int totalRecords = 0;
            RegistroPonto obj = new RegistroPonto();
            var objs = new RegistroPonto().ObterTodosUsuario(usuario, _paramBase);


            if (!String.IsNullOrEmpty(ValordataPonto))
            {
                DateTime aux;
                DateTime.TryParse(ValordataPonto, out aux);
                objs = objs.Where(p => p.data == aux).ToList();
            }

            totalRecords = objs.Count();
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            var auxAprovador = "";
            var auxDataAprovado = "";
            var auxSituacaoAprovacao = "";


            objs = objs.Skip(12 * request.PageIndex).Take(12).ToList();
            foreach (var item in objs)
            {
                if (item.aprovador != null)
                    auxAprovador = item.aprovador.nome;
                if (item.dataAprovado != null)
                    auxDataAprovado = item.dataAprovado.ToString();
                if (item.SituacaoAprovacao != null)
                    auxSituacaoAprovacao = item.SituacaoAprovacao;

                response.Records.Add(new JqGridRecord(Convert.ToString(item.id), new List<object>()
                {
                    item.apontador.nome,
                    item.data.ToShortDateString(),
                    item.ponto1.ToShortTimeString(),
                    item.ponto2.ToShortTimeString(),
                    item.ponto3.ToShortTimeString(),
                    item.ponto4.ToShortTimeString(),
                    item.ponto5.ToShortTimeString(),
                    item.ponto6.ToShortTimeString(),
                    item.ponto7.ToShortTimeString(),
                    item.ponto8.ToShortTimeString(),
                    auxAprovador,
                    auxDataAprovado,
                    auxSituacaoAprovacao
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegistroPontoView objView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RegistroPonto obj = new RegistroPonto();

                    obj.apontador_id = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
                    obj.estabelecimento_id = _paramBase.estab_id;
                    obj.comentarios = objView.comentarios;
                    obj.data = objView.data;

                    var hora = 0;
                    var minuto = 0;
                    TimeSpan p1 = new TimeSpan(0, 0, 0);
                    if (objView.ponto1 != null)
                    {
                        if (objView.ponto1.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto1.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto1.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto1.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto1.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto1 = DateTime.Now.Date + p1;
                    }
                    else { obj.ponto1 = DateTime.Now.Date; }

                    if (objView.ponto2 != null)
                    {
                        if (objView.ponto2.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto2.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto2.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto2.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto2.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto2 = DateTime.Now.Date + p1;
                    }
                    else { obj.ponto2 = DateTime.Now.Date; }

                    if (objView.ponto3 != null)
                    {
                        if (objView.ponto3.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto3.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto3.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto3.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto3.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto3 = DateTime.Now.Date + p1;
                    }
                    else { obj.ponto3 = DateTime.Now.Date; }

                    if (objView.ponto4 != null)
                    {
                        if (objView.ponto4.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto4.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto4.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto4.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto4.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto4 = DateTime.Now.Date + p1;
                    }
                    else { obj.ponto4 = DateTime.Now.Date; }

                    if (objView.ponto5 != null)
                    {
                        if (objView.ponto5.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto5.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto5.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto5.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto5.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto5 = DateTime.Now.Date + p1;
                    }
                    else { obj.ponto5 = DateTime.Now.Date; }

                    if (objView.ponto6 != null)
                    {
                        if (objView.ponto6.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto6.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto6.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto6.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto6.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto6 = DateTime.Now.Date + p1;
                    }
                    else { obj.ponto6 = DateTime.Now.Date; }

                    if (objView.ponto7 != null)
                    {
                        if (objView.ponto7.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto7.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto7.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto7.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto7.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto7 = DateTime.Now.Date + p1;
                    }
                    else { obj.ponto7 = DateTime.Now.Date; }

                    if (objView.ponto8 != null)
                    {
                        if (objView.ponto8.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto8.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto8.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto8.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto8.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto8 = DateTime.Now.Date + p1;
                    }
                    else { obj.ponto8 = DateTime.Now.Date; }

                    if (obj.Incluir(_paramBase))
                    {
                        ViewBag.msg = "Incluído com sucesso";
                        return View(objView);
                    }
                    else
                    {
                        ViewBag.msg = "Impossivel salvar, este registro já esta cadastrado";
                        return View(objView);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(objView);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Create(string dataPonto)
        {
            try
            {
                var obj = new RegistroPontoView();
                obj.data = DateTime.Parse(dataPonto);
                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegistroPontoView objView)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    RegistroPonto obj = new RegistroPonto();

                    obj.apontador_id = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
                    obj.estabelecimento_id = _paramBase.estab_id;
                    obj.comentarios = objView.comentarios;
                    obj.data = objView.data;
                    obj.id = objView.id;
                    DateTime dataAux = objView.data.Date;

                    var hora = 0;
                    var minuto = 0;
                    TimeSpan p1 = new TimeSpan(0, 0, 0);
                    if (objView.ponto1 != null)
                    {
                        if (objView.ponto1.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto1.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto1.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto1.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto1.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto1 = dataAux + p1;
                    }
                    else { obj.ponto1 = DateTime.Now.Date; }

                    if (objView.ponto2 != null)
                    {
                        if (objView.ponto2.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto2.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto2.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto2.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto2.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto2 = dataAux + p1;
                    }
                    else { obj.ponto2 = DateTime.Now.Date; }

                    if (objView.ponto3 != null)
                    {
                        if (objView.ponto3.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto3.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto3.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto3.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto3.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto3 = dataAux + p1;
                    }
                    else { obj.ponto3 = DateTime.Now.Date; }

                    if (objView.ponto4 != null)
                    {
                        if (objView.ponto4.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto4.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto4.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto4.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto4.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto4 = dataAux + p1;
                    }
                    else { obj.ponto4 = DateTime.Now.Date; }

                    if (objView.ponto5 != null)
                    {
                        if (objView.ponto5.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto5.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto5.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto5.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto5.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto5 = dataAux + p1;
                    }
                    else { obj.ponto5 = DateTime.Now.Date; }

                    if (objView.ponto6 != null)
                    {
                        if (objView.ponto6.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto6.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto6.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto6.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto6.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto6 = dataAux + p1;
                    }
                    else { obj.ponto6 = DateTime.Now.Date; }

                    if (objView.ponto7 != null)
                    {
                        if (objView.ponto7.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto7.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto7.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto7.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto7.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto7 = dataAux + p1;
                    }
                    else { obj.ponto7 = DateTime.Now.Date; }

                    if (objView.ponto8 != null)
                    {
                        if (objView.ponto8.Substring(1, 1) == ":")
                        {
                            hora = Convert.ToInt32(objView.ponto8.Substring(0, 1));
                            minuto = Convert.ToInt32(objView.ponto8.Substring(2, 2));
                        }
                        else
                        {
                            hora = Convert.ToInt32(objView.ponto8.Substring(0, 2));
                            minuto = Convert.ToInt32(objView.ponto8.Substring(3, 2));
                        }
                        p1 = new TimeSpan(hora, minuto, 0);
                        obj.ponto8 = dataAux + p1;
                    }
                    else { obj.ponto8 = DateTime.Now.Date; }

                    if (obj.Alterar(_paramBase))
                    {
                        ViewBag.msg = "Alterado com sucesso";
                        return View(objView);
                    }
                    else
                    {
                        ViewBag.msg = "Impossivel alterar, registro excluído";
                        return View(objView);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dados Invalidos");
                    return View(objView);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Edit(int ID)
        {
            try
            {
                RegistroPonto obj = new RegistroPonto();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);
                obj.apontador_id = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
                obj.estabelecimento_id = _paramBase.estab_id;

                RegistroPontoView objView = new RegistroPontoView();
                objView.id = obj.id;
                objView.comentarios = obj.comentarios;
                objView.data = obj.data;
                objView.ponto1 = obj.ponto1.ToShortTimeString();
                objView.ponto2 = obj.ponto2.ToShortTimeString();
                objView.ponto3 = obj.ponto3.ToShortTimeString();
                objView.ponto4 = obj.ponto4.ToShortTimeString();
                objView.ponto5 = obj.ponto5.ToShortTimeString();
                objView.ponto6 = obj.ponto6.ToShortTimeString();
                objView.ponto7 = obj.ponto7.ToShortTimeString();
                objView.ponto8 = obj.ponto8.ToShortTimeString();

                if (obj.SituacaoAprovacao == "Aprovado")
                    return RedirectToAction("/Index");
                return View(objView);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Detail(int ID)
        {
            try
            {
                ApontamentoDiario obj = new ApontamentoDiario();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);

                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(RegistroPonto obj)
        {
            try
            {
                string erro = "";
                if (obj.Excluir(ref erro, _paramBase))
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
                return RedirectToAction("Index", "Erros");
            }
        }
        public ActionResult Delete(int ID)
        {
            try
            {
                RegistroPonto obj = new RegistroPonto();
                obj = obj.ObterPorId(ID, _paramBase);
                SoftFin.Utils.UtilSoftFin.Seguranca.validaNulo(obj);

                return View(obj);
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return RedirectToAction("Index", "Erros");
            }
        }

        public ActionResult FolhaPonto(RegistroPonto obj)
        {
            CarregaViewData();
            return View(obj);
        }

        private void CarregaViewData()
        {
            ViewData["usuario"] = new SelectList(new Usuario().ObterTodosUsuariosAtivos(_paramBase).OrderBy(x => x.nome), "id", "nome");
            ViewData["DataInicial"] = DateTime.Now.ToShortDateString();
            ViewData["DataFinal"] = DateTime.Now.ToShortDateString();
        }

        public FileStreamResult geraFolhaPontoPDF(string dataInicial, string dataFinal, int usuario)
        {

            DateTime DataInicial = new DateTime();
            DataInicial = DateTime.Parse(dataInicial);

            DateTime DataFinal = new DateTime();
            DataFinal = DateTime.Parse(dataFinal);

            var registro = new RegistroPonto().ObterTodosDataUsuario(DataInicial, DataFinal, usuario, _paramBase).Where(x=> x.SituacaoAprovacao != null).OrderBy(p=> p.data);

            int estab = _paramBase.estab_id;
            MemoryStream stream = new MemoryStream();

            Document doc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
            pdfWriter.CloseStream = false;

            doc.Open();

            //Carrega Logo
            try
            {
                var arquivo = _estabobj.Logo;
                if (_estabobj.Logo != null)
                {
                    var request = WebRequest.Create(arquivo);
                    using (var response = request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(arquivo);
                            logo.ScalePercent(50f);
                            //logo.SetAbsolutePosition(70f, 10f);
                            doc.Add(logo);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError &&
                    ex.Response != null)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Do something
                    }
                    else
                    {
                        // Do something else
                    }
                }
                else
                {
                    // Do something else
                }
            }


            //Pula linha
            doc.Add(Chunk.NEWLINE);

            //Título
            Font verdanaTitle = FontFactory.GetFont("Verdana", 14, Font.BOLD, BaseColor.DARK_GRAY);
            Chunk title = new Chunk("Folha de Ponto");
            title.SetUnderline(1f, -2f);
            title.Font = verdanaTitle;
            Phrase f1 = new Phrase();
            f1.Add(title);
            Paragraph p1 = new Paragraph();
            p1.Add(f1);
            p1.Alignment = 1;
            doc.Add(p1);

            //Pula linha
            doc.Add(Chunk.NEWLINE);

            //Tabela Cabeçalho
            PdfPTable tableHeader = new PdfPTable(2);
            tableHeader.TotalWidth = 500f;
            tableHeader.LockedWidth = true;
            float[] widths2 = new float[] { 300f, 200f };
            tableHeader.SetWidths(widths2);

            //Dados do Empregador
            Estabelecimento obj = new Estabelecimento().ObterPorId(estab, _paramBase);
            Chunk razao = new Chunk("Empregador: " + obj.NomeCompleto);
            Phrase Prazao = new Phrase(razao);
            PdfPCell cRazao = new PdfPCell(Prazao);
            cRazao.HorizontalAlignment = 0;
            cRazao.Border = 0;
            tableHeader.AddCell(cRazao);

            Chunk cnpj = new Chunk("CNPJ: " + obj.CNPJ);
            Phrase Pcnpj = new Phrase(cnpj);
            PdfPCell cCnpj = new PdfPCell(Pcnpj);
            cCnpj.HorizontalAlignment = 0;
            cCnpj.Border = 0;
            tableHeader.AddCell(cCnpj);

            //Dados do Empregador
            Chunk periodo = new Chunk("Mês/Ano: " + DataFinal.Month + "/" + DataFinal.Year);
            Phrase Pperiodo = new Phrase(periodo);
            PdfPCell CPeriodo = new PdfPCell(Pperiodo);
            CPeriodo.HorizontalAlignment = 0;
            CPeriodo.Border = 0;
            tableHeader.AddCell(CPeriodo);

            Chunk branco = new Chunk("");
            Phrase Pbranco = new Phrase(branco);
            PdfPCell Cbranco = new PdfPCell(Pbranco);
            Cbranco.HorizontalAlignment = 0;
            Cbranco.Border = 0;
            tableHeader.AddCell(Cbranco);

            //Linha em branco na tabela
            Chunk branco2 = new Chunk("");
            Phrase Pbranco2 = new Phrase(branco2);
            PdfPCell Cbranco2 = new PdfPCell(Pbranco2);
            Cbranco2.HorizontalAlignment = 0;
            Cbranco2.Border = 0;
            tableHeader.AddCell(Cbranco2);

            Chunk branco3 = new Chunk("");
            Phrase Pbranco3 = new Phrase(branco3);
            PdfPCell Cbranco3 = new PdfPCell(Pbranco3);
            Cbranco3.HorizontalAlignment = 0;
            Cbranco3.Border = 0;
            tableHeader.AddCell(Cbranco3);

            //Dados do Funcionário
            string usu = new Usuario().ObterPorId(usuario).nome;
            Chunk colaborador = new Chunk("Funcionário: " + usu);
            Phrase Pcolaborador = new Phrase(colaborador);
            PdfPCell c1 = new PdfPCell(Pcolaborador);
            c1.HorizontalAlignment = 0;
            c1.Border = 0;
            tableHeader.AddCell(c1);

            Chunk branco4 = new Chunk("");
            Phrase Pbranco4 = new Phrase(branco4);
            PdfPCell Cbranco4 = new PdfPCell(Pbranco4);
            Cbranco4.HorizontalAlignment = 0;
            Cbranco4.Border = 0;
            tableHeader.AddCell(Cbranco4);

            //Adiciona tableHeader ao Documento
            doc.Add(tableHeader);

            //Pula linhas
            doc.Add(Chunk.NEWLINE);

            //Corpo do formulário
            PdfPTable table = new PdfPTable(6);
            table.TotalWidth = 500f;
            table.LockedWidth = true;
            float[] widths3 = new float[] { 100f, 50f, 50f, 50f, 50f, 200f};
            table.SetWidths(widths3);

            //Fonte do Título da Tabela
            Font tableTitle = FontFactory.GetFont("Verdana", 12, Font.BOLD, BaseColor.WHITE);

            //Título Data
            Chunk data = new Chunk("Data");
            data.Font = tableTitle;
            Phrase dataP = new Phrase(data);
            PdfPCell cellData = new PdfPCell(dataP);
            cellData.HorizontalAlignment = 1;
            cellData.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(cellData);

            //Título Pontos
            Chunk ponto1 = new Chunk("ENT"); //Ponto1
            ponto1.Font = tableTitle;
            Phrase Pponto1 = new Phrase(ponto1);
            PdfPCell CPonto1 = new PdfPCell(Pponto1);
            CPonto1.HorizontalAlignment = 1;
            CPonto1.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(CPonto1);

            Chunk ponto2 = new Chunk("SAI"); //Ponto1
            ponto2.Font = tableTitle;
            Phrase Pponto2 = new Phrase(ponto2);
            PdfPCell CPonto2 = new PdfPCell(Pponto2);
            CPonto2.HorizontalAlignment = 1;
            CPonto2.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(CPonto2);

            Chunk ponto3 = new Chunk("ENT"); //Ponto1
            ponto3.Font = tableTitle;
            Phrase Pponto3 = new Phrase(ponto3);
            PdfPCell CPonto3 = new PdfPCell(Pponto3);
            CPonto3.HorizontalAlignment = 1;
            CPonto3.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(CPonto3);

            Chunk ponto4 = new Chunk("SAI"); //Ponto1
            ponto4.Font = tableTitle;
            Phrase Pponto4 = new Phrase(ponto4);
            PdfPCell CPonto4 = new PdfPCell(Pponto4);
            CPonto4.HorizontalAlignment = 1;
            CPonto4.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(CPonto4);

            Chunk assinatura = new Chunk("Assinatura"); //Ponto1
            assinatura.Font = tableTitle;
            Phrase Passinatura = new Phrase(assinatura);
            PdfPCell Cassinatura = new PdfPCell(Passinatura);
            Cassinatura.HorizontalAlignment = 1;
            Cassinatura.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(Cassinatura);

            foreach (var itens in registro)
            {

            //Conteúdo Data
            PdfPCell cellConteudoDescricao = new PdfPCell(new Phrase(itens.data.ToShortDateString()));
            cellConteudoDescricao.HorizontalAlignment = 1;
            table.AddCell(cellConteudoDescricao);

            //Conteúdo Ponto1
            PdfPCell cellConteudoPonto1 = new PdfPCell(new Phrase(itens.ponto1.ToShortTimeString()));
            cellConteudoPonto1.HorizontalAlignment = 1;
            table.AddCell(cellConteudoPonto1);
            //Conteúdo Ponto2
            PdfPCell cellConteudoPonto2 = new PdfPCell(new Phrase(itens.ponto2.ToShortTimeString()));
            cellConteudoPonto2.HorizontalAlignment = 1;
            table.AddCell(cellConteudoPonto2);
            //Conteúdo Ponto3
            PdfPCell cellConteudoPonto3 = new PdfPCell(new Phrase(itens.ponto3.ToShortTimeString()));
            cellConteudoPonto3.HorizontalAlignment = 1;
            table.AddCell(cellConteudoPonto3);
            //Conteúdo Ponto4
            PdfPCell cellConteudoPonto4 = new PdfPCell(new Phrase(itens.ponto4.ToShortTimeString()));
            cellConteudoPonto4.HorizontalAlignment = 1;
            table.AddCell(cellConteudoPonto4);
            //Conteúdo Assinatura
            PdfPCell cellAssinatura = new PdfPCell(new Phrase(""));
            cellAssinatura.HorizontalAlignment = 1;
            table.AddCell(cellAssinatura);
            }
            doc.Add(table);

            //Pula linha
            doc.Add(Chunk.NEWLINE);

            //Tabela Rodapé
            PdfPTable tableFooter = new PdfPTable(2);
            tableFooter.TotalWidth = 350f;
            tableFooter.LockedWidth = true;
            float[] widths4 = new float[] { 250f, 100f };
            tableFooter.SetWidths(widths4);

            //Dados do Responsável
            Chunk visto = new Chunk("Visto Responsável:______________");
            Phrase Pvisto = new Phrase(visto);
            PdfPCell cVisto = new PdfPCell(Pvisto);
            cVisto.HorizontalAlignment = 0;
            cVisto.Border = 0;
            tableFooter.AddCell(cVisto);

            Chunk visto2 = new Chunk("____/____/____");
            Phrase Pvisto2 = new Phrase(visto2);
            PdfPCell cVisto2 = new PdfPCell(Pvisto2);
            cVisto2.HorizontalAlignment = 0;
            cVisto2.Border = 0;
            tableFooter.AddCell(cVisto2);
            doc.Add(tableFooter);

            doc.Close();

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/pdf", "Folha_Registro - " + Acesso.UsuarioLogado() + ".pdf");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Pesquisa(string dataInicial, string dataFinal, string usuario)
        {

            var objs = new RegistroPonto().ObterTodosDataUsuario(DateTime.Parse(dataInicial), DateTime.Parse(dataFinal), int.Parse(usuario), _paramBase);
            objs = objs.Where(p => p.SituacaoAprovacao != null).OrderBy(p => p.data).ToList();

            var rets = new List<RetornoPontoAprovacao>();

            foreach (var item in objs)
            {
                var auxComentarios = "";
                if (item.comentarios != null)
                    auxComentarios = item.comentarios;

                rets.Add(new RetornoPontoAprovacao
                {
                    id = item.id,
                    usuario = item.apontador.nome,
                    data = item.data.ToShortDateString(),
                    ponto1 = item.ponto1.ToShortTimeString(),
                    ponto2 = item.ponto2.ToShortTimeString(),
                    ponto3 = item.ponto3.ToShortTimeString(),
                    ponto4 = item.ponto4.ToShortTimeString(),
                    ponto5 = item.ponto5.ToShortTimeString(),
                    ponto6 = item.ponto6.ToShortTimeString(),
                    ponto7 = item.ponto7.ToShortTimeString(),
                    ponto8 = item.ponto8.ToShortTimeString(),
                    comentarios = auxComentarios
                }
                );
            }
            return Json(rets, JsonRequestBehavior.AllowGet);
        }
    }
}

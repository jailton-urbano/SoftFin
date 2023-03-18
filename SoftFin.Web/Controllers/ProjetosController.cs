using iTextSharp.text;
using iTextSharp.text.pdf;
using SoftFin.Utils;
using SoftFin.Web.Classes;
using SoftFin.Web.Controllers;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Controllers
{
    public class ProjetosController : BaseController
    {
        public ActionResult Excel()
        {
            var obj = new Projeto();
            var lista = obj.ObterTodos(_paramBase);
            CsvExport myExport = new CsvExport();
            foreach (var item in lista)
            {
                myExport.AddRow();
                myExport["id"] = item.id;
                if (item.ProjetoPai == null)
                    myExport["ProjetoPai"] = "";
                else
                    myExport["ProjetoPai"] = item.ProjetoPai.codigoProjeto + " " + item.ProjetoPai.nomeProjeto;
                myExport["codigoProjeto"] = item.codigoProjeto;
                myExport["nomeProjeto"] = item.nomeProjeto;
                myExport["dataInicial"] = item.dataInicial;
                myExport["dataFinal"] = item.dataFinal;
                myExport["totalHoras"] = item.totalHoras;
            }
            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();
            return File(myCsvData, "application/vnd.ms-excel", "Projeto.csv");
        }

        public FileStreamResult PDF(int id)
        {
            var projetos = new Projeto().ObterPorId(id, _paramBase);
            int estab = _estab;
            MemoryStream stream = new MemoryStream();

            Document doc = new Document();
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);
            pdfWriter.CloseStream = false;

            doc.Open();

            //Carrega Logo
            //try
            //{
            //    var request = WebRequest.Create(_estabobj.Logo);
            //    using (var response = request.GetResponse())
            //    {
            //        using (var responseStream = response.GetResponseStream())
            //        {
            //            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("https://sistemahom.blob.core.windows.net/compartilhado/Logo/Logo_" + estab + ".jpg");
            //            logo.ScalePercent(50f);
            //            //logo.SetAbsolutePosition(70f, 10f);
            //            doc.Add(logo);
            //        }
            //    }
            //}
            //catch (WebException ex)
            //{
            //    if (ex.Status == WebExceptionStatus.ProtocolError &&
            //        ex.Response != null)
            //    {
            //        var resp = (HttpWebResponse)ex.Response;
            //        if (resp.StatusCode == HttpStatusCode.NotFound)
            //        {
            //            // Do something
            //        }
            //        else
            //        {
            //            // Do something else
            //        }
            //    }
            //    else
            //    {
            //        // Do something else
            //    }
            //}


            //Pula linha
            doc.Add(Chunk.NEWLINE);

            //Título
            PDFTitulo(doc, "Ficha Projeto", 24);

            //Pula linha
            doc.Add(Chunk.NEWLINE);

            //Tabela dados do Estabelecimento e dados da ND
            PdfPTable tableHeader = new PdfPTable(2);
            tableHeader.TotalWidth = 500f;
            tableHeader.LockedWidth = true;
            float[] widths = new float[] { 300f, 200f };
            tableHeader.SetWidths(widths);

            //Dados do Estabelecimento
            Chunk nome = new Chunk(projetos.Estabelecimento.NomeCompleto);
            Chunk cnpj = new Chunk("CNPJ: " + projetos.Estabelecimento.CNPJ);
            Chunk endereco = new Chunk("Endereço: " + projetos.Estabelecimento.Logradouro + ", "
                                               + projetos.Estabelecimento.NumeroLogradouro.ToString()
                );
            Chunk municipio = new Chunk(projetos.Estabelecimento.Municipio.DESC_MUNICIPIO + " - " + projetos.Estabelecimento.UF);
            Chunk bairro = new Chunk(projetos.Estabelecimento.BAIRRO + " - " + projetos.Estabelecimento.CEP);

            //Dados da Nota de Débito
            Font verdanaND = FontFactory.GetFont("Verdana", 8,Font.BOLD, BaseColor.BLACK);
            Chunk numero = new Chunk("Projeto: " + projetos.codigoProjeto + "(" + projetos.codigoProjeto + ")");
            numero.SetUnderline(1f, -2f);
            numero.Font = verdanaND;
            Chunk dtaini = new Chunk("Data Inicial : " + projetos.dataInicial.ToString("dd/MMM/yyyy"));
            Chunk dtafim = new Chunk("Data Final : " + projetos.dataFinal.ToString("dd/MMM/yyyy"));
            Chunk totalHoras = new Chunk("Total Horas: " + projetos.totalHoras);

            //Célula do Estabelecimento #1
            PdfPCell c1 = new PdfPCell(new Phrase(nome));
            c1.HorizontalAlignment = 0;
            c1.Border = 0;
            tableHeader.AddCell(c1);

            //Célula dados da ND #1
            PdfPCell c2 = new PdfPCell(new Phrase(numero));
            c2.HorizontalAlignment = 2;
            c2.Border = 0;
            tableHeader.AddCell(c2);

            //Célula do Estabelecimento #2
            PdfPCell c3 = new PdfPCell(new Phrase(cnpj));
            c3.HorizontalAlignment = 0;
            c3.Border = 0;
            tableHeader.AddCell(c3);

            //Célula dados da ND #2
            PdfPCell c4 = new PdfPCell(new Phrase(dtaini));
            c4.HorizontalAlignment = 2;
            c4.Border = 0;
            tableHeader.AddCell(c4);

            //Célula do Estabelecimento #3
            PdfPCell c5 = new PdfPCell(new Phrase(endereco));
            c5.HorizontalAlignment = 0;
            c5.Border = 0;
            tableHeader.AddCell(c5);

            //Célula dados da ND #3
            PdfPCell c6 = new PdfPCell(new Phrase(dtafim));
            c6.HorizontalAlignment = 2;
            c6.Border = 0;
            tableHeader.AddCell(c6);

            //Célula do Estabelecimento #4
            PdfPCell c7 = new PdfPCell(new Phrase(municipio));
            c7.HorizontalAlignment = 0;
            c7.Border = 0;
            tableHeader.AddCell(c7);

            //Célula do Estabelecimento #5
            PdfPCell c9 = new PdfPCell(new Phrase(bairro));
            c9.HorizontalAlignment = 0;
            c9.Border = 0;
            tableHeader.AddCell(c9);

            //Célula dados da ND #5
            PdfPCell c10 = new PdfPCell(new Phrase(""));
            c10.HorizontalAlignment = 2;
            c10.Border = 0;
            tableHeader.AddCell(c10);

            doc.Add(tableHeader);

            //Pula linhas
            doc.Add(Chunk.NEWLINE);

            PDFTitulo(doc, "Dados do Projeto", 18);
            //Pula duas linhas
            doc.Add(Chunk.NEWLINE);
            PDFDados(projetos, doc);

            doc.Add(Chunk.NEWLINE);

            PDFTitulo(doc, "Despesas", 18);
            //Pula duas linhas
            doc.Add(Chunk.NEWLINE);

            PDFDespesas(projetos, doc);

            doc.Close();

            stream.Flush();
            stream.Position = 0;

            return File(stream, "application/pdf", "Projeto - " + projetos.codigoProjeto + ".pdf");
        }


        private void PDFDados(Projeto projeto, Document doc)
        {
            //Corpo da ND
            PdfPTable table = new PdfPTable(6);
            table.TotalWidth = 500f;
            table.LockedWidth = true;
            float[] widths5 = new float[] { 100f, 100f, 100f, 100f, 100f, 100f };
            table.SetWidths(widths5);

            //Fonte do Título da Tabela
            Font tableTitle = FontFactory.GetFont("Verdana", 9, Font.BOLD, BaseColor.WHITE);

            //Título Descrição
            PDFTituloCelula(table, tableTitle, "Valor Projeto", 2);
            PDFTituloCelula(table, tableTitle, "Horas Apontadas", 2);
            PDFTituloCelula(table, tableTitle, "Despesas Diretas", 2);
            PDFTituloCelula(table, tableTitle, "Custo Total", 2);
            PDFTituloCelula(table, tableTitle, "Margem", 2);
            PDFTituloCelula(table, tableTitle, "Saldo", 2);

            Decimal valorProjeto = 0;
            if (projeto.ContratoItem_id != null)
                valorProjeto = projeto.ContratoItem.valor;

            PDFCelula(table, valorProjeto.ToString("n"), 2);

            ApontamentoDiario ad = new ApontamentoDiario();
            Decimal horasApontadas = ad.ObterTotalHorasProjeto(projeto.id, _paramBase);
            PDFCelula(table, horasApontadas.ToString("n"), 2);

            var cpag = new DocumentoPagarParcela().ObterPorProjeto(_paramBase, projeto.id);

            Decimal despesasDiretas = 0;

            foreach (var item in cpag)
            {
                var participacaoRateio = new DocumentoPagarProjeto().ObterPorCPAG(item.DocumentoPagarMestre_id);
                var num = participacaoRateio.Where(p => p.Projeto_Id == projeto.id).FirstOrDefault();
                var porcentagem = Math.Round((num.Valor / item.DocumentoPagarMestre.valorBruto) * 100, 2);
                despesasDiretas += Math.Round((item.valor / 100) * porcentagem,2);
            }

            PDFCelula(table, despesasDiretas.ToString("n"), 2);

            Decimal custoProjeto = CalculaCustoProjeto(projeto.id);
            PDFCelula(table, custoProjeto.ToString("n"), 2);

            var margem = valorProjeto -(custoProjeto + despesasDiretas);
            PDFCelula(table, margem.ToString("n"), 2);

            var saldo = projeto.totalHoras - horasApontadas;
            PDFCelula(table, saldo.ToString("n"), 2);
   

            doc.Add(table);
        }

        public Decimal CalculaCustoProjeto(int projeto)
        {
            DbControle db = new DbControle();
            ApontamentoDiario ad = new ApontamentoDiario();
            List<ApontamentoDiario> lista = ad.ObterTodosProjeto(projeto, _paramBase);

            Decimal custo = 0;
            for (int i = 0; i < lista.Count(); i++)
            {
                custo = custo + (lista[i].qtdHoras * ObtemCustoApontador(lista[i].apontador.id));
            }
            return custo;
        }

        public Decimal ObtemCustoApontador(int apontador)
        {
            int estab = _paramBase.estab_id;
            DbControle db = new DbControle();
            ProjetoUsuario pu = new ProjetoUsuario().ObterCategoriaApontador(apontador, _paramBase);
            var tx = db.TaxaHora.Where(x => x.categoria_id == pu.categoria_id);
            if (tx.Count() == 0)
                return 0;
            else

                return tx.Sum(x => x.taxaHoraCusto);
        }
        private void PDFDespesas(Projeto projeto, Document doc)
        {
            //Corpo da ND
            PdfPTable table = new PdfPTable(8);
            table.TotalWidth = 500f;
            table.LockedWidth = true;
            float[] widths5 = new float[] { 50f, 20f, 45f, 100f, 40f, 50f, 25f, 50f };
            table.SetWidths(widths5);

            //Fonte do Título da Tabela
            Font tableTitle = FontFactory.GetFont("Verdana", 9, Font.BOLD, BaseColor.WHITE);

            //Título Descrição
            PDFTituloCelula(table, tableTitle, "Documento", 0);
            PDFTituloCelula(table, tableTitle, "Parc", 1);
            PDFTituloCelula(table, tableTitle, "Vencimento", 1);
            PDFTituloCelula(table, tableTitle, "Descrição", 1);
            PDFTituloCelula(table, tableTitle, "Situação", 1);
            PDFTituloCelula(table, tableTitle, "Valor", 2);
            PDFTituloCelula(table, tableTitle, "%", 1);
            PDFTituloCelula(table, tableTitle, "Valor Rateio", 2);

            var docs = new DocumentoPagarParcela().ObterPorProjeto(_paramBase, projeto.id);

            foreach (var item in docs)
            {
                //var participacaoRateio = item.DocumentoPagarMestre.DocumentoPagarProjetos.Where(p => p.Projeto_Id == projeto.id).FirstOrDefault();
                var participacaoRateio = new DocumentoPagarProjeto().ObterPorCPAG(item.DocumentoPagarMestre_id);
                var num = participacaoRateio.Where(p => p.Projeto_Id == projeto.id).FirstOrDefault();

                var porcentagem = Math.Round((num.Valor / item.DocumentoPagarMestre.valorBruto )  * 100 ,2);

                PDFCelula(table, item.DocumentoPagarMestre.numeroDocumento.ToString(), 0);
                PDFCelula(table, item.parcela.ToString(), 1);
                PDFCelula(table, item.vencimentoPrevisto.ToString("dd/MMM/yyyy"), 1);

                PDFCelula(table, item.historico, 0);

                if (item.statusPagamento == 1)
                    PDFCelula(table, "Em Aberto", 1);

                if (item.statusPagamento == 2)
                    PDFCelula(table, "Pago Parcial", 1);

                if (item.statusPagamento == 3)
                    PDFCelula(table, "Pago Total", 1);
                var valor = item.valor;
                var valorPor = (item.valor / 100) * porcentagem;
                PDFCelula(table, "R$ " + valor.ToString("n"), 2);
                PDFCelula(table, porcentagem.ToString("n"), 2);
                PDFCelula(table, "R$ " + valorPor.ToString("n"), 2);
            }

            PDFTituloCelula(table, tableTitle, "", 1);
            PDFTituloCelula(table, tableTitle, "", 1);
            PDFTituloCelula(table, tableTitle, "", 1);
            PDFTituloCelula(table, tableTitle, "", 1);
            PDFTituloCelula(table, tableTitle, "Total", 1);
            PDFCelula(table, "R$ " + docs.Sum(p => p.valor).ToString("n"), 2);



            doc.Add(table);
        }

        private static void PDFTituloCelula(PdfPTable table, Font tableTitle, string titulo, int alinhamento)
        {
            Chunk desc = new Chunk(titulo);
            desc.Font = tableTitle;
            Phrase descP = new Phrase(desc);
            PdfPCell cellDescricao = new PdfPCell(descP);
            cellDescricao.HorizontalAlignment = alinhamento;
            cellDescricao.BackgroundColor = iTextSharp.text.BaseColor.BLACK;
            table.AddCell(cellDescricao);
        }

        private static void PDFCelula(PdfPTable table, string texto, int alinhamento)
        {
            Font verdana = FontFactory.GetFont("Verdana", 8, Font.NORMAL, BaseColor.DARK_GRAY); 
            PdfPCell cellConteudonumeroDocumento = new PdfPCell(new Phrase(texto, verdana));
            
            cellConteudonumeroDocumento.HorizontalAlignment = alinhamento;
            
            table.AddCell(cellConteudonumeroDocumento);
        }

        private static void PDFTitulo(Document doc, string titulo, int tamanho)
        {
            Font verdanaTitle = FontFactory.GetFont("Verdana", tamanho, Font.BOLD, BaseColor.DARK_GRAY);
            Chunk title = new Chunk(titulo);
            title.SetUnderline(1f, -2f);
            title.Font = verdanaTitle;
            Phrase f1 = new Phrase();
            f1.Add(title);
            Paragraph p1 = new Paragraph();
            p1.Add(f1);
            p1.Alignment = 1;
            doc.Add(p1);
        }

        [HttpPost]
        public override JsonResult TotalizadorDash(int? id)
        {
            base.TotalizadorDash(id);
            var soma = new Projeto().ObterTodos(_paramBase).Where(p => p.ativo).Count();
            return Json(new { CDStatus = "OK", Result = "Ativos: " + soma }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult ObterTodos()
        {
            var objs = new Projeto().ObterTodos(_paramBase).ToList();


            return Json(objs.Select(p => new
            {
                p.id,
                p.ativo,
                p.codigoProjeto,
                p.ContratoItem_id,
                dataInicial = p.dataInicial.ToString("o"),
                dataFinal = p.dataFinal.ToString("o"),
                p.estabelecimento_id,
                p.nomeProjeto,
                p.projeto_id,
                p.totalHoras,
                nome = (p.ContratoItem != null) ? p.ContratoItem.Contrato.Pessoa.nome : "",
                contrato = (p.ContratoItem != null) ?  p.ContratoItem.Contrato.descricao : "",
                contratoItem = (p.ContratoItem != null) ? p.ContratoItem.pedido : "" ,
                valoritem = (p.ContratoItem != null) ? p.ContratoItem.valor : 0,
                ProjetoUsuarios = p.ProjetoUsuarios.OrderBy(e => e.Usuario.nome).Select(f => new
                {
                    f.id,
                    f.projeto_id,
                    f.selecionado,
                    f.usuario_id,
                    nome = f.Usuario.nome,
                    f.categoria_id,
                    categoria = f.categoriaProfissional.descricao,
                    f.Projeto.estabelecimento_id
                })
            }), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult ObterPorID(int id)
        {
            var obj = new Projeto().ObterPorId(id,_paramBase);

            if (obj == null)
            {
                obj = new Projeto();
                obj.estabelecimento_id = _estab;
                obj.dataInicial = DateTime.Now;
                obj.dataFinal = DateTime.Now.AddDays(30);
                obj.totalHoras = 0;
            }

            var usuarios = new Usuario().ObterTodosUsuariosAtivos(_paramBase);
            foreach (var item in usuarios)
            {
                if (obj.ProjetoUsuarios.Where(p => p.usuario_id == item.id).Count() == 0)
                {
                    obj.ProjetoUsuarios.Add(new ProjetoUsuario
                    {
                        categoria_id = 0,
                        id = 0,
                        usuario_id = item.id,
                        projeto_id = id,
                        Usuario = item
                    });
                }
                else
                {
                    obj.ProjetoUsuarios.Where(p => p.usuario_id == item.id).First().selecionado = true;
                }
            }
                

            

            return Json( new
            {
                obj.id,
                obj.ativo,
                obj.codigoProjeto,
                obj.ContratoItem_id,
                dataInicial = obj.dataInicial.ToString("o"),
                dataFinal = obj.dataFinal.ToString("o"),
                obj.estabelecimento_id,
                obj.nomeProjeto,
                obj.projeto_id,
                obj.totalHoras,
                ContratoId = (obj.ContratoItem != null) ? obj.ContratoItem.contrato_id : 0,
                ProjetoUsuarios = obj.ProjetoUsuarios.OrderBy(p => p.Usuario.nome).Select(p => new 
                {
                    p.id,
                    p.projeto_id,
                    p.selecionado,
                    p.usuario_id,
                    p.Usuario.nome,

                    p.categoria_id
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterProjetos()
        {
            var objs = new Projeto().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.nomeProjeto + ((p.ContratoItem == null) ? "": "(" + p.ContratoItem.Contrato.Pessoa.nome + ")")
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterContratos()
        {
            var objs = new Contrato().ObterTodos(_paramBase).OrderBy(p => p.descricao);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.descricao + " (" + p.Pessoa.nome + ")"
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterContratosItem(int id)
        {
            var objs = new ContratoItem().ObterTodos(_paramBase, id);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.pedido
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObterCategoriaProfissional()
        {
            var objs = new CategoriaProfissional().ObterTodos(_paramBase);


            return Json(objs.Select(p => new
            {
                Value = p.id,
                Text = p.categoria
            }), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult Salvar(Projeto obj)
        {
            try
            {
                var objErros = obj.Validar(ModelState);

                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

                //if (obj.ContratoItem_id == null)
                //    return Json(new { CDMessage = "NOK", DSMessage = "Contrato não encontrado" }, JsonRequestBehavior.AllowGet);

                if (objErros.Count() > 0)
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel salvar.", Erros = objErros });
                }

                DbControle db = new DbControle();


                var projetoUsuarios = obj.ProjetoUsuarios;
                obj.ProjetoUsuarios = null;

                foreach (var item in projetoUsuarios)
                {
                    if (item.selecionado)
                    {
                        if (item.categoria_id == 0)
                            return Json(new { CDStatus = "NOK", DSMessage = "Escolha categoria para os recursos selecionados.", Erros = objErros });
                    }
                }

                if (obj.id == 0)
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {


                        if (obj.Incluir(_paramBase, db))
                        {
                            IncluirRelacionamentoUsuario( db, obj, projetoUsuarios);

                            dbcxtransaction.Commit();
                            ViewBag.msg = "Incluído com sucesso";
                            return Json(new { CDStatus = "OK", DSMessage = "Incluído com sucesso" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            dbcxtransaction.Rollback();
                            return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, já este registro já esta cadastrado" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                else
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {




                        if (obj.Alterar(_paramBase, db))
                        {
                            AlterRelacionamentoUsuario(db, obj, projetoUsuarios);

                            dbcxtransaction.Commit();
                            ViewBag.msg = "Incluído com sucesso";
                            return Json(new { CDStatus = "OK", DSMessage = "Salvo com sucesso" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            dbcxtransaction.Rollback();
                            return Json(new { CDStatus = "NOK", DSMessage = "Impossivel salvar, já este registro já esta cadastrado" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
       
        private void IncluirRelacionamentoUsuario( DbControle db, Projeto objPrinc, List<ProjetoUsuario> projetoUsuarios)
        {
            
            foreach (var item in projetoUsuarios)
            {
                if (item.selecionado)
                {
                    var rel = new ProjetoUsuario();
                    rel.projeto_id = objPrinc.id;
                    rel.usuario_id = item.usuario_id;
                    rel.categoria_id = item.categoria_id;
                    rel.Incluir(_paramBase, db);
                }
            }
        }

        private void AlterRelacionamentoUsuario( DbControle db, Projeto objPrinc, List<ProjetoUsuario> pu )
        {
            var listaAnterior = new ProjetoUsuario().ObterTodosPorProjeto(objPrinc.id, _paramBase, db);

            foreach (var item in pu)
            {
                if (!item.selecionado)
                {
                    if (listaAnterior.Where(p => p.usuario_id == item.usuario_id).Count() > 0)
                    {
                        item.Excluir(_paramBase, db);
                    }
                }
                else
                {
                    var aux = listaAnterior.Where(p => p.usuario_id == item.usuario_id).FirstOrDefault();
                    if (aux == null)
                    {
                        var rel = new ProjetoUsuario();
                        rel.projeto_id = objPrinc.id;
                        rel.usuario_id = item.usuario_id;
                        rel.categoria_id = item.categoria_id;
                        rel.Incluir(_paramBase, db);
                    }
                    else
                    {

                        aux.projeto_id = objPrinc.id;
                        aux.usuario_id = item.usuario_id;
                        aux.categoria_id = item.categoria_id;
                        aux.Alterar( aux, db);

                    }
                }
            }
        }
        
        private void ExcluirRelacionamentoUsuario(DbControle db, Projeto objPrinc)
        {
            var listaAnterior = new ProjetoUsuario().ObterTodosPorProjeto(objPrinc.id, _paramBase, db);

            foreach (var item in listaAnterior)
            {
                item.Excluir(_paramBase, db);
            }
        }
        
        public JsonResult Excluir(Projeto obj)
        {

            try
            {
                DbControle db = new DbControle();

                if (obj.estabelecimento_id != _estab)
                    return Json(new { CDMessage = "NOK", DSMessage = "Erro! Recarregue a tela estabelecimento inválido, pode ter sido trocado em outra aba" }, JsonRequestBehavior.AllowGet);

                var objBanco = new Projeto().ObterPorId(obj.id, db, _paramBase);

                string Erro = "";

                ExcluirRelacionamentoUsuario(db, objBanco);
                if (objBanco.Excluir(obj.id, ref Erro, _paramBase))
                {
                    

                    return Json(new { CDStatus = "OK", DSMessage = "Ordem de venda excluida com sucesso" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CDStatus = "NOK", DSMessage = "Não foi possivel excluir" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _eventos.Error(ex);
                return Json(new { CDStatus = "NOK", DSMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        
    

    }
}

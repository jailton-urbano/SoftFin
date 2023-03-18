using SoftFin.GestorProcessos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.GestorProcessos.Controllers
{
    public class ABDController : BaseController
    {
        DBGPControle _db = new DBGPControle();
        SQLManipulation _sQLManipulation = new SQLManipulation();
        // GET: ABD
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Execute()
        {

            CriaTabelas();

            CriaCampos();

            CriarProceduresAtividade();

            CriarProceduresTabelas();

            return RedirectToAction("Index");
        }

        private void CriarProceduresTabelas()
        {
            var tabelas = _db.Tabela.ToList().Where(p => p.CadastroAuxiliar == true);
            foreach (var item in tabelas)
            {
                var tabelaCampos = _db.TabelaCampo.Where(p => p.Tabela_Id == item.Id).ToList();
                tabelaCampos.Add(new 
                    Models.TabelaCampo { Ativo = true, Campo = "Id", Descricao = "Id", TamanhoCampo = "8", Obrigatorio = true, TipoCampo = _db.TipoCampo.Where(p => p.TipoBancoDados.ToUpper().Equals("INT")).FirstOrDefault() });

                _sQLManipulation.CriarProcedureTabelaObter(item.Nome, tabelaCampos, _db);
                _sQLManipulation.CriarProcedureTabelaSalvar(item.Nome, tabelaCampos, _db);
               // _sQLManipulation.CriarProcedureTabelaExcluir(item.Nome, tabelaCampos, _db);
                //_sQLManipulation.CriarProcedureIncluir(item.Codigo, atividadeVisao, _db);
                //_sQLManipulation.CriarProcedureNovo(item.Codigo, atividadeVisao, _db);
                //_sQLManipulation.CriarProcedureIncluir(item.Codigo, atividadeVisao, _db);



            }
        }

        private void CriarProceduresAtividade()
        {
            var atividades = _db.Atividade.ToList();
            foreach (var item in atividades)
            {
                var atividadeVisao = _db.AtividadeVisao.Where(p => p.IdAtividade == item.Id).ToList();
                _sQLManipulation.CriarProcedureNovo(item.Codigo, atividadeVisao, _db);
                _sQLManipulation.CriarProcedureIncluir(item.Codigo, atividadeVisao, _db);
                _sQLManipulation.CriarProcedureObter(item.Codigo, atividadeVisao, _db);
            }
        }

        private void CriaCampos()
        {
            var campos = _db.TabelaCampo.ToList();
            


            foreach (var item in campos)
            {
                var dadosTabela = _sQLManipulation.ExisteCampo(item.Tabela.Nome, item.Campo);


                var tipo = item.TipoCampo.TipoBancoDados.Replace("#numpos", item.TamanhoCampo).Replace("#numpre", item.Precisao.ToString());


                if (dadosTabela == null)
                {
                    _sQLManipulation.CriaCampo(item.Tabela.Nome, item.Campo, tipo);
                    
                }
                else
                {
                    _sQLManipulation.AlterarCampo(item.Tabela.Nome, item.Campo, tipo);
                }

                if (item.IdChaveEstrageira != null)
                {
                    _sQLManipulation.ExcluiIndice(item.Tabela.Nome, item.Campo, item.ChaveEstrageira.Nome);
                    _sQLManipulation.CriaIndice(item.Tabela.Nome, item.Campo, item.ChaveEstrageira.Nome);
                }

            }
        }

        private void CriaTabelas()
        {
            List<Models.Tabela> tabelas = new List<Models.Tabela>();
            tabelas = _db.Tabela.ToList();
            foreach (var item in tabelas)
            {
                if (!_sQLManipulation.ExisteTabela(item.Nome))
                {
                    _sQLManipulation.CriaTabela(item.Nome, item.CadastroAuxiliar);
                }
            }
        }
    }
}

using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class Funcionario: BaseModels
    {

        [Key]
        public int id { get; set; }

        [JsonIgnore,ForeignKey("pessoa_id")]
        public virtual Pessoa Pessoa { get; set; }

        [Display(Name = "Pessoa"), Required(ErrorMessage = "informe a Pessoa")]
        public int pessoa_id { get; set; }

        [JsonIgnore,ForeignKey("responsavel_id")]
        public virtual Pessoa Responsavel { get; set; }

        [Display(Name = "Responsável"), Required(ErrorMessage = "informe o Responsável")]
        public int responsavel_id { get; set; }

        [Display(Name = "Data de Admissão")]
        public DateTime? dataAdmissao { get; set; }

        [Display(Name = "Unidade de Negócio")]
        public int? unidadeNegocio_id { get; set; }

        [JsonIgnore,ForeignKey("unidadeNegocio_id")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }

        [Display(Name = "Data Saída")]
        public DateTime? dataSaida { get; set; }

        [Display(Name = "Data de Nascimento")]
        public DateTime? dataNascimento { get; set; }

        [Display(Name = "Função")]
        public int? funcao_id { get; set; }

        [JsonIgnore,ForeignKey("funcao_id")]
        public virtual FuncionarioFuncao Funcao { get; set; }

        [Display(Name = "Profissão"),MaxLength(50)]
        public string profissao { get; set; }

        [Display(Name = "Estado Civil"), MaxLength(20)]
        public string estadocivil { get; set; }

        public bool Excluir(ParamBase _paramBase, ref string erro)
        {
            return Excluir(this.id,  _paramBase, ref erro);
        }
        public bool Excluir(int id, ParamBase pb, ref string erro)
        {
            try
            {

                DbControle db = new DbControle();
                var obj = ObterPorId(id,pb, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", db, pb);
                    db.Funcionario.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("FK") > 0)
                {
                    erro = "Registro esta relacionado com outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }
        }
        public bool Alterar(ParamBase _paramBase)
        {
            return Alterar(this, _paramBase);
        }
        public bool Alterar(Funcionario obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id, pb);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "",db, pb);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }
        private bool validaExistencia(DbControle db, Funcionario obj, ParamBase _paramBase)
        {

            return (ObterPorPessoa_id(obj.pessoa_id, _paramBase) != null);
        }
        public bool Incluir(ParamBase _paramBase)
        {
            return Incluir(this, _paramBase);
        }
        public bool Incluir(Funcionario obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj, pb))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Funcionario>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public Funcionario ObterPorId(int id, ParamBase _paramBase)
        {
            return ObterPorId(id, null);
        }
        public Funcionario ObterPorId(int id, ParamBase _paramBase, DbControle banco)
        {
            int idempresa = _paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.Funcionario.Where(x => x.id == id && x.Pessoa.empresa_id == idempresa).FirstOrDefault();
        }

        public Funcionario ObterPorPessoa_id(int id, ParamBase _paramBase)
        {
            return ObterPorPessoa_id(id, _paramBase, null);
        }
        public Funcionario ObterPorPessoa_id(int id, ParamBase _paramBase, DbControle banco)
        {
            int idempresa = _paramBase.empresa_id;
            if (banco == null)
                banco = new DbControle();

            return banco.Funcionario.Where(x => x.pessoa_id == id && x.Pessoa.empresa_id == idempresa).FirstOrDefault();
        }
        public List<Funcionario> ObterTodos( ParamBase _paramBase)
        {
            int idempresa = _paramBase.empresa_id;
            var banco = new DbControle();

            return banco.Funcionario.Where(x => x.Pessoa.empresa_id == idempresa && (
                x.Pessoa.TipoPessoa.Descricao == "Colaborador" || x.Pessoa.TipoPessoa.Descricao == "Funcionário"
                )).ToList();
                
                
        }
        public List<Funcionario> ObterTodos2( ParamBase _paramBase)
        {
            int idempresa = _paramBase.empresa_id;
            var banco = new DbControle();

            return banco.Funcionario.Where(x => x.Pessoa.empresa_id == idempresa).ToList();


        }
        public SelectList ObterColaboradores(ParamBase _paramBase)
        {
            var objs = ObterTodos(_paramBase).OrderBy(p => p.Pessoa.nome);
            var items = new List<SelectListItem>();
            foreach (var item in objs)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} ", item.Pessoa.nome) });
            }
            return new SelectList(items, "Value", "Text");
        }

        public SelectList ObterListaTodos(ParamBase _paramBase)
        {
            var bancos = ObterTodos(_paramBase);

            var items = new List<SelectListItem>();

            foreach (var item in bancos)
            {
                items.Add(new SelectListItem() { Value = item.id.ToString(), Text = String.Format("{0} - {1}", item.Pessoa.nome, item.Pessoa.cnpj)});
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;

        }
    }
}
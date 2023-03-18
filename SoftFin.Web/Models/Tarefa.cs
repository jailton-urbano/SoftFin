//using SoftFin.Web.Classes;
//using SoftFin.Web.Negocios;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;

//namespace SoftFin.Web.Models
//{
//    public class Tarefa
//    {

//        public int id { get; set; }

//        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
//        public int estabelecimento_id { get; set; }

//        [JsonIgnore,ForeignKey("estabelecimento_id")]
//        public virtual Estabelecimento Estabelecimento { get; set; }

//        [Display(Name = "Usuario"), Required(ErrorMessage = "*")]
//        public int usuario_id { get; set; }

//        [JsonIgnore,ForeignKey("usuario_id")]
//        public virtual Usuario Usuario { get; set; }

//        [Display(Name = "Data Lançamento"), Required(ErrorMessage = "*")]
//        public DateTime data { get; set; }

//        [Display(Name = "Quantidade Horas"), Required(ErrorMessage = "*")]
//        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
//        public decimal qtdHoras { get; set; }

//        [Display(Name = "Histórico"), Required(ErrorMessage = "*")]
//        public string historico { get; set; }
        
//        [Display(Name = "Aprovador"), Required(ErrorMessage = "*")]
//        public int aprovador_id { get; set; }

//        [Display(Name = "Tarefa"), Required(ErrorMessage = "*")]
//        public int atividade_id { get; set; }

//        [JsonIgnore,ForeignKey("aprovador_id")]
//        public virtual Usuario aprovadorusu { get; set; }

//        [JsonIgnore,ForeignKey("atividade_id")]
//        public virtual Atividade atividade { get; set; }

//        public bool Excluir(ref string erro, ParamBasepb)
//        {
//            return Excluir(this.id, ref erro, pb);
//        }

//        public bool Excluir(int id, ref string erro, ParamBase pb)
//        {
//            try
//            {
//                int estab = pb.estab_id;
//                DbControle db = new DbControle();
//                var obj = ObterPorId(id, db);
//                if (obj == null)
//                {
//                    erro = "Registro não encontrado";
//                    return false;
//                }
//                else
//                {
//                    new LogMudanca().Incluir(obj, "", "",db, pb);
//                    db.Apontamento.Remove(obj);
//                    db.SaveChanges();
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                if (ex.Message.IndexOf("FK") > 0)
//                {
//                    erro = "Registro esta relacionado com outro cadastro";
//                    return false;
//                }
//                else
//                {
//                    throw ex;
//                }
//            }
//        }

//        public bool Alterar()
//        {
//            return Alterar(this);
//        }

//        public bool Alterar(Tarefa obj)
//        {
//            DbControle db = new DbControle();

//            var objAux = ObterPorId(obj.id);
//            if (objAux == null)
//                return false;
//            else
//            {
//                new LogMudanca().Incluir(objAux, "");
//                new LogMudanca().Incluir(obj, "", "",db, pb);
//                db.Entry(obj).State = EntityState.Modified;
//                db.SaveChanges();

//                return true;
//            }
//        }


//        private bool validaExistencia(DbControle db, Tarefa obj)
//        {
//            return (false);
//        }
//        public bool Incluir()
//        {
//            return Incluir(this);
//        }
//        public bool Incluir(Tarefa obj)
//        {
//            DbControle db = new DbControle();

//            if (validaExistencia(db, obj))
//                return false;
//            else
//            {
//                new LogMudanca().Incluir(obj, "", "",db, pb);

//                db.Set<Tarefa>().Add(obj);
//                db.SaveChanges();

//                return true;
//            }
//        }


//        public Tarefa ObterPorId(int id)
//        {
//            return ObterPorId(id, null);
//        }
//        public Tarefa ObterPorId(int id, DbControle db)
//        {
//            int estab = pb.estab_id;
//            if (db == null)
//                db = new DbControle();

//            return db.Apontamento.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
//        }
//        public List<Tarefa> ObterTodos()
//        {
//            int estab = pb.estab_id;
//            DbControle db = new DbControle();
//            return db.Apontamento.Where(x => x.estabelecimento_id == estab).ToList();
//        }




//        public System.Collections.IEnumerable ObterTodosPorUsuario()
//        {
//            int estab = pb.estab_id;
//            DbControle db = new DbControle();
//            int idusuario = Acesso.RetornaIdUsuario(Acesso.UsuarioLogado());
//            return db.Apontamento.Where(x => x.estabelecimento_id == estab && x.atividade_id == idusuario).ToList();
//        }
//    }

//}
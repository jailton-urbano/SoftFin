using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using SoftFin.Web.Negocios;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class Contrato:BaseModels
    {
        public int id { get; set; }
                
        [Display(Name = "Contrato"),
        Required(ErrorMessage = "Informe o Contrato")]
        public string contrato { get; set; }

        [Display(Name = "Descrição"),
        Required(ErrorMessage = "Informe a descrição do Contrato")]
        public string descricao { get; set; }

        [Display(Name = "Valor Total"),
        Required(ErrorMessage = "Informe o valor total do Contrato")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valortotal { get; set; }

        [DisplayName("Data Emissão"),
        Required(ErrorMessage = "Informe a data de emissão do contrato")]
        [DataType(DataType.Date)]
        public DateTime emissao { get; set; }

        [Display(Name = "Prazo de Pagamento"),
        Required(ErrorMessage = "Informe o Prazo do Contrato")]
        public string prazo { get; set; }

        [Display(Name = "Cliente"),
        Required(ErrorMessage = "Infome o Cliente do Contrato")]
        public int pessoas_ID { get; set; }
        [JsonIgnore,ForeignKey("pessoas_ID")]
        public virtual Pessoa Pessoa { get; set; }

        [Display(Name = "Estabelecimento"),
        Required(ErrorMessage = "Informe o Estabelecimento do Contrato")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [NotMapped]
        public string pessoa_nome { get; set; }

        [Display(Name = "Usuario Inclusão")]
        public int? usuarioinclusaoid { get; set; }
        [JsonIgnore,ForeignKey("usuarioinclusaoid")]
        public virtual Usuario UsuarioInclusao { get; set; }
        [Display(Name = "Usuario Alteração")]
        public int? usuarioalteracaoid { get; set; }
        [JsonIgnore,ForeignKey("usuarioalteracaoid")]
        public virtual Usuario UsuarioAlteracao { get; set; }

        public DateTime? dataInclusao { get; set; }

        public DateTime? dataAlteracao { get; set; }


        public DateTime? DataInicioVigencia { get; set; }

        public DateTime? DataFinalVigencia { get; set; }
                
        public int? MunicipioPrestador_id { get; set; }

        [JsonIgnore, ForeignKey("MunicipioPrestador_id")]
        public virtual Municipio MunicipioPrestador { get; set; }

        public int? Vendedor_id { get; set; }

        [JsonIgnore, ForeignKey("Vendedor_id")]
        public virtual Pessoa Vendedor { get; set; }


        public bool Excluir(int id, ref string erro,ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db,pb);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.Contrato.Remove(obj);
                    db.SaveChanges();
                    return true;
                }
            }

            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
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

        public bool Alterar(Contrato obj,ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id,pb);
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


        private bool validaExistencia(DbControle db, Contrato obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this,pb, db);
        }
        public bool Incluir(Contrato obj, ParamBase pb, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<Contrato>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public Contrato ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public Contrato ObterPorId(int id, DbControle db,ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.Contrato.Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }


        public List<Contrato> ObterTodos(ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();
            return db.Contrato.Where(x => x.estabelecimento_id == estab).Include(p => p.Pessoa).ToList();
        }
        public List<Contrato> ObterTodosPorContrato(ParamBase pb, string contrato, DbControle db)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();
            return db.Contrato.Where(x => x.estabelecimento_id == estab && x.contrato == contrato).Include(p => p.Pessoa).ToList();
        }

        public int Quantidade(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.Contrato.Where(x => x.estabelecimento_id == estab).Count();
        }
    }
}

using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
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
    public class CodigoServicoMunicipio
    {
        public int id { get; set; }


        public int? municipio_id { get; set; }
        [JsonIgnore,ForeignKey("municipio_id")]
        public virtual Municipio municipio { get; set; }

        [Required(ErrorMessage = "Codigo do Serviço obrigatório"), MaxLength(50)]
        public string codigo { get; set; }
        [Required(ErrorMessage = "Descrição do Serviço obrigatório")]
        public string descricao { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal aliquota { get; set; }

        public string grupolei { get; set; }
        [Required(ErrorMessage = "Natureza (PF ou PJ)"), MaxLength(2)]
        public string natureza { get; set; }

        public bool? Ativo { get; set; }


        public bool Excluir(int id, ref string erro, ParamBase pb)
        {
            try
            {
                int estab = pb.estab_id;
                DbControle db = new DbControle();
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.CodigoServicoMunicipio.Remove(obj);
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

        public bool Alterar(CodigoServicoMunicipio obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            var objAux = ObterPorId(obj.id);
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


        private bool validaExistencia(DbControle db, CodigoServicoMunicipio obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(CodigoServicoMunicipio obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<CodigoServicoMunicipio>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public CodigoServicoMunicipio ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public CodigoServicoMunicipio ObterPorId(int id, DbControle db)
        {
            
            if (db == null)
                db = new DbControle();

            return db.CodigoServicoMunicipio.Where(x => x.id == id ).FirstOrDefault();
        }



        public List<CodigoServicoMunicipio> ObterTodos(ParamBase pb)
        {
            DbControle db = new DbControle();
            return db.CodigoServicoMunicipio.Where(x => x.municipio_id == pb.municipio_id).ToList();
        }


    }
}
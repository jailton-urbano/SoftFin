using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class CodigoServico
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Codigo do Serviço obrigatório"), MaxLength(5)]
        public string codigo { get; set; }
        [Required(ErrorMessage = "Descrição do Serviço obrigatório")]
        public string descricao { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal aliquota { get; set; }

        [Display(Name = "Empresa"), Required(ErrorMessage = "*")]
        public int empresa_id { get; set; }
        [JsonIgnore,ForeignKey("empresa_id")]
        public virtual Empresa Empresa { get; set; }


        //public bool Excluir(int id, ref string erro, ParamBase pb)
        //{
        //    try
        //    {
        //        int estab = pb.estab_id;
        //        DbControle db = new DbControle();
        //        var obj = ObterPorId(id, db);
        //        if (obj == null)
        //        {
        //            erro = "Registro não encontrado";
        //            return false;
        //        }
        //        else
        //        {
        //            new LogMudanca().Incluir(obj, "", "",db, pb);
        //            db.CodigoServico.Remove(obj);
        //            db.SaveChanges();
        //            return true;
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
        //        {
        //            erro = "Registro esta relacionado com outro cadastro";
        //            return false;
        //        }
        //        else
        //        {
        //            throw ex;
        //        }
        //    }
        //}

        //public bool Alterar(CodigoServico obj)
        //{
        //    DbControle db = new DbControle();

        //    var objAux = ObterPorId(obj.id);
        //    if (objAux == null)
        //        return false;
        //    else
        //    {
        //        new LogMudanca().Incluir(obj, objAux, "",db, pb);
        //        db.Entry(obj).State = EntityState.Modified;
        //        db.SaveChanges();

        //        return true;
        //    }
        //}


        //private bool validaExistencia(DbControle db, CodigoServico obj)
        //{
        //    return (false);
        //}
        //public bool Incluir()
        //{
        //    return Incluir(this);
        //}
        //public bool Incluir(CodigoServico obj)
        //{
        //    DbControle db = new DbControle();

        //    if (validaExistencia(db, obj))
        //        return false;
        //    else
        //    {
        //        new LogMudanca().Incluir(obj, "", "",db, pb);

        //        db.Set<CodigoServico>().Add(obj);
        //        db.SaveChanges();

        //        return true;
        //    }
        //}


        //public CodigoServico ObterPorId(int id)
        //{
        //    return ObterPorId(id, null);
        //}
        //public CodigoServico ObterPorId(int id, DbControle db)
        //{
        //    int empresa  = pb.empresa_id;
        //    if (db == null)
        //        db = new DbControle();

        //    return db.CodigoServico.Where(x => x.id == id && x.empresa_id == empresa).FirstOrDefault();
        //}
        //public List<CodigoServico> ObterTodos()
        //{
        //    int empresa  = pb.empresa_id;
        //    DbControle db = new DbControle();
        //    return db.CodigoServico.Where(x => x.empresa_id == empresa).ToList();
        //}


    }
}
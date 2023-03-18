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
    public class CodigoServicoEstabelecimento: BaseModels
    {
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Codigo Serviço Prefeitura"), Required(ErrorMessage = "*")]
        public int codigoServicoMunicipio_id { get; set; }
        [JsonIgnore,ForeignKey("codigoServicoMunicipio_id")]
        public virtual CodigoServicoMunicipio CodigoServicoMunicipio { get; set; }




        public bool Excluir(int id, ref string erro, ParamBase pb)
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
                    db.CodigoServicoEstabelecimento.Remove(obj);
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

        public bool Alterar(CodigoServicoEstabelecimento obj, ParamBase pb)
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


        private bool validaExistencia(DbControle db, CodigoServicoEstabelecimento obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb)
        {
            return Incluir(this, pb);
        }
        public bool Incluir(CodigoServicoEstabelecimento obj, ParamBase pb)
        {
            DbControle db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "",db, pb);

                db.Set<CodigoServicoEstabelecimento>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }


        public CodigoServicoEstabelecimento ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null,pb);
        }
        public CodigoServicoEstabelecimento ObterPorId(int id, DbControle db,ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.CodigoServicoEstabelecimento.Where(x => x.id == id && x.estabelecimento_id == estab ).FirstOrDefault();
        }
        public List<CodigoServicoEstabelecimento> ObterTodos(ParamBase pb,int idmunicipio = 0)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            if (idmunicipio == 0)
                return db.CodigoServicoEstabelecimento.Where(x => x.estabelecimento_id == estab).ToList();
            else
                return db.CodigoServicoEstabelecimento.Where(x => x.estabelecimento_id == estab && x.CodigoServicoMunicipio.municipio_id == idmunicipio).ToList();

        }


        public SelectList CarregaCombo(ParamBase pb)
        {
            var con1 = ObterTodos(pb);
            var items = new List<SelectListItem>();
            foreach (var item in con1)
            {
                items.Add(new SelectListItem() { Value = item.CodigoServicoMunicipio.codigo, Text = String.Format("{0} - {1} ", item.CodigoServicoMunicipio.codigo, item.CodigoServicoMunicipio.descricao) });
            }
            var listret = new SelectList(items, "Value", "Text");
            return listret;
        }


    }
}
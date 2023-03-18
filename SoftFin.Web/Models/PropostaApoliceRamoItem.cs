using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{

    public class PropostaApoliceRamoItem
    {
        [Key]
        public int id { get; set; }

        [JsonIgnore,ForeignKey("ramoseguro_id")]
        public virtual RamoSeguro RamoSeguro { get; set; }

        [Display(Name = "Ramo"), Required(ErrorMessage = "*")]
        public int ramoseguro_id { get; set; }

        [JsonIgnore, ForeignKey("PropostaApolice_id")]
        public virtual PropostaApolice PropostaApolice { get; set; }

        [Display(Name = "PropostaApolice"), Required(ErrorMessage = "*")]
        public int PropostaApolice_id { get; set; }
        
        
        [Display(Name = "Valor Prémio"), Required(ErrorMessage = "*")]
        public decimal premio { get; set; }

        [Display(Name = "Número Item"), Required(ErrorMessage = "*")]
        public int NumeroItem { get; set; }
        
        [NotMapped]
        public string ramoseguro_descr
        {
            get { return (RamoSeguro == null) ? null : RamoSeguro.descricao; }
        }

        public string ToJson()
        {
            StringWriter sw = new StringWriter();
            JsonTextWriter writer = new JsonTextWriter(sw);

            writer.WriteStartObject();

            writer.WritePropertyName("id");
            writer.WriteValue(this.id);

            writer.WritePropertyName("NumeroItem");
            writer.WriteValue(this.NumeroItem);

            writer.WritePropertyName("premio");
            writer.WriteValue(this.premio);

            writer.WritePropertyName("PropostaApolice_id");
            writer.WriteValue(this.PropostaApolice_id);

            writer.WritePropertyName("ramoseguro_id");
            writer.WriteValue(this.ramoseguro_id);

            writer.WritePropertyName("ramoseguro_descr");
            writer.WriteValue(this.ramoseguro_descr);

            writer.WriteEndObject();

            return sw.ToString();
        }

        
        public List<string> Validar(System.Web.Mvc.ModelStateDictionary ModelState = null)
        {
            var erros = new List<string>();
            if (ModelState != null)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                erros.AddRange(from a in allErrors select a.ErrorMessage);
            }
            return new List<string>();
        }

        public bool Alterar(PropostaApoliceRamoItem obj, ParamBase pb)
        {
            return Alterar(obj, null);
        }

        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, db, pb);
        }
        public bool Alterar(PropostaApoliceRamoItem obj, DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();




            var objAux = ObterPorId(obj.id,db);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "",db, pb);

                objAux.NumeroItem = obj.NumeroItem;
                objAux.premio = obj.premio;
                objAux.ramoseguro_id = obj.ramoseguro_id;

                db.SaveChanges();

                return true;
            }
        }

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(PropostaApoliceRamoItem obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            //int idempresa  = pb.empresa_id;
            new LogMudanca().Incluir(obj, "", "", banco, pb);
            banco.Set<PropostaApoliceRamoItem>().Add(obj);
            banco.SaveChanges();
            return true;
        }

        public PropostaApoliceRamoItem ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }

        public PropostaApoliceRamoItem ObterPorId(int id, DbControle banco)
        {
            //int idEstab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.PropostaApoliceRamoItem.Where(x => x.id == id ).FirstOrDefault();
        }

        public List<PropostaApoliceRamoItem> ObterTodos(int PropostaApolice_id, DbControle banco= null)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.PropostaApoliceRamoItem.Where(x => x.PropostaApolice_id == PropostaApolice_id).ToList();
        }


        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle banco = null)
        {
            try
            {
                int estab = pb.estab_id;


                if (banco == null)
                    banco = new DbControle();
                var obj = ObterPorId(id, banco);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "", banco, pb);
                    banco.PropostaApoliceRamoItem.Remove(obj);
                    banco.SaveChanges();
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

        
    }
}
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

namespace SoftFin.Web.Models
{
    
    public class PropostaApolice
    {
        public PropostaApolice()
        {
            PropostaApoliceRamoItems = new List<PropostaApoliceRamoItem>();
        }

        [Key]
        public int id { get; set; }
        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }
        
        
        
        [JsonIgnore,ForeignKey("seguradora_id")]
        public virtual Pessoa PessoaSeguradora { get; set; }

        [Display(Name = "Seguradora")]
        public int? seguradora_id { get; set; }

        [JsonIgnore,ForeignKey("grupoRamoSeguro_id")]
        public virtual GrupoRamoSeguro GrupoRamoSeguro { get; set; }

        [Display(Name = "Grupo Ramo")]
        public int? grupoRamoSeguro_id { get; set; }

        
        
        
        [JsonIgnore,ForeignKey("regiao_id")]
        public virtual Regiao Regiao { get; set; }

        [Display(Name = "Região")]
        public int? regiao_id { get; set; }

        [JsonIgnore,ForeignKey("sucursal_id")]
        public virtual Sucursal Sucursal { get; set; }

        [Display(Name = "Sucursal")]
        public int? sucursal_id { get; set; }

        [JsonIgnore,ForeignKey("representante_id")]
        public virtual Pessoa PessoaRepresentante { get; set; }

        [Display(Name = "Representante")]
        public int? representante_id { get; set; }

        [JsonIgnore,ForeignKey("tipoCondicaoPagamentoPremio_id")]
        public virtual TipoCondicaoPagamentoPremio TipoCondicaoPagamentoPremio { get; set; }

        [Display(Name = "Tipo Condição Pagamento Premio")]
        public int? tipoCondicaoPagamentoPremio_id { get; set; }
        

        
        [Display(Name = "Data Registro"), Required(ErrorMessage = "*")]
        public DateTime dataRegistro { get; set; }

        [Display(Name = "Data Proposta"), Required(ErrorMessage = "*")]
        public DateTime dataProposta { get; set; }

        [Display(Name = "Data Efetivação")]
        public DateTime? dataEfetivacaoProposta { get; set; }

        [Display(Name = "Data Vigencia Inicial"), Required(ErrorMessage = "*")]
        public DateTime dataVigenciaInicio { get; set; }

        [Display(Name = "Data Vigencia Fim"), Required(ErrorMessage = "*")]
        public DateTime dataVigenciaFim { get; set; }

        [Display(Name = "Bem Garantia"), Required(ErrorMessage = "*")]
        public Boolean bemGarantia { get; set; }

        [Display(Name = "Observacoes"), Required(ErrorMessage = "*"), MaxLength(250)]
        public string observacoes { get; set; }


        [JsonIgnore,ForeignKey("Segurado_Principal_id")]
        public virtual Pessoa Segurado_Principal { get; set; }

        [Display(Name = "Segurado_Principal_id"), Required(ErrorMessage = "*")]
        public int Segurado_Principal_id { get; set; }

        [NotMapped]
        public string Segurado_Principal_Nome { get; set; }


        [JsonIgnore,ForeignKey("tipoStatusPropostaSeguro_id")]
        public virtual TipoStatusPropostaSeguro TipoStatusPropostaSeguro { get; set; }

        [Display(Name = "Tipo Status Proposta Seguro"), Required(ErrorMessage = "*")]
        public int tipoStatusPropostaSeguro_id { get; set; }


        [JsonIgnore,ForeignKey("tipoPropostaSeguro_id")]
        public virtual TipoPropostaSeguro TipoPropostaSeguro { get; set; }

        [Display(Name = "Tipo Proposta Seguro"), Required(ErrorMessage = "*")]
        public int tipoPropostaSeguro_id { get; set; }



        [Display(Name = "Número"), MaxLength(150)]
        public String numeroProposta { get; set; }


        [Display(Name = "Número Apolice"), MaxLength(150)]
        public String numeroApolice { get; set; }


        [Display(Name = "Código"), MaxLength(150)]
        public String codigo { get; set; }

        [Display(Name = "Comissão Corretora")]
        public Decimal? comissaoCorretora { get; set; }

        [Display(Name = "Comissão Representante")]
        public Decimal? comissaoRepresentante { get; set; }

        [Display(Name = "Comissão Representante")]
        public int numeroOportunidade { get; set; }

        public virtual ICollection<PropostaApoliceRamoItem> PropostaApoliceRamoItems { get; set; }


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

        public bool Alterar(PropostaApolice obj, ParamBase pb)
        {
            return Alterar(obj, null, pb);
        }

        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, null,pb);
        }
        public bool Alterar(PropostaApolice obj, DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id,pb);
            if (objAux == null)
                return false;
            else
            {
                using (var dbcxtransaction = db.Database.BeginTransaction())
                {
                    new LogMudanca().Incluir(obj, objAux, "", db, pb);
                    objAux.bemGarantia = obj.bemGarantia;
                    objAux.dataEfetivacaoProposta = obj.dataEfetivacaoProposta;
                    objAux.dataProposta = obj.dataProposta;
                    objAux.dataRegistro = obj.dataRegistro;
                    objAux.dataVigenciaFim = obj.dataVigenciaFim;
                    objAux.dataVigenciaInicio = obj.dataVigenciaInicio;
                    objAux.grupoRamoSeguro_id = obj.grupoRamoSeguro_id;
                    objAux.numeroApolice = obj.numeroApolice;
                    objAux.numeroProposta = obj.numeroProposta;

                    objAux.regiao_id = obj.regiao_id;
                    objAux.representante_id = obj.representante_id;
                    objAux.Segurado_Principal_id = obj.Segurado_Principal_id;
                    objAux.seguradora_id = obj.seguradora_id;
                    objAux.sucursal_id = obj.sucursal_id;
                    objAux.tipoCondicaoPagamentoPremio_id = obj.tipoCondicaoPagamentoPremio_id;
                    objAux.tipoPropostaSeguro_id = obj.tipoPropostaSeguro_id;
                    objAux.tipoStatusPropostaSeguro_id = obj.tipoStatusPropostaSeguro_id;
                    objAux.observacoes = obj.observacoes;
                    objAux.comissaoCorretora = obj.comissaoCorretora;
                    objAux.comissaoRepresentante = obj.comissaoRepresentante;
                    objAux.numeroOportunidade = obj.numeroOportunidade;
                    db.SaveChanges();

                    if (obj.PropostaApoliceRamoItems != null)
                    {
                        var idExistentes = (obj.PropostaApoliceRamoItems.Where(p => p.id != 0).Select(p => p.id)).ToArray();
                        var Idexcluidos =
                            new PropostaApoliceRamoItem().ObterTodos(obj.id, db).Select(p => p.id);
                        var items = Idexcluidos.Except(idExistentes);

                        foreach (var item in items)
                        {
                            var erro = "";
                            new PropostaApoliceRamoItem().Excluir(item, ref erro, pb, db);
                            if (erro != "")
                                throw new Exception(erro);
                        }

                        foreach (var item in obj.PropostaApoliceRamoItems)
                        {
                            if (item.id == 0)
                            {
                                item.Incluir(pb, db);
                            }
                            else
                            {
                                item.Alterar(pb, db);
                            }
                            

                        }
                        dbcxtransaction.Commit();

                    }
                    return true;
                }
            }
        }

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(PropostaApolice obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();
            using (var dbcxtransaction = banco.Database.BeginTransaction())
            {
                new LogMudanca().Incluir(obj, "", "", banco, pb);
                banco.Set<PropostaApolice>().Add(obj);
                banco.SaveChanges();
                //foreach (var item in obj.PropostaApoliceRamoItems)
                //{
                //    item.PropostaApolice_id = obj.id;
                //    item.Incluir(banco);
                //}
                dbcxtransaction.Commit();
            }
            return true;
        }

        public PropostaApolice ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }

        public PropostaApolice ObterPorId(int id, DbControle banco, ParamBase pb)
        {
            int idEstab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            return banco.PropostaApolice.Where(x => x.id == id && x.estabelecimento_id == idEstab).FirstOrDefault();
        }

        public List<PropostaApolice> ObterTodos(ParamBase pb, DbControle banco = null)
        {
            int idEstab = pb.estab_id;
            
            if (banco == null)
                banco = new DbControle();

            return banco.PropostaApolice.Where(x => x.estabelecimento_id == idEstab).ToList();
        }

        [NotMapped]
        public string seguradora_descr { get; set; }

        public void isValidoRegrasNegocio(ParamBase pb, DbControle banco = null)
        {

            if (this.PropostaApoliceRamoItems.Count() == 0)
                throw new Exception("Favor informar pelo menos um ramo para salvar");

            int idEstab = pb.estab_id;
            if (banco == null)
                banco = new DbControle();

            if (!string.IsNullOrEmpty(this.numeroProposta))
                if (banco.PropostaApolice.Where(x => x.seguradora_id == this.seguradora_id 
                    && x.id != this.id 
                    && x.numeroProposta == numeroProposta
                    && (x.TipoStatusPropostaSeguro.descricao.ToUpper() != "CANCELADA" || x.TipoStatusPropostaSeguro.descricao.ToUpper() != "RECUSADA" )
                    && x.estabelecimento_id == idEstab).Count() > 0)
                {
                    throw new Exception("Númento de Apolice já cadastrada.");
                }

            if (!string.IsNullOrEmpty(this.numeroApolice))
                if (banco.PropostaApolice.Where(x => x.seguradora_id == this.seguradora_id
                    && x.id != this.id
                    && x.numeroApolice == numeroApolice
                    && (x.TipoStatusPropostaSeguro.descricao.ToUpper() != "CANCELADA" || x.TipoStatusPropostaSeguro.descricao.ToUpper() != "RECUSADA")
                    && x.estabelecimento_id == idEstab).Count() > 0)
                {
                    throw new Exception("Númento de Proposta já cadastrada.");
                }

            if (!string.IsNullOrEmpty(this.codigo))
                if (banco.PropostaApolice.Where(x => x.id != this.id
                    && x.codigo == codigo
                    && x.estabelecimento_id == idEstab).Count() > 0)
                {
                    throw new Exception("Código já incluido, recarregue o formulário.");
                }
        }

        public List<PropostaApolice> ObterTodosVencimento(DateTime dateTime1, DateTime dateTime2, int estab)
        {
            var banco = new DbControle();

            return banco.PropostaApolice.
                Where(x => x.dataVigenciaFim >= dateTime1
                    && x.dataVigenciaFim <= dateTime2
                    && x.estabelecimento_id == estab).ToList();   
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class NotaFiscal: BaseModels
    {
        public NotaFiscal()
        {
            SituacaoRecebimento = 1;
            Recebimentos = new List<Recebimento>();
        }

        [Key]
        public int id { get; set; }

        [Display(Name = "Situação Prefeitura"), Required(ErrorMessage = "Preencha a situação da prefeitura")]
        public int situacaoPrefeitura_id { get; set; }

        //public const int RPS_NF_EMITIDANAOENVIADA = 1;
        //public const int NFGERADAENVIADA = 2;
        //public const int NFCANCELADAEMCONF = 3;
        //public const int NFCANCELADACCONF = 4;
        //public const int NFBAIXA = 5;
        //public const int NFAVULSA = 6;
        //1-Em aberto, 2-RPS Enviado, 3-Numero da Nota Atualizado

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Preencha o estabelecimento")]
        public int estabelecimento_id { get; set; }

        
        [Index("IX_ORDEM_UNICO", IsUnique = true)]
        [Display(Name = "Ordem de Venda")]
        public Nullable<int> ordemVenda_id { get; set; }

        [Display(Name = "Código Serviço"), StringLength(50)]
        public string codigoServico { get; set; }


        [Display(Name = "Banco")]
        public int? banco_id { get; set; }

        [Display(Name = "Operação")]
        public int? operacao_id { get; set; }

        [Display(Name = "Tipo RPS"), Required(ErrorMessage = "Preencha o Tipo de RPS")]
        public int tipoRps { get; set; }

        [Display(Name = "Serie RPS"), Required(ErrorMessage = "Preencha a série"), StringLength(15)]
        public string serieRps { get; set; }

        [Display(Name = "Número RPS"), Required(ErrorMessage = "Preencha o Numero da RPS")]
        public int numeroRps { get; set; }

        [Display(Name = "Data Emissão RPS"), Required(ErrorMessage = "Preencha a data de Emissão do RPS")]
        public DateTime dataEmissaoRps { get; set; }

        [Display(Name = "Situação RPS"), Required(ErrorMessage = "Preencha a Situação do RPS"), StringLength(15)]
        public string situacaoRps { get; set; }


        [Display(Name = "Número Nfse")]
        public int? numeroNfse { get; set; }

        [Display(Name = "Data Emissão NFse"), Required(ErrorMessage = "Preencha a Data Emissão")]
        public DateTime dataEmissaoNfse { get; set; }

        

        [Display(Name = "Data Vencimento NFse"), Required(ErrorMessage = "Preencha a Data de Vencimento")]
        [DataType(DataType.Date)]
        public DateTime dataVencimentoNfse { get; set; }

        public DateTime DataVencimentoOriginal { get; set; }

        [Display(Name = "Codigo Verificação")]
        public string codigoVerificacao { get; set; }
        
        [Display(Name = "Valor NFSe"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorNfse { get; set; }

        [Display(Name = "Valor Deduções"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorDeducoes { get; set; }

        [Display(Name = "Base de Calculo"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal basedeCalculo { get; set; }

        [Display(Name = "Aliquota ISS"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal aliquotaISS { get; set; }

        [Display(Name = "Valor ISS"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal valorISS { get; set; }

        [Display(Name = "Credito Imposto"), Required]
        public Decimal creditoImposto { get; set; }

        [Display(Name = "Descriminação Serviço"), Required, StringLength(1500)]
        public string discriminacaoServico { get; set; }

        [Display(Name = "IRRF"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal irrf { get; set; }

        [Display(Name = "Pis REtido"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal pisRetido { get; set; }

        [Display(Name = "Confin Retido"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal cofinsRetida { get; set; }

        [Display(Name = "CSLL Retido"), Required, DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public Decimal csllRetida { get; set; }

        [Display(Name = "Valor Liquido"), Required, DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public Decimal valorLiquido { get; set; }

        [Display(Name = "Aliquita de IRRF"), Required, DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public Decimal aliquotaIrrf { get; set; }


        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }


        [JsonIgnore,ForeignKey("ordemVenda_id")]
        public virtual OrdemVenda OrdemVenda { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco banco { get; set; }

        [JsonIgnore,ForeignKey("operacao_id")]
        public virtual Operacao Operacao { get; set; }

        public virtual List<Recebimento> Recebimentos { get; set; }

        [Display(Name = "Recebimento"), Required(ErrorMessage = "Preencha a Situacao do Recebimento(1 - Em Aberto, 2 - Recebido Parcialmente, 3 -Recebido Integralmente)")]
        public int SituacaoRecebimento { get; set; }
        /*1 - Em Aberto, 2 - Recebido Parcialmente, 3 -Recebido Integralmente */

        [Display(Name = "Entrada ou Saida (E ou S)"), MaxLength(1)]
        public string entradaSaida { get; set; }


        public int? municipio_id { get; set; }

        [JsonIgnore,ForeignKey("municipio_id")]
        public virtual Municipio municipio { get; set; }


        [Display(Name = "Aliquota INSS")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal? aliquotaINSS { get; set; }

        [Display(Name = "Valor INSS")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public Decimal? valorINSS { get; set; }



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

        [MaxLength(50)]
        public String ObraNumEncapsulamento { get; set; }


        public int? notaFiscalTomador_id { get; set; }

        [JsonIgnore,ForeignKey("notaFiscalTomador_id")]
        public virtual NotaFiscalPessoa NotaFiscalPessoaTomador { get; set; }

        public int? notaFiscalPrestador_id { get; set; }

        [JsonIgnore,ForeignKey("notaFiscalPrestador_id")]
        public virtual NotaFiscalPessoa NotaFiscalPessoaPrestador { get; set; }


        public int? notaFiscalIntermediario_id { get; set; }

        [JsonIgnore, ForeignKey("notaFiscalIntermediario_id")]
        public virtual NotaFiscalPessoa NotaFiscalIntermediario { get; set; }

        public int? NotaFiscalNFE_id { get; set; }

        [JsonIgnore,ForeignKey("NotaFiscalNFE_id")]
        public virtual NotaFiscalNFE NotaFiscalNFE { get; set; }


        

        public bool Excluir(ref string erro, ParamBase pb, DbControle db = null)
        {
            return Excluir(this.id, ref erro, pb, db);
        }

        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle db = null)
        {
            try
            {

                if (db == null)
                    db = new DbControle();
                
                var obj = ObterPorId(id, db);
                if (obj == null)
                {
                    erro = "Registro não encontrado";
                    return false;
                }
                else
                {
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.NotaFiscal.Remove(obj);
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
        public bool Alterar(ParamBase pb, DbControle db = null)
        {
            return Alterar(this, pb, db);
        }
        public bool Alterar(NotaFiscal obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id,db);
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
        private bool validaExistencia(DbControle db, NotaFiscal obj)
        {
            return (false);
        }
        public bool Incluir(ParamBase pb, DbControle db = null)
        {
            return Incluir(this, pb, db );
        }
        public bool Incluir(NotaFiscal obj, ParamBase pb, DbControle db = null)
        {
            if (db==null)
                db = new DbControle();

            if (validaExistencia(db, obj))
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "",null,db,pb);

                db.Set<NotaFiscal>().Add(obj);
                db.SaveChanges();

                return true;
            }
        }
        public NotaFiscal ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public NotaFiscal ObterPorId(int id, DbControle db)
        {

            if (db == null)
                db = new DbControle();
                
            var nfe = db.NotaFiscal.Where(nf => nf.id == id ).Include(p => p.NotaFiscalNFE.NotaFiscalNFEFormaPagamentos).FirstOrDefault();
            //db.Entry(nfe).State = EntityState.Detached;

            return nfe;
        }
        public List<NotaFiscal> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab && x.NotaFiscalNFE == null).OrderBy(p => p.dataEmissaoNfse).ToList();
        }
        public List<NotaFiscal> ObterTodosPorNumNFe(int numnfe, int serie, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab && x.numeroNfe == numnfe && x.serieNfe == serie).ToList();
        }

        public List<NotaFiscal> ObterTodosNFe(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab && x.NotaFiscalNFE != null).OrderBy(p => p.dataEmissaoNfse).ToList();
        }
        public List<NotaFiscal> ObterTodos(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var auxDataInicial = new DateTime(DataInicial.Year, DataInicial.Month, DataInicial.Day, 0, 0, 0);
            var auxDataFinal = new DateTime(DataFinal.Year, DataFinal.Month, DataFinal.Day, 23, 59, 59);
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab && x.dataEmissaoRps >= auxDataInicial && x.dataEmissaoRps <= auxDataFinal).ToList();
        }

        public const int RPS_NF_EMITIDANAOENVIADA = 1;
        public const int NFGERADAENVIADA = 2;
        public const int NFCANCELADAEMCONF = 3;
        public const int NFCANCELADACCONF = 4;
        public const int NFBAIXA = 5;
        public const int NFAVULSA = 6;
        

        public const string RPSEMITIDA_TEXTO = "1 - RPS Emitido";
        public const string NFSEGERADA_TEXTO = "2 - NFS-e Gerada";
        public const string NFSECANCELADAEMCONF_TEXTO = "3 - NFS-e cancelada sem confirmação";
        public const string NFSECANCELADACCONF_TEXTO = "4 - NFS-e cancelada com confirmação";
        public const string NFSEBAIXADA_TEXTO = "5 - NFS-e baixada como perda";
        public const string NFAVULSA_TEXTO = "6 - Nota Avulsa";
        public const string OUTRODOC_TEXTO = "9 - OUTRO DOCUMENTO";

        public const string NFEEMITIDANAOENVIADA_TEXTO = "1 - Nota Gerada (Não Enviada)";
        public const string NFEGERADA_TEXTO = "2 - NF-e Gerada";
        public const string NFECANCELADAEMCONF_TEXTO = "3 - NF-e cancelada sem confirmação";
        public const string NFECANCELADACCONF_TEXTO = "4 - NF-e cancelada com confirmação";
        public const string NFEBAIXADA_TEXTO = "5 - NF-e baixada como perda";

        public const int NFEMABERTO = 1;
        public const int NFRECEBIDAPARC = 2;
        public const int NFRECEBIDATOTAL = 3;
        public const int NFBAIXADA = 4;

        public static string CarregaSituacao(int situacao_id)
        {
            var situacao = "";
            switch (situacao_id)
            {
                case Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA:
                    situacao = Models.NotaFiscal.RPSEMITIDA_TEXTO;
                    break;
                case Models.NotaFiscal.NFGERADAENVIADA:
                    situacao = Models.NotaFiscal.NFSEGERADA_TEXTO;
                    break;
                case Models.NotaFiscal.NFCANCELADAEMCONF:
                    situacao = Models.NotaFiscal.NFSECANCELADAEMCONF_TEXTO;
                    break;
                case Models.NotaFiscal.NFCANCELADACCONF:
                    situacao = Models.NotaFiscal.NFSECANCELADACCONF_TEXTO;
                    break;
                case Models.NotaFiscal.NFBAIXA:
                    situacao = Models.NotaFiscal.NFSEBAIXADA_TEXTO;
                    break;
                default:
                    situacao = "Não Informada";
                    break;
            }
            return situacao;
        }
        public static SelectList ListaDropDrow()
        {
            var items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Value = RPS_NF_EMITIDANAOENVIADA.ToString(), Text = RPSEMITIDA_TEXTO, Selected = false });
            items.Add(new SelectListItem() { Value = NFGERADAENVIADA.ToString(), Text = NFSEGERADA_TEXTO, Selected = false });
            items.Add(new SelectListItem() { Value = NFCANCELADAEMCONF.ToString(), Text = NFSECANCELADAEMCONF_TEXTO, Selected = false });
            items.Add(new SelectListItem() { Value = NFCANCELADACCONF.ToString(), Text = NFSECANCELADACCONF_TEXTO, Selected = false });
            items.Add(new SelectListItem() { Value = NFBAIXA.ToString(), Text = NFSEBAIXADA_TEXTO, Selected = false });


            var listret = new SelectList(items, "Value", "Text");

            return listret;
        }
        public List<NotaFiscal> ObterTodosEmAberto(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            //Lista quando em aberto ou recebido parcial
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab 
                                        && (x.situacaoPrefeitura_id == NotaFiscal.NFGERADAENVIADA
                                        || x.situacaoPrefeitura_id == NotaFiscal.NFAVULSA
                                        )
                                        && (x.SituacaoRecebimento  == NotaFiscal.NFEMABERTO 
                                        ||  x.SituacaoRecebimento  == NotaFiscal.NFRECEBIDAPARC)).ToList();

        }
        public List<NotaFiscal> ObterTodosRecebidos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab
                                        && (x.situacaoPrefeitura_id == NotaFiscal.NFGERADAENVIADA
                                        || x.situacaoPrefeitura_id == NotaFiscal.NFAVULSA
                                        )
                                        && x.SituacaoRecebimento != NotaFiscal.NFEMABERTO).ToList();
        }
        public List<NotaFiscal> ObterEntreData(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.dataEmissaoNfse) >= DataInicial
                                        && DbFunctions.TruncateTime(x.dataEmissaoNfse) <= DataFinal).Include(p => p.Recebimentos).Include( p => p.OrdemVenda).ToList();
        }

        public List<NotaFiscal> ObterEntreDataOD(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab
                                        && x.TipoFaturamento == 2 /*2 - Outros*/
                                        && DbFunctions.TruncateTime(x.dataEmissaoNfse) >= DataInicial
                                        && DbFunctions.TruncateTime(x.dataEmissaoNfse) <= DataFinal).Include(p => p.Recebimentos).Include(p => p.OrdemVenda).ToList();
        }

        public NotaFiscal ObterODPorId(int id, ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;

            if (db == null)
                db = new DbControle();

            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab
                                        &&  x.TipoFaturamento == 2 /*2 - Outros*/
                                        && x.id == id /*2 - Outros*/
                                        ).FirstOrDefault();
        }

        public List<NotaFiscal> ObterEntreDataSomenteNFe(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.dataEmissaoNfse) >= DataInicial
                                        && DbFunctions.TruncateTime(x.dataEmissaoNfse) <= DataFinal
                                        && x.NotaFiscalNFE != null).Include(p => p.Recebimentos).Include(p => p.OrdemVenda).ToList();
        }
        public List<NotaFiscal> ObterEntreDataSomenteNFSe(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.dataEmissaoRps) >= DataInicial
                                        && DbFunctions.TruncateTime(x.dataEmissaoRps) <= DataFinal
                                        && x.NotaFiscalNFE == null).Include(p => p.Recebimentos).Include(p => p.OrdemVenda).ToList();
        }
        public List<NotaFiscal> ObterEntreDataVencto(System.DateTime DataInicial, System.DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab
                                        && DbFunctions.TruncateTime(x.dataEmissaoNfse) >= DataInicial
                                        && DbFunctions.TruncateTime(x.dataEmissaoNfse ) <= DataFinal).Include(p => p.Recebimentos).Include(p => p.OrdemVenda).ToList();
        }
        public NotaFiscal ObterPorCodigoVerificacao(int municipio_id, string codigoVerificacao,ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            if (municipio_id == 0)
                return db.NotaFiscal.Where(x => x.codigoVerificacao == codigoVerificacao  && x.estabelecimento_id == estab).FirstOrDefault();
            else
                return db.NotaFiscal.Where(x => x.codigoVerificacao == codigoVerificacao && x.municipio_id == municipio_id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<NotaFiscal> ObterTodosEmitidosSemConfirmacaoWS(DbControle db, int estab)
        {
            //Lista quando em aberto ou recebido parcial
            return db.NotaFiscal.Where(x => x.estabelecimento_id == estab
                                        && x.numeroNfse == null
                                        && (x.situacaoPrefeitura_id == NotaFiscal.NFGERADAENVIADA
                                        || x.situacaoPrefeitura_id == NotaFiscal.NFAVULSA
                                        )
                                        && (x.SituacaoRecebimento == NotaFiscal.NFEMABERTO
                                        || x.SituacaoRecebimento == NotaFiscal.NFRECEBIDAPARC)).ToList();
        }
        public NotaFiscal ObterPorOV(int p)
        {
            DbControle db = new DbControle();
            return db.NotaFiscal.Where(y => y.ordemVenda_id == p).FirstOrDefault();
        }
        public int serieNfe { get; set; }
        public int numeroNfe { get; set; }
        public int loteNfe { get; set; }
        [MaxLength(1)]
        public string tipoNfe { get; set; }

        public decimal? percentualCargaTributaria { get; set; }
        public decimal? valorCargaTributaria { get; set; }
        
        [MaxLength(10)]
        public string fonteCargaTributaria { get; set; }
        [MaxLength(20)]
        public string codigoCEI { get; set; }


        [MaxLength(30)]
        public string matriculaObra { get; set; }

        public int TipoFaturamento { get; set; }
        public decimal ValAliqPIS { get;  set; }
        public decimal ValISSRetido { get;  set; }
        public string RespRetencao { get;  set; }
        public decimal ValAliqCSLL { get;  set; }
        public decimal ValAliqISSRetido { get;  set; }
        public decimal ValAliqCOFINS { get;  set; }

        [MaxLength(1)]
        public string LocalPrestServ { get; set; }

        public int ObterTodosUltimaNFe()
        {
            DbControle db = new DbControle();
            return db.NotaFiscal.Max(y => y.numeroNfe) + 1;
        }

        public IQueryable<NotaFiscal> ObterEntreDataSemBoleto(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();

            var lista = (from nf in db.NotaFiscal
                         join bl in db.Boleto
                         on nf.id equals bl.id into left
                         from bl in left.DefaultIfEmpty()
                         where (nf.estabelecimento_id == estab
                          && DbFunctions.TruncateTime(nf.dataVencimentoNfse) >= DataInicial
                          && DbFunctions.TruncateTime(nf.dataVencimentoNfse) <= DataFinal)
                         select nf);
            return lista;
        }
    }
}

using Newtonsoft.Json;
using SoftFin.Web.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;


namespace SoftFin.Web.Models
{

    public class BancoMovimento : BaseModels
    {
        [Key]
        public int id { get; set; }
        


        [Display(Name = "Banco"),
        Required(ErrorMessage = "Informe o banco")]
        public int banco_id { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco Banco { get; set; }


        [Display(Name = "Plano de Contas"),
        Required(ErrorMessage = "Informe o Plano de Contas")]
        public int planoDeConta_id { get; set; }

        [JsonIgnore,ForeignKey("planoDeConta_id")]
        public virtual PlanoDeConta PlanoDeConta { get; set; }
        
        
        [Display(Name = "Origem Movimento"),
        Required(ErrorMessage = "Informe a Origem de Movimento")]
        public int origemmovimento_id { get; set; }
        [JsonIgnore,ForeignKey("origemmovimento_id")]
        public virtual OrigemMovimento OrigemMovimento { get; set; }


        [Display(Name = "Tipo de Movimento"),
        Required(ErrorMessage = "Infome o Tipo de Movimento")]
        public int tipoDeMovimento_id { get; set; }
        [JsonIgnore,ForeignKey("tipoDeMovimento_id")]
        public virtual TipoMovimento TipoMovimento { get; set; }

        [Display(Name = "Tipo de Documento"),
        Required(ErrorMessage = "Informe o Tipo de Documento")]
        public int tipoDeDocumento_id { get; set; }
        [JsonIgnore,ForeignKey("tipoDeDocumento_id")]
        public virtual TipoDocumento TipoDocumento { get; set; }





        [Display(Name = "Data"),
        Required(ErrorMessage = "Informe a Data")]
        public DateTime data { get; set; }
        [Display(Name = "Historico"),
        StringLength(500), Required(ErrorMessage = "Informe o historico")]
        public string historico { get; set; }
        [Display(Name = "Valor"),
        Required(ErrorMessage = "Informe Valor")]
        public decimal valor { get; set; }

        [Display(Name = "Nota Fiscal")]
        public int? notafiscal_id { get; set; }
        [JsonIgnore,ForeignKey("notafiscal_id")]
        public virtual NotaFiscal NotaFiscal { get; set; }
        


        [Display(Name = "Extrato Lançamento")]
        public int? LanctoExtrato_id { get; set; }
        [JsonIgnore,ForeignKey("LanctoExtrato_id")]
        public virtual LanctoExtrato LanctoExtrato { get; set; }

        [Display(Name = "Recebimento")]
        public int? recebimento_id { get; set; }
        [JsonIgnore,ForeignKey("recebimento_id")]
        public virtual Recebimento Recebimento { get; set; }

        [Display(Name = "Pagamento")]
        public int? pagamento_id { get; set; }
        [JsonIgnore,ForeignKey("pagamento_id")]
        public virtual Pagamento Pagamento { get; set; }



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


        [Display(Name = "Documento Contas a Pagar")]
        public int? DocumentoPagarParcela_id { get; set; }
        [JsonIgnore,ForeignKey("DocumentoPagarParcela_id")]
        public virtual DocumentoPagarParcela DocumentoPagarParcela { get; set; }


        [Display(Name = "Loja Fechamento CC")]
        public int? LojaFechamentoCC_id { get; set; }
        [JsonIgnore,ForeignKey("LojaFechamentoCC_id")]
        public virtual LojaFechamentoCC LojaFechamentoCC { get; set; }

        [Display(Name = "Unidade de Negocio")]
        public int? UnidadeNegocio_id { get; set; }
        [JsonIgnore, ForeignKey("UnidadeNegocio_id")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }


        public bool Excluir(int id, ref string erro, ParamBase pb,DbControle db = null)
        {
            try
            {
                int estab = pb.estab_id;
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
                    var objSemRef = TiraReferencia(obj);


                    new LogMudanca().Incluir(objSemRef, "", "", db,pb);
                    db.BancoMovimento.Remove(obj);
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


        private static BancoMovimento TiraReferencia(BancoMovimento obj)
        {
            var objSemRef = new BancoMovimento();

            objSemRef.banco_id = obj.banco_id;
            objSemRef.data = obj.data;
            objSemRef.DocumentoPagarParcela_id = obj.DocumentoPagarParcela_id;
            //objSemRef.empresa_id = obj.empresa_id;
            objSemRef.LanctoExtrato_id = obj.LanctoExtrato_id;
            objSemRef.notafiscal_id = obj.LanctoExtrato_id;
            objSemRef.origemmovimento_id = obj.origemmovimento_id;
            objSemRef.planoDeConta_id = obj.planoDeConta_id;
            objSemRef.tipoDeDocumento_id = obj.tipoDeDocumento_id;
            objSemRef.tipoDeMovimento_id = obj.tipoDeMovimento_id;
            //objSemRef.unidadeDeNegocio_id = obj.unidadeDeNegocio_id;
            objSemRef.valor = obj.valor;
            return objSemRef;
        }

        public bool Alterar(ParamBase pb, DbControle db)
        {
            return Alterar(this, pb, db);
        }


        public bool Alterar(ParamBase pb)
        {
            return Alterar(this, pb, null);
        }

        public bool Alterar(BancoMovimento obj, ParamBase pb)
        {
            return Alterar(obj, pb, null);
        }
        
        public bool Alterar(BancoMovimento obj, ParamBase pb, DbControle db)
        {
            if (db  == null)
                db = new DbControle();

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

        public bool Incluir(ParamBase pb, DbControle banco = null)
        {
            return Incluir(this, pb, banco);
        }
        public bool Incluir(BancoMovimento obj, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

  
            new LogMudanca().Incluir(obj, "", "", banco, pb);
            banco.Set<BancoMovimento>().Add(obj);
            banco.SaveChanges();
            return true;
        }

        public BancoMovimento ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }

        public BancoMovimento ObterPorId(int id, DbControle banco)
        {
             if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.id == id).FirstOrDefault();
        }



        public List<BancoMovimento> ObterTodos(ParamBase paramBase)
        {
            DbControle banco = new DbControle();
            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id).ToList();
        }

        public IQueryable<BancoMovimento> IQObterTodosQuery(ParamBase paramBase)
        {
            DbControle banco = new DbControle();
            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id);
        }
        public List<BancoMovimento> ObterTodos(DbControle banco, ParamBase paramBase)
        {

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id).ToList();
        }
        public List<BancoMovimento> ObterTodos(int Banco, DateTime data, ParamBase paramBase)
        {
            DbControle banco = new DbControle();
            var dataInicial = DateTime.Parse(data.ToShortDateString());
            var dataFinal = dataInicial.AddDays(1).AddSeconds(-1);
            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.banco_id == Banco 
			&& DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(dataInicial) 
			&& DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(dataFinal)).Include(p => p.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa).ToList();
        }

        public List<BancoMovimento> ObterPorRecebimento(int id, DbControle banco, ParamBase paramBase)
        {

            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.recebimento_id == id).ToList();
        }

        public List<BancoMovimento> ObterPorPagamento(int id, DbControle banco, ParamBase paramBase)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.pagamento_id == id).ToList();
        }


        public List<BancoMovimento> ObterPorNFES(int id, DbControle banco, ParamBase paramBase)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.notafiscal_id == id).ToList();
        }

        public BancoMovimento ObterPorNFPlano(int id, int planoid, DbControle banco, ParamBase paramBase)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id 
                                                    && x.notafiscal_id == id
                                                    && x.planoDeConta_id == planoid
                                                    ).FirstOrDefault();
        }

        public List<BancoMovimento> ObterPorNFES(int id, ParamBase paramBase)
        {
            return ObterPorNFES(id, null, paramBase);
        }


        public List<BancoMovimento> ObterTodosData(DateTime dataInicial, DateTime dataFinal, ParamBase paramBase)
        {
            
            DbControle banco = new DbControle();
            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id &&
                                                       DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(dataInicial) &&
                                                       DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(dataFinal)).OrderBy(x=>x.data).ToList();
        }

        public List<BancoMovimento> ObterTodosAConciliar(DateTime dataInicial, DateTime dataFinal, ParamBase paramBase)
        {

            DbControle banco = new DbControle();
            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id &&
                                                       DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(dataInicial) &&
                                                       DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(dataFinal) &&
                                                       (x.PlanoDeConta.codigo == "12" || x.PlanoDeConta.codigo == "12.01" || x.PlanoDeConta.codigo == "12.02")
                                                       ).OrderBy(x => x.data).ToList();
        }

        public List<BancoMovimento> ObterTodosDataConsolidado(DateTime dataInicial, DateTime dataFinal, string consolidado, ParamBase pb)
        {
             

            Estabelecimento estab = new Estabelecimento();


            DbControle banco = new DbControle();


            if (!consolidado.ToUpper().Equals("TRUE"))
            {
                return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == pb.estab_id &&
                                                           DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(dataInicial) &&
                                                           DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(dataFinal)).Include(p => p.DocumentoPagarParcela.DocumentoPagarMestre.DocumentoPagarDetalhes).OrderBy(x => x.data).ToList();
            }
            else
            {
                return banco.BancoMovimento.Where(x => x.Banco.Estabelecimento.Empresa_id == pb.empresa_id &&
                                                           DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(dataInicial) &&
                                                           DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(dataFinal)).Include(p => p.DocumentoPagarParcela.DocumentoPagarMestre.DocumentoPagarDetalhes).OrderBy(x => x.data).ToList();
            }


        }

        public List<BancoMovimento> ObterTodosHoldingDataConsolidado(DateTime dataInicial, DateTime dataFinal, ParamBase pb, List<int> estabs = null)
        {


            Estabelecimento estab = new Estabelecimento();


            DbControle banco = new DbControle();

            if (estabs == null)
            {
                return banco.BancoMovimento.Where(x => x.Banco.Estabelecimento.Empresa.Holding_id == pb.holding_id &&
                                                            DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(dataInicial) &&
                                                            DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(dataFinal)).
                                                            Include(p => p.DocumentoPagarParcela.DocumentoPagarMestre.DocumentoPagarDetalhes).
                                                            Include(p => p.Banco.Estabelecimento.Empresa.Holding).
                                                            OrderBy(x => x.data).ToList();
            }
            else
            {
                return banco.BancoMovimento.Where(x => estabs.Contains(x.Banco.estabelecimento_id) &&
                                            DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(dataInicial) &&
                                            DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(dataFinal)).
                                            Include(p => p.DocumentoPagarParcela.DocumentoPagarMestre.DocumentoPagarDetalhes).
                                            Include(p => p.Banco.Estabelecimento.Empresa.Holding).
                                            OrderBy(x => x.data).ToList();
            }
            


        }


        public List<BancoMovimento> ObterTodosJoinCPAGDataConsolidado(DateTime dataInicial, DateTime dataFinal, string consolidado, ParamBase pb)
        {


            Estabelecimento estab = new Estabelecimento();
            List<Estabelecimento> list = estab.ObterTodos(pb).Where(x => x.Empresa_id == pb.estab_id).ToList();
            List<int> ids = list.Select(x => x.id).ToList();

            DbControle banco = new DbControle();


            if (consolidado.ToUpper().Equals("TRUE"))
            {
                return banco.BancoMovimento.Where(x => ids.Any(id => id == x.Banco.estabelecimento_id) &&
                                                           DbFunctions.TruncateTime(x.DocumentoPagarParcela.DocumentoPagarMestre.dataDocumento) >= DbFunctions.TruncateTime(dataInicial) &&
                                                           DbFunctions.TruncateTime(x.DocumentoPagarParcela.DocumentoPagarMestre.dataDocumento) <= DbFunctions.TruncateTime(dataFinal)).Include(p => p.DocumentoPagarParcela.DocumentoPagarMestre.DocumentoPagarDetalhes).OrderBy(x => x.data).ToList();
            }
            else
            {
                return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == pb.estab_id &&
                                                           DbFunctions.TruncateTime(x.DocumentoPagarParcela.DocumentoPagarMestre.dataDocumento) >= DbFunctions.TruncateTime(dataInicial) &&
                                                           DbFunctions.TruncateTime(x.DocumentoPagarParcela.DocumentoPagarMestre.dataDocumento) <= DbFunctions.TruncateTime(dataFinal)).Include(p => p.DocumentoPagarParcela.DocumentoPagarMestre.DocumentoPagarDetalhes).OrderBy(x => x.data).ToList();
            }


        }



        public List<BancoMovimento> ObterTodosDataBanco(DateTime dataInicial, DateTime dataFinal, int idbanco, ParamBase paramBase)
        {

            DbControle banco = new DbControle();
            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.banco_id == idbanco &&
                                                       DbFunctions.TruncateTime(x.data) >= dataInicial &&
                                                       DbFunctions.TruncateTime(x.data) <= dataFinal).OrderBy(x=>x.data).ToList();
        }

        public List<BancoMovimento> ObterTodosDataBanco(DateTime dataInicial, DateTime dataFinal, int idbanco, decimal valor, ParamBase paramBase)
        {
  
            DbControle banco = new DbControle();
            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.banco_id == idbanco &&
                                                       DbFunctions.TruncateTime(x.data) >= DbFunctions.TruncateTime(dataInicial) &&
                                                       DbFunctions.TruncateTime(x.data) <= DbFunctions.TruncateTime(dataFinal) 
                                                       && x.valor == valor).OrderBy(x => x.data).ToList();
        }





        public List<BancoMovimento> ObterPorCPAG(int p, ParamBase paramBase, DbControle banco = null)
        {

            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.DocumentoPagarParcela_id == p).ToList();
        }
        public List<BancoMovimento> ObterPorCPAGSemPagamento(int p, ParamBase paramBase, DbControle banco = null)
        {

            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.DocumentoPagarParcela_id == p && x.pagamento_id == null).ToList();
        }

        public List<BancoMovimento> ObterPorFechamentoCC(int p, ParamBase paramBase,DbControle banco = null)
        {

            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.LojaFechamentoCC_id == p).ToList();
        }

        public List<BancoMovimento> ObterPorCPAGParcela(int p, ParamBase paramBase,DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            return banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id 
                                                && x.DocumentoPagarParcela_id == p
                                                && x.pagamento_id == null).ToList();
        }

        public List<BancoMovimentoConciliacaoVLW3> ObterAgrupado(int Banco, DateTime data, ParamBase paramBase)
        {
            var objs = new List<BancoMovimentoConciliacaoVLW3>();

            var objsTemp = new BancoMovimento().ObterTodos(Banco,data,paramBase).OrderBy(p => p.valor).ThenBy(p => p.data);

            foreach (var item in objsTemp)
            {
                var itemAux = new BancoMovimentoConciliacaoVLW3();

                itemAux.Data = item.data.ToShortDateString();
                var itemConciliado = (new BancoMovimentoLanctoExtrato().ObterBancoMovimento_id(item.id,null).Count() > 0) ;

                if (!itemConciliado)
                {
                    if (item.DocumentoPagarParcela != null)
                        itemAux.ContaBancaria = item.DocumentoPagarParcela.DocumentoPagarMestre.Pessoa.contaBancaria;

                    itemAux.Descricao = item.historico;
                    itemAux.id = item.id.ToString();
                    if (item.TipoMovimento.Codigo == "SAI")
                        itemAux.Valor = (item.valor *-1).ToString("n");
                    else
                        itemAux.Valor = item.valor.ToString("n");
                        
                    if (!string.IsNullOrEmpty(itemAux.Valor))
                    {
                        var selecionado = (new BancoMovimentoLanctoExtratoUsuario().ObterUsuarioCodigo(itemAux.id) != null);

                        if (selecionado)
                            itemAux.Selecionado = "S";
                        else
                            itemAux.Selecionado = "N";

                        objs.Add(itemAux);
                    }
                }
            
            }

            return objs;

        }

        [NotMapped]
        public int estab { get; set; }

        public DateTime ObterDataSuperior(DateTime data, decimal valor, ParamBase paramBase)
        {
            var banco = new DbControle();

            var dataaux = banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.valor == valor && x.data >= data).
                                            OrderBy(p => p.data).Take(1).ToList();

            if (dataaux.Count()== 0)
            {
                return data;
            }
            else
            {
                return dataaux.FirstOrDefault().data;
            }
        }

        public DateTime ObterDataInferior(DateTime data, decimal valor, ParamBase paramBase)
        {
            var banco = new DbControle();

            var dataaux = banco.BancoMovimento.Where(x => x.Banco.estabelecimento_id == paramBase.estab_id && x.valor == valor && x.data <= data).
                                            OrderByDescending(p => p.data).Take(1).ToList();

            if (dataaux.Count() == 0)
            {
                return data;
            }
            else
            {
                return dataaux.FirstOrDefault().data;
            }
        }


        public List<BancoMovimento> ObterTodosDataValor(DateTime dateTime, decimal valor, ParamBase paramBase)
        {
            var banco = new DbControle();

            var dataInic = SoftFin.Utils.UtilSoftFin.TiraHora(dateTime);
            var dataFim = dataInic.AddDays(1);
            var dataaux = banco.BancoMovimento.
                                    Where(x => x.Banco.estabelecimento_id == paramBase.estab_id 
                                        && x.valor == valor
                                        && x.data >= DbFunctions.TruncateTime(dataInic) 
										&& x.data < DbFunctions.TruncateTime(dataFim)).ToList();

            return dataaux;
        }

        public BancoMovimento ObterND(int numero, int? idBM, DbControle db, ParamBase pb)
        {
            if  (db == null)
                db = new DbControle();

            //Gambirra provisoria por falha na modelagem
            if (idBM == null)
            {
                var historico =  "Nota de Débito: " + numero.ToString();
                var dataaux = db.BancoMovimento.
                                        Where(x => x.Banco.estabelecimento_id == pb.estab_id
                                            && x.historico == historico).FirstOrDefault();
                return dataaux;
            }
            else
            {
                return ObterPorId(idBM.Value,db);
            }

 
        }
    }

    public class BancoMovimentoConciliacaoVLW3
    {
        [Key]
        public string id { get; set; }

        [Display(Name = "Data Lançamento")]
        public string Data { get; set; }

        [Display(Name = "Valor Bruto")]
        public String Valor { get; set; }

        [Display(Name = "Valor Bruto")]
        public String Descricao { get; set; }

        [Display(Name = "Nota Fiscal")]
        public int? notafiscal_id { get; set; }

        [Display(Name = "Documento Contas a Pagar")]
        public int? DocumentoPagarMestre_id { get; set; }

        [Display(Name = "Extrato Lançamento")]
        public int? LanctoExtrato_id { get; set; }

        [Display(Name = "Selecionado"), StringLength(1)]
        public String Selecionado { get; set; }
        public string ContaBancaria { get; set; }
    }

}

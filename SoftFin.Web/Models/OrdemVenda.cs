using Newtonsoft.Json;
using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace SoftFin.Web.Models
{

    public class OrdemVenda: BaseModels
    {
        public OrdemVenda()
        {
            NotasFiscais = new List<NotaFiscal>();
            Boletos = new List<Boleto>();
        }


        public int id { get; set; }

        [Display(Name = "Descrição"), Required(ErrorMessage = "Informe a descrição da Ordem de venda"), MaxLength(350)]
        public string descricao { get; set; }


        [Display(Name = "Valor"), Required(ErrorMessage = "Informe o valor da Ordem de venda")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valor { get; set; }

        [DisplayName("Situação Parcela"), Required(ErrorMessage = "Informe a situação parcela da Ordem de venda")]
        public int statusParcela_ID { get; set; }

        [Display(Name = "Unidade de negócio")]
        public int unidadeNegocio_ID { get; set; }

        [Display(Name = "Cliente"),
        Required(ErrorMessage = "Informe o cliente da Ordem de venda")]
        public int pessoas_ID { get; set; }
        
        [DisplayName("Data Emissão"), Required(ErrorMessage = "Informe a data emissão da Ordem de venda")]
        [DataType(DataType.Date)]
        public DateTime data { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "Informe a estabelecimento da Ordem de venda")]
        public int estabelecimento_id { get; set; }

        [DisplayName("Número da Ordem")]
        public int Numero { get; set; }

        [Display(Name = "Item Produto/Serviço")]
        public int? itemProdutoServico_ID { get; set; }

        [Display(Name = "Tabela de Preço")]
        public int? tabelaPreco_ID { get; set; }

        [DisplayName("Parcela Contrato")]
        public int? parcelaContrato_ID { get; set; }

        [JsonIgnore,ForeignKey("parcelaContrato_ID")]
        public virtual ParcelaContrato ParcelaContrato { get; set; }

        [JsonIgnore,ForeignKey("statusParcela_ID")]
        public virtual StatusParcela statusParcela { get; set; }

        [JsonIgnore,ForeignKey("unidadeNegocio_ID")]
        public virtual UnidadeNegocio UnidadeNegocio { get; set; }

        [JsonIgnore,ForeignKey("pessoas_ID")]
        public virtual Pessoa Pessoa { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Usuário Autorizador:")]
        public int? usuarioAutorizador_id { get; set; }
        [JsonIgnore,ForeignKey("usuarioAutorizador_id")]
        public virtual Usuario usuarioAutorizador { get; set; }
        [DisplayName("Data Autorização")]
        public DateTime? dataAutorizacao { get; set; }


        [JsonIgnore,ForeignKey("itemProdutoServico_ID")]
        public virtual ItemProdutoServico ItemProdutoServico { get; set; }

        [JsonIgnore,ForeignKey("tabelaPreco_ID")]
        public virtual TabelaPrecoItemProdutoServico TabelaPrecoItemProdutoServico { get; set; }

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
        
        //0 Servico 1 Mercadoria 2 Outros
        public int TipoFaturamento { get; set; }

        [NotMapped]
        public string pessoanome { get; set; }

        [JsonIgnore]
        public List<NotaFiscal> NotasFiscais { get; set; }

        [JsonIgnore]
        public List<Boleto> Boletos { get; set; }

        public bool Excluir(int id, ref string erro, ParamBase pb, DbControle db = null)
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
                    new LogMudanca().Incluir(obj, "", "",db, pb);
                    db.OrdemVenda.Remove(obj);
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

        public bool Alterar(OrdemVenda obj, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, pb);
                obj.Numero = objAux.Numero;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }


        private bool validaExistencia(DbControle db, OrdemVenda obj)
        {
            return (false);
        }

        public bool Incluir(OrdemVenda obj, ref int numeroOV, ParamBase pb, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();
            
            if (validaExistencia(db, obj))
                return false;
            else
            {
                var estab = pb.estab_id;
                int numero = 1;

                if (db.OrdemVenda.Where(p => p.estabelecimento_id == estab).FirstOrDefault() != null)
                    numero = db.OrdemVenda.Where(p => p.estabelecimento_id == estab).Max(p => p.Numero) + 1;

                obj.Numero = numero;
                new LogMudanca().Incluir(obj, "", "", db, pb);

                db.Set<OrdemVenda>().Add(obj);
                db.SaveChanges();
                numeroOV = obj.Numero;
                return true;
            }
        }


        public OrdemVenda ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public OrdemVenda ObterPorId(int id, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.OrdemVenda.Where(x => x.id == id ).FirstOrDefault();
        }
        public List<OrdemVenda> ObterTodos(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrdemVenda.Where(x => x.estabelecimento_id == estab).ToList();
        }

        public List<OrdemVenda> ObterTodosHolding(ParamBase pb, List<int> estabs = null)
        {
            DbControle db = new DbControle();
            if (estabs == null)
            {
                return db.OrdemVenda.Where(x => x.Estabelecimento.Empresa.Holding_id == pb.holding_id).ToList();
            }
            else
            {
                return db.OrdemVenda.Where(x => estabs.Contains(x.estabelecimento_id)).ToList();
            }
        }
        public List<OrdemVenda> ObterEntreData(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrdemVenda.Where(x => x.estabelecimento_id == estab 
            && DbFunctions.TruncateTime(x.data) >= DataInicial 
            && DbFunctions.TruncateTime(x.data) <= DataFinal).ToList();
        }

        public List<OrdemVenda> ObterTodosAvulso(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.OrdemVenda.Where(x => x.estabelecimento_id == estab && x.parcelaContrato_ID == null).ToList();
        }

        public List<OrdemVenda> ObterTodosNaoAutorizado(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {
            
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var situacao = StatusParcela.SituacaoPendente();

            return db.OrdemVenda.Where(x => x.estabelecimento_id == estab && DbFunctions.TruncateTime(x.data) >= DataInicial && DbFunctions.TruncateTime(x.data) <= DataFinal && x.usuarioAutorizador_id == null && x.statusParcela_ID == situacao).ToList();
        }

        public List<OrdemVenda> ObterTodosAutorizado(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {

            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var situacao = StatusParcela.SituacaoLiberada();
            return db.OrdemVenda.Where(x => x.estabelecimento_id == estab && DbFunctions.TruncateTime(x.data) >= DataInicial && DbFunctions.TruncateTime(x.data) <= DataFinal && x.usuarioAutorizador_id != null && x.statusParcela_ID == situacao).ToList();
        }

        public List<OrdemVenda> ObterTodosPendentesAutorizadas(ParamBase pb)
        {

            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var situacaoliberada = StatusParcela.SituacaoLiberada();
            return db.OrdemVenda.Where(x => x.estabelecimento_id == estab && x.usuarioAutorizador_id != null && x.statusParcela_ID == situacaoliberada).ToList();
        }

        public List<OrdemVenda> ObterTodosPendentes(ParamBase pb)
        {

            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var situacaoliberada = StatusParcela.SituacaoLiberada();
            return db.OrdemVenda.Where(x => x.estabelecimento_id == estab && x.statusParcela_ID == situacaoliberada).ToList();
        }

        public List<OrdemVenda> ObterTodosEmitidos(ParamBase pb)
        {

            int estab = pb.estab_id;
            DbControle db = new DbControle();
            var situacaoEmitida = StatusParcela.SituacaoEmitida();
            return db.OrdemVenda.Where(x => x.estabelecimento_id == estab && x.statusParcela_ID == situacaoEmitida).ToList();
        }


        public OrdemVenda ObterPorParcelaId(int idParcela, int estab, DbControle db)
        {
            if (db == null)
                db = new DbControle();

            return db.OrdemVenda.Where(x => x.parcelaContrato_ID == idParcela && x.estabelecimento_id == estab).FirstOrDefault();
        }



        public List<OrdemVenda> ObterEntreDataSemBoleto(DateTime DataInicial, DateTime DataFinal, ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();

            var retorno =  db.OrdemVenda.Where(x => x.estabelecimento_id == estab 
                                && DbFunctions.TruncateTime(x.data) >= DataInicial 
                                && DbFunctions.TruncateTime(x.data) <= DataFinal 
                                //&& x.Boletos.Count() == 0
                                //&& x.NotasFiscais.Where(p => p.situacaoPrefeitura_id == 2).Count() > 0
                                )
                                .ToList();


            foreach (var item in retorno)
            {
                var nf = new NotaFiscal().ObterPorOV(item.id);
                if (nf != null)
                    if (nf.situacaoPrefeitura_id == 2)
                        item.NotasFiscais.Add(nf);

                var bols = new Boleto().ObterPorOV(pb, item.id).ToList();
                if (bols.Count() > 0)
                    item.Boletos.AddRange(bols);
            }

            retorno = retorno.Where(x => x.Boletos.Count() == 0).ToList();
            retorno = retorno.Where(p => p.NotasFiscais.Count() > 0).ToList();


            return retorno;

        }


    }
}
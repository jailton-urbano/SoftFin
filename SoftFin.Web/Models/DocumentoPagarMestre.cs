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
using System.Web.Script.Serialization;

namespace SoftFin.Web.Models
{
    public class DocumentoPagarMestre : BaseModels
    {
        public const int DOCEMABERTO = 1;
        public const int DOCPAGOPARC = 2;
        public const int DOCPAGOTOTAL = 3;

        public DocumentoPagarMestre()
        {
            DocumentoPagarParcelas = new List<DocumentoPagarParcela>();
            DocumentoPagarDetalhes = new List<DocumentoPagarDetalhe>();
            DocumentoPagarProjetos = new List<DocumentoPagarProjeto>();
        }

        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        
        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Pessoa"), Required(ErrorMessage = "*")]
        public int pessoa_id { get; set; }

        [Display(Name = "Data Lançamento"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        public DateTime dataLancamento { get; set; }

        [Display(Name = "Data Competencia"), Required(ErrorMessage = "*"), StringLength(7)]
        public string dataCompetencia { get; set; }

        //[Display(Name = "Data Vencimento"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        //public DateTime dataVencimento { get; set; }

        //[Display(Name = "Data Vencimento Original"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        //public DateTime dataVencimentoOriginal { get; set; }

        [Display(Name = "Valor Bruto"), Required(ErrorMessage = "*")]
        public decimal valorBruto { get; set; }

        [Display(Name = "Tipo Documento"), Required(ErrorMessage = "*")]
        public int tipodocumento_id { get; set; }

        [Display(Name = "Tipo Lançamento"), Required(ErrorMessage = "*"), StringLength(1)]
        public string tipolancamento { get; set; }

        [Display(Name = "Número Documento"), Required(ErrorMessage = "*")]
        public int numeroDocumento { get; set; }

        [Display(Name = "Data Documento"), Required(ErrorMessage = "*"), DataType(DataType.Date)]
        public DateTime dataDocumento { get; set; }

        [Display(Name = "Situação Pagamento"), Required(ErrorMessage = "*"), StringLength(1)]
        public string situacaoPagamento { get; set; }

        //[Display(Name = "Data Pagamento"), DataType(DataType.Date)]
        //public DateTime? dataPagamanto { get; set; }

        [Display(Name = "Codigo Pagamento"),]
        public int? codigoPagamento { get; set; }

        //[Display(Name = "Lote Pagamento Banco"), StringLength(15)]
        //public string lotePagamentoBanco { get; set; }


        
        [JsonIgnore,ForeignKey("pessoa_id")]
        public virtual Pessoa Pessoa { get; set; }

        [JsonIgnore,ForeignKey("tipodocumento_id")]
        public virtual TipoDocumento tipoDocumento { get; set; }

        [Display(Name = "Documento Pagar Aprovacao")]
        public int? documentopagaraprovacao_id { get; set; }

        
        [JsonIgnore,ForeignKey("documentopagaraprovacao_id")]
        public virtual DocumentoPagarAprovacao DocumentoPagarAprovacao { get; set; }



        [Display(Name = "Banco"),Required(ErrorMessage = "*")]
        public int banco_id { get; set; }

        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco Banco { get; set; }

        [Display(Name = "Codigo de Barras"), StringLength(60)]
        public string LinhaDigitavel { get; set; }

        [Display(Name = "Pagamento"), Required(ErrorMessage = "*")]
        public int StatusPagamento { get; set; }
        /*1 - Em Aberto, 2 - Pago Parcialmente, 3 -Pago Integralmente */

        
        [JsonIgnore,ForeignKey("planoDeConta_id")]
        public virtual PlanoDeConta PlanoDeConta { get; set; }


        [Display(Name = "Plano de Conta"),
        Required(ErrorMessage = "Informe o plano de contas")]
        public int planoDeConta_id { get; set; }


        public string CodigoVerificacao { get; set; }


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

        [Required]
        public int qtdParcelas { get; set; }

        public List<DocumentoPagarParcela> DocumentoPagarParcelas { get; set; }
        [JsonIgnore]
        public List<DocumentoPagarDetalhe> DocumentoPagarDetalhes { get; set; }
        [JsonIgnore]
        public List<DocumentoPagarProjeto> DocumentoPagarProjetos { get; set; }

        [NotMapped]
        public int RepetirLancamento { get; set; }


        [NotMapped]
        public string pessoa_desc { get; set; }

        [NotMapped]
        public string urlNomeimagem { get; set; }


        public int QtdArquivosUpload { get; set; }


        public string ReferenciaProjeto  { get; set; }




        //[Display(Name = "Projeto")]
        //public int? Projeto_Id { get; set; }

        //[JsonIgnore, ForeignKey("Projeto_Id")]
        //public virtual Projeto Projeto { get; set; }
        




        public bool Excluir(int id, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            var obj = banco.DocumentoPagarMestre.Where(x => x.id == id).First();
            if (obj == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, "", "", banco, pb);
                banco.Set<DocumentoPagarMestre>().Remove(obj);
                banco.SaveChanges();
                return true;
            }
        }

        public void Alterar(ParamBase parambase, DbControle db = null)
        {
            Alterar(this, db, parambase);
        }


        public void Alterar(DocumentoPagarMestre obj,  DbControle banco, ParamBase parambase)
        {
            Alterar(obj, null, null, null, banco, parambase);
        }

        public void Alterar(DocumentoPagarMestre obj, List<DocumentoPagarDetalhe> col, List<DocumentoPagarParcela> parcelas, List<DocumentoPagarProjeto> projetos, DbControle banco, ParamBase pb)
        {
            if (banco == null)
                banco = new DbControle();

            obj.QtdArquivosUpload = banco.DocumentoPagarArquivo.Where(p => p.documentoPagarMestre_id == obj.id).Count();

            if (projetos != null)
            {
                obj.ReferenciaProjeto += "";
                foreach (var item in projetos)
                {
                    item.DocumentoPagarMestre_id = obj.id;
                }
            }


            new LogMudanca().Incluir(obj, "", "", banco, pb);
            banco.Entry(obj).State = EntityState.Modified;
            banco.SaveChanges();
            new LogMudanca().Incluir(obj, "", "", banco, pb);
            if (col != null)
            {
                SalvaItens(obj, col, parcelas, projetos, banco, true, pb);
            }
            
        }

        public void Incluir(DocumentoPagarMestre obj, List<DocumentoPagarDetalhe> col, List<DocumentoPagarParcela> parcelas, List<DocumentoPagarProjeto> projetos, ParamBase pb, DbControle banco = null)
        {
            if (banco == null)
                banco = new DbControle();

            int estab = pb.estab_id;

            obj.dataLancamento = DateTime.Now;
            



            var objAux = banco.DocumentoPagarMestre.Where(x =>
                       x.estabelecimento_id == estab
                    && x.pessoa_id == obj.pessoa_id
                    && x.numeroDocumento == obj.numeroDocumento
                           //&& x.dataVencimento == obj.dataVencimento
                    && x.valorBruto == obj.valorBruto).FirstOrDefault();

            if (objAux != null)
            {
                throw new Exception("Documento já lançado");
            }
            new LogMudanca().Incluir(obj, "", "", banco, pb);

            banco.DocumentoPagarMestre.Add(obj);
            banco.SaveChanges();

            SalvaItens(obj, col, parcelas, projetos, banco, false, pb);
        }

        private  void SalvaItens(DocumentoPagarMestre obj,
            List<DocumentoPagarDetalhe> col, 
            List<DocumentoPagarParcela> parcelas, 
            List<DocumentoPagarProjeto> projetos,
            DbControle banco, 
            bool atualiza, 
            ParamBase pb)
        {
            try
            {
                BancoMovimento bancoMovimento = new BancoMovimento();


                if (atualiza)
                {

                    var itensProjetos = new  DocumentoPagarProjeto().ObterPorCPAG(obj.id,  banco);

                    string Erro = "";

                    foreach (var item in itensProjetos)
                    {
                        item.Excluir(item.Id, ref Erro, pb, banco);
                    }

                    var itens = bancoMovimento.ObterPorCPAG(obj.id,pb, banco);

                    foreach (var item in itens)
                    {
                        if (item.pagamento_id == null)
                        {
                            item.Excluir(item.id, ref Erro, pb, banco);
                        }
                    }

                    var objitem = new DocumentoPagarDetalhe().ObterPorCPAG(obj.id, banco);

                    foreach (var item in objitem)
                    {
                        item.Excluir(item.id, ref Erro, pb, banco);
                    }

                    var objitemParcelas = new DocumentoPagarParcela().ObterPorCPAG(obj.id, banco);

                    foreach (var item in objitemParcelas)
                    {
                        var objBcoMovimento = new BancoMovimento().ObterPorCPAGParcela(item.id, pb,banco);
                        foreach (var itemBC in objBcoMovimento)
                        {
                            itemBC.Excluir(itemBC.id, ref Erro,pb, banco);
                        }
                        var objLcs = new LancamentoContabil().ObterPorCPAGParcela(item.id, pb, banco);
                        foreach (var itemLC in objLcs)
                        {
                            itemLC.Excluir(itemLC.id, ref Erro, pb, banco);
                        }
                        item.Excluir(item.id, ref Erro, pb, banco);
                    }

                    

                }



                //CRIA
                foreach (var item in col)
                {
                    var objitem = new DocumentoPagarDetalhe();
                    objitem.documentoPagarMestre_id = obj.id;
                    objitem.historico = item.historico;
                    objitem.percentual = item.percentual;

                    objitem.unidadenegocio_id = item.unidadenegocio_id;
                    objitem.valor = item.valor;
                    objitem.estabelecimento_id = pb.estab_id;
                    objitem.documentoPagarMestre_id = obj.id;
                    banco.DocumentoPagarDetalhe.Add(objitem);

                }

                banco.SaveChanges();

                if (projetos != null)
                {
                    foreach (var itemProjeto in projetos)
                    {
                        itemProjeto.DocumentoPagarMestre_id = obj.id;
                        itemProjeto.Incluir(pb, banco);
                    }
                }


                foreach (var item in parcelas)
                {
                    var objitem = new DocumentoPagarParcela();
                    objitem.DocumentoPagarMestre_id = obj.id;
                    objitem.historico = item.historico;
                    objitem.lotePagamentoBanco = item.lotePagamentoBanco;
                    objitem.parcela = item.parcela;
                    objitem.usuarioAutorizador_id = item.usuarioAutorizador_id;
                    objitem.valor = item.valor;
                    objitem.vencimento = item.vencimento;
                    objitem.vencimentoPrevisto = item.vencimentoPrevisto;
                    objitem.statusPagamento = 1;
                    banco.DocumentoPagarParcela.Add(objitem);
                    banco.SaveChanges();


                    bancoMovimento.valor = item.valor;
                    bancoMovimento.data = item.vencimentoPrevisto;
                    bancoMovimento.tipoDeMovimento_id = new TipoMovimento().TipoSaida(pb);
                    bancoMovimento.tipoDeDocumento_id = obj.tipodocumento_id;
                    bancoMovimento.origemmovimento_id = new OrigemMovimento().TipoCPAG(pb);
                    bancoMovimento.planoDeConta_id = obj.planoDeConta_id;
                    bancoMovimento.historico = "CPAG Nº " + obj.numeroDocumento + " - Parcela: " + item.parcela.ToString() ;
                    bancoMovimento.banco_id = obj.banco_id;
                    bancoMovimento.DocumentoPagarParcela_id = objitem.id;
                    bancoMovimento.usuarioalteracaoid = obj.usuarioalteracaoid;
                    bancoMovimento.dataInclusao = DateTime.Now;

                    // Inicio Lançamento Contabil

                    var idCredito = 0;
                    var idDebito = 0;

                    var pcf = new PessoaContaContabil().ObterPorPessoa(obj.pessoa_id);

                    if (pcf == null)
                    {
                        var ecf = new EmpresaContaContabil().ObterPorEmpresa(pb, banco);
                        if (ecf != null)
                        {
                            idCredito = ecf.ContaContabilTitulo_id;
                        }
                    }
                    else
                    {
                        if (pcf.contaContabilDespesaPadrao_id != null)
                        {
                            idCredito = pcf.contaContabilDespesaPadrao_id.Value;
                        }
                        else
                        {
                            var ecf = new EmpresaContaContabil().ObterPorEmpresa(pb, banco);
                            if (ecf != null)
                            {
                                idCredito = ecf.ContaContabilTitulo_id;
                            }
                        }
                    }

                    var cc = new ContaContabilCategoriaDespesa().ObterPorPlanoContas(obj.planoDeConta_id,pb,banco);

                    if (cc != null)
                    {
                        idDebito = cc.contaContabil_id;
                    }

                    if (idCredito != 0 && idDebito != 0)
                    {
                        foreach (var unidade in col)
                        {
                            var ccLC = new LancamentoContabil();
                            var numeroLcto = new EstabelecimentoCodigoLanctoContabil().ObterUltimoLacto(pb, banco);
                            ccLC.data = item.vencimentoPrevisto;
                            ccLC.dataInclusao = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                            ccLC.estabelecimento_id = pb.estab_id;
                            ccLC.historico = "CPAG Nº " + obj.numeroDocumento + " - Parcela: " + item.parcela.ToString();
                            ccLC.usuarioinclusaoid = pb.usuario_id;
                            ccLC.origemmovimento_id = new OrigemMovimento().TipoCPAG(pb);
                            ccLC.DocumentoPagarParcela_id = objitem.id;
                            ccLC.codigoLancamento = numeroLcto;
                            ccLC.UnidadeNegocio_ID = unidade.unidadenegocio_id;
                            ccLC.Incluir(pb, banco);

                            var ccDebito = new LancamentoContabilDetalhe();
                            ccDebito.lancamentoContabil_id = ccLC.id;
                            ccDebito.contaContabil_id = idDebito;
                            ccDebito.DebitoCredito = "D";
                            
                            decimal porc = (unidade.valor * 100) / obj.valorBruto;
                            decimal mult = (item.valor / 100) * porc;
                            mult = decimal.Round(mult, 2);
                            ccDebito.valor = mult;
                            ccDebito.Incluir(pb, banco);

                            var ccCredito = new LancamentoContabilDetalhe();
                            ccCredito.lancamentoContabil_id = ccLC.id;
                            ccCredito.contaContabil_id = idCredito;
                            ccCredito.DebitoCredito = "C";
                            ccCredito.valor = mult;
                            ccCredito.Incluir(pb, banco);
                        }
                    }


                    
                    //Fim Lançamento Contabil

                    new LogMudanca().Incluir(obj, "", "", banco, pb);
                    banco.BancoMovimento.Add(bancoMovimento);
                    banco.SaveChanges();
                }

                banco.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public DocumentoPagarMestre ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public DocumentoPagarMestre ObterPorId(int id, DbControle db, ParamBase pb)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarMestre.Include(p => p.DocumentoPagarParcelas).Include(p => p.DocumentoPagarProjetos).Where(x => x.id == id && x.estabelecimento_id == estab).FirstOrDefault();
        }
        public List<DocumentoPagarMestre> ObterTodos(ParamBase pb, DbControle db = null)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            return db.DocumentoPagarMestre.Include(p => p.DocumentoPagarParcelas).Where(x => x.estabelecimento_id == estab).ToList();
        }
        public IQueryable<DocumentoPagarMestre> ObterTodos2(ParamBase pb)
        {
            int estab = pb.estab_id;
            DbControle db = new DbControle();
            return db.DocumentoPagarMestre.Include(p => p.DocumentoPagarParcelas).Where(x => x.estabelecimento_id == estab);
        }

        public void AtualizaQtdArquivos(int id, ParamBase pb, DbControle db)
        {
            int estab = pb.estab_id;
            if (db == null)
                db = new DbControle();

            var obj =  db.DocumentoPagarMestre.Where(x => x.estabelecimento_id == estab && x.id == id).First();
            obj.QtdArquivosUpload = db.DocumentoPagarArquivo.Where(p => p.documentoPagarMestre_id == id).Count();
            db.SaveChanges();
        }



    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using SoftFin.Web.Negocios;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class ParcelaContrato:BaseModels
    {
        public int id { get; set; }

        [Display(Name = "Parcela"), Required(ErrorMessage = "Informe a parcela")]
        public int parcela { get; set; }
        [Display(Name = "Descrição"), Required(ErrorMessage = "Informe a descrição*")]
        public string descricao { get; set; }
        [Display(Name = "Valor"), Required(ErrorMessage = "Informe o valor")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal valor { get; set; }
        [DisplayName("Data Emissão"), Required(ErrorMessage = "Informe a data de emissão")]
        [DataType(DataType.Date)]
        public DateTime data { get; set; }

        [DisplayName("Item Contrato"), Required(ErrorMessage = "Informe o Item de Contrato Relacionado")]
        public int contratoitem_ID { get; set; }

        [DisplayName("Situação Parcela"), Required(ErrorMessage = "Informe a Situação da Parcela")]
        public int statusParcela_ID { get; set; }

        [JsonIgnore,ForeignKey("contratoitem_ID")]
        public virtual ContratoItem ContratoItem { get; set; }

        [JsonIgnore,ForeignKey("statusParcela_ID")]
        public virtual StatusParcela statusParcela { get; set; }


        [Display(Name = "Código Serviço")]
        public int? codigoServicoEstabelecimento_id { get; set; }
        
        [Display(Name = "Banco")]
        public int? banco_id { get; set; }

        [Display(Name = "Operação")]
        public int? operacao_id { get; set; }


        [JsonIgnore,ForeignKey("banco_id")]
        public virtual Banco banco { get; set; }

        [JsonIgnore,ForeignKey("operacao_id")]
        public virtual Operacao Operacao { get; set; }

        [JsonIgnore,ForeignKey("codigoServicoEstabelecimento_id")]
        public virtual CodigoServicoEstabelecimento CodigoServicoEstabelecimento { get; set; }

        public bool? NFAutomatico { get; set; }

        [NotMapped]
        public bool? recorrente { get; set; }


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
        public string Codigo { get; set; }

        public DateTime? DataVencimento { get; set; }

        //0 Servico 1 Mercadoria 2 Outros
        public int TipoFaturamento { get; set; }
        //Exclui um Parcela
        public bool ExcluiParcela(int ID, ref string erro)
        {
            try
            {
                DbControle banco = new DbControle();
                ParcelaContrato parcelaExcluir = new ParcelaContrato();

                var OV = banco.OrdemVenda.Where(x => x.parcelaContrato_ID == ID).FirstOrDefault();

                if (OV != null)
                {
                    banco.Set<OrdemVenda>().Remove(OV);
                }

                parcelaExcluir = banco.ParcelaContrato.Where(x => x.id == ID).First();
                banco.Set<ParcelaContrato>().Remove(parcelaExcluir);
                banco.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.IndexOf("The DELETE statement conflicted with the REFERENCE constraint") > -1)
                {
                    erro = "Registro não pode ser excluido, por estar relacionado com nota fiscal ou outro cadastro";
                    return false;
                }
                else
                {
                    throw ex;
                }
            }

        }
        //Inclui um Parcela
        public bool IncluiParcela(ParcelaContrato obj, string recorrente, ParamBase pb, DbControle db = null, int diaVencimento = 0, string prazo = "")
        {
            if (db == null)
                db = new DbControle();

            int estab = pb.estab_id;
            //Verificar se a parcela já existe no banco de dados
            if (recorrente.Equals("false"))
            {
                ParcelaContrato parcelaPesquisar = new ParcelaContrato();
                if (db.ParcelaContrato.Where(x => (x.contratoitem_ID == obj.contratoitem_ID) & (x.parcela == obj.parcela)).Count() != 0)
                {
                    return false;
                }
                else
                {
                    db.ParcelaContrato.Add(obj);
                    db.SaveChanges();
                    OrdemVenda OV = new OrdemVenda();

                    var contratoitem = new ContratoItem().ObterPorId(obj.contratoitem_ID, db, pb);
                    OV.data = obj.data;
                    OV.descricao = obj.descricao;
                    OV.estabelecimento_id = estab;
                    OV.parcelaContrato_ID = obj.id;
                    OV.valor = obj.valor;
                    OV.unidadeNegocio_ID = contratoitem.unidadeNegocio_ID;
                    OV.statusParcela_ID = obj.statusParcela_ID;
                    OV.pessoas_ID = contratoitem.Contrato.pessoas_ID;
                    OV.itemProdutoServico_ID = contratoitem.itemProdutoServico_ID;
                    OV.tabelaPreco_ID = contratoitem.tabelaPreco_ID;
                    OV.dataInclusao = DateTime.Now;
                    OV.usuarioinclusaoid = obj.usuarioinclusaoid;
                    


                    int numero = 1;

                    if (db.OrdemVenda.Where(p => p.estabelecimento_id == estab).FirstOrDefault() != null)
                        numero = db.OrdemVenda.Where(p => p.estabelecimento_id == estab).Max(p => p.Numero) + 1;

                    OV.Numero = numero;


                    db.OrdemVenda.Add(OV);
                    db.SaveChanges();
                    return true;
                }
            }
            if (recorrente.Equals("true"))
            {
                {
                    int qtd = obj.parcela;
                    int n = 1;
                    DateTime dataCalculada = obj.data;
                    //Se existirem mais parcelas serão incluidas aqui  
                    while (n <= qtd)
                    {
                        OrdemVenda OV = new OrdemVenda();
                        ParcelaContrato PC = new ParcelaContrato();
                        PC.data = dataCalculada;
                        PC.descricao = obj.descricao;
                        PC.valor = obj.valor;
                        PC.parcela = n;
                        PC.statusParcela_ID = obj.statusParcela_ID;
                        PC.contratoitem_ID = obj.contratoitem_ID;
                        PC.dataInclusao = DateTime.Now;
                        PC.usuarioinclusaoid = obj.usuarioinclusaoid;
                        PC.NFAutomatico = obj.NFAutomatico;
                        PC.codigoServicoEstabelecimento_id = obj.codigoServicoEstabelecimento_id;
                        PC.banco_id = obj.banco_id;
                        PC.operacao_id = obj.operacao_id;

                        if (diaVencimento != 0)
                        {
                            var dataSemFeriado = new DateTime(dataCalculada.Year, dataCalculada.Month, diaVencimento );

                            if (dataSemFeriado.DayOfWeek == DayOfWeek.Saturday)
                            {
                                PC.DataVencimento = dataSemFeriado.AddDays(2);
                            }
                            else if (dataSemFeriado.DayOfWeek == DayOfWeek.Sunday)
                            {
                                PC.DataVencimento = dataSemFeriado.AddDays(1);
                            }
                            else
                            {
                                PC.DataVencimento = dataSemFeriado;
                            }
                        }

                        db.ParcelaContrato.Add(PC);

                        db.SaveChanges();

                        var contratoitem = new ContratoItem().ObterPorId(PC.contratoitem_ID, db, pb);
                        OV.data = dataCalculada;
                        OV.descricao = obj.descricao;
                        OV.estabelecimento_id = estab;
                        OV.parcelaContrato_ID = PC.id;
                        OV.valor = obj.valor;
                        OV.unidadeNegocio_ID = contratoitem.unidadeNegocio_ID;
                        OV.statusParcela_ID = obj.statusParcela_ID;
                        OV.pessoas_ID = contratoitem.Contrato.pessoas_ID;
                        OV.itemProdutoServico_ID = contratoitem.itemProdutoServico_ID;
                        OV.tabelaPreco_ID = contratoitem.tabelaPreco_ID;
                        OV.dataInclusao = DateTime.Now;
                        OV.usuarioinclusaoid = obj.usuarioinclusaoid;

                        int numero = 1;

                        if (db.OrdemVenda.Where(p => p.estabelecimento_id == estab).FirstOrDefault() != null)
                            numero = db.OrdemVenda.Where(p => p.estabelecimento_id == estab).Max(p => p.Numero) + 1;

                        OV.Numero = numero;

                        db.OrdemVenda.Add(OV);
                        db.SaveChanges();
                        n++;
                        dataCalculada = Convert.ToDateTime(dataCalculada).AddMonths(1);
                    }

                }

            }
            if (recorrente.Equals("recorrente2"))
            {
                int qtd = obj.parcela;
                DateTime dataCalculada = obj.data;
                //Se existirem mais parcelas serão incluidas aqui  
                while 
                    (dataCalculada >= SoftFin.Utils.UtilSoftFin.DateBrasilia())
                    
                {
                    OrdemVenda OV = new OrdemVenda();
                    ParcelaContrato PC = new ParcelaContrato();
                    var dataCalc = new DateTime(dataCalculada.Year, dataCalculada.Month, 1);

                    if (dataCalc <= SoftFin.Utils.UtilSoftFin.DateBrasilia())
                        PC.data = SoftFin.Utils.UtilSoftFin.DateBrasilia();
                    else
                        PC.data = dataCalc;

                    if (prazo != "")
                    {

                        int diaprazo = 1; 

                        if (prazo == "No mês")
                        {
                            PC.DataVencimento = dataCalc;
                        }
                        else if (prazo == "Posterior")
                        {
                            PC.DataVencimento = dataCalc.AddMonths(1);
                            diaprazo = 30;
                        }
                        else if (prazo == "30 ddl")
                        {
                            PC.DataVencimento = dataCalc.AddMonths(1);
                            diaprazo = 30;
                        }
                        else if (prazo.IndexOf("ddl") > 0 )
                        {
                            diaprazo = int.Parse(prazo.Replace("ddl", ""));
                            PC.DataVencimento = dataCalc.AddDays(diaprazo);
                        }

                        if (diaVencimento != 0)
                        {
                            if (diaVencimento == 30 && PC.DataVencimento.Value.Month == 2)
                            {
                                PC.DataVencimento = new DateTime(PC.DataVencimento.Value.Year, PC.DataVencimento.Value.Month, 28);
                            }
                            else
                            {
                                PC.DataVencimento = new DateTime(PC.DataVencimento.Value.Year, PC.DataVencimento.Value.Month, diaVencimento);
                            }
                        }
                        //var dataSemFeriado = PC.DataVencimento;

                        //if (dataSemFeriado.Value.DayOfWeek == DayOfWeek.Saturday)
                        //{
                        //    PC.DataVencimento = PC.DataVencimento.Value.AddDays(2);
                        //}
                        //else if (dataSemFeriado.Value.DayOfWeek == DayOfWeek.Sunday)
                        //{
                        //    PC.DataVencimento = PC.DataVencimento.Value.AddDays(1);
                        //}

                        //PC.DataVencimento = dataSemFeriado;
                        
                    }

                    PC.descricao = obj.descricao;
                    PC.valor = obj.valor;
                    PC.parcela = qtd;
                    PC.statusParcela_ID = obj.statusParcela_ID;
                    PC.contratoitem_ID = obj.contratoitem_ID;
                    PC.dataInclusao = DateTime.Now;
                    PC.usuarioinclusaoid = obj.usuarioinclusaoid;
                    PC.NFAutomatico = obj.NFAutomatico;
                    PC.codigoServicoEstabelecimento_id = obj.codigoServicoEstabelecimento_id;
                    PC.banco_id = obj.banco_id;
                    PC.operacao_id = obj.operacao_id;

                    db.ParcelaContrato.Add(PC);

                    db.SaveChanges();

                    var contratoitem = new ContratoItem().ObterPorId(PC.contratoitem_ID, db, pb);
                    OV.data = dataCalculada;
                    OV.descricao = obj.descricao;
                    OV.estabelecimento_id = estab;
                    OV.parcelaContrato_ID = PC.id;
                    OV.valor = obj.valor;
                    OV.unidadeNegocio_ID = contratoitem.unidadeNegocio_ID;
                    OV.statusParcela_ID = obj.statusParcela_ID;
                    OV.pessoas_ID = contratoitem.Contrato.pessoas_ID;
                    OV.itemProdutoServico_ID = contratoitem.itemProdutoServico_ID;
                    OV.tabelaPreco_ID = contratoitem.tabelaPreco_ID;
                    OV.dataInclusao = DateTime.Now;
                    OV.usuarioinclusaoid = obj.usuarioinclusaoid;

                    OV.Numero = qtd;

                    db.OrdemVenda.Add(OV);
                    db.SaveChanges();
                    qtd--;
                    if (qtd == 0)
                    {
                        break;
                    }
                    dataCalculada = Convert.ToDateTime(dataCalculada).AddMonths(-1);
                }
            
            }
            return true;
        }

            //Altera Parcela
        public void AlteraParcela(ParcelaContrato obj, ParamBase pb, DbControle db = null)
        {
            //inicia contexto do banco de dados
            if (banco == null)
                db = new DbControle();

            //cria um novo usuário com os dados recebidos do formulário
            ParcelaContrato parcelaSalvar = new ParcelaContrato();
            parcelaSalvar = db.ParcelaContrato.Where(x => x.id == obj.id).First();



            //passa os novos valores para o usuário a partir dos dados alterados
            parcelaSalvar.data = obj.data;
            parcelaSalvar.parcela = obj.parcela;
            parcelaSalvar.descricao = obj.descricao;
            parcelaSalvar.valor = obj.valor;
            parcelaSalvar.contratoitem_ID = obj.contratoitem_ID;
            parcelaSalvar.statusParcela_ID = obj.statusParcela_ID;
            parcelaSalvar.banco_id = obj.banco_id;
            parcelaSalvar.codigoServicoEstabelecimento_id = obj.codigoServicoEstabelecimento_id;
            parcelaSalvar.operacao_id = obj.operacao_id;
            parcelaSalvar.NFAutomatico = obj.NFAutomatico;
            parcelaSalvar.DataVencimento = obj.DataVencimento;
            parcelaSalvar.Codigo = obj.Codigo;
            parcelaSalvar.TipoFaturamento = obj.TipoFaturamento;


            //salva no banco de dados
            db.SaveChanges();

            parcelaSalvar = db.ParcelaContrato.Where(x => x.id == obj.id).First();
            parcelaSalvar.id = obj.id;
            db.SaveChanges();


            var OV = db.OrdemVenda.Where(x => x.parcelaContrato_ID == obj.id).FirstOrDefault();

            if (OV != null)
            {
                int estab = pb.estab_id;
                var contratoitem = new ContratoItem().ObterPorId(obj.contratoitem_ID, pb);
                OV.data = obj.data;
                OV.descricao = obj.descricao;
                OV.estabelecimento_id = estab;
                OV.parcelaContrato_ID = obj.id;
                OV.valor = obj.valor;
                OV.unidadeNegocio_ID = contratoitem.unidadeNegocio_ID;
                OV.statusParcela_ID = obj.statusParcela_ID;
                OV.pessoas_ID = contratoitem.Contrato.pessoas_ID;
                OV.TipoFaturamento = obj.TipoFaturamento;
                db.SaveChanges();
            }
        }
        //Lista Parcela
        public IEnumerable<ParcelaContrato> listaParcela()
        {
            DbControle banco = new DbControle();
            IEnumerable<ParcelaContrato> lista = banco.ParcelaContrato.Include(x => x.ContratoItem).Include(y => y.statusParcela).Include(z => z.ContratoItem.UnidadeNegocio).ToList();
            return lista;
        }

        public IEnumerable<ParcelaContrato> listaParcelaPendente()
        {
            DbControle banco = new DbControle();
            IEnumerable<ParcelaContrato> lista = banco.ParcelaContrato.Include(x => x.ContratoItem).Include(y => y.statusParcela).Include(z => z.ContratoItem.UnidadeNegocio).ToList().Where(a => a.statusParcela.status == "Liberada");
            return lista;
        }

        public IEnumerable<ParcelaContrato> listaPrevisaoFaturamento()
        {
            DbControle banco = new DbControle();
            IEnumerable<ParcelaContrato> lista = banco.ParcelaContrato.Include(x => x.ContratoItem).Include(y => y.statusParcela).Include(z => z.ContratoItem.UnidadeNegocio).ToList();
            return lista;
        }


        public ParcelaContrato ObterPorId(int id, ParamBase pb)
        {
            return ObterPorId(id, null, pb);
        }
        public ParcelaContrato ObterPorId(int id, DbControle db, ParamBase pb)
        {
            if (db == null)
                db = new DbControle();
            int estab = pb.estab_id;
            return db.ParcelaContrato.Where(x => x.id == id && x.ContratoItem.Contrato.estabelecimento_id == estab).FirstOrDefault();
        }



        public List<ParcelaContrato> ObterTodos(ParamBase pb, int idContratoItem = 0)
        {
            DbControle db = new DbControle();
            int estab = pb.estab_id;
            if (idContratoItem == 0)
                return db.ParcelaContrato.Where(x => x.ContratoItem.Contrato.estabelecimento_id == estab).ToList();
            else
                return db.ParcelaContrato.Where(x => x.ContratoItem.Contrato.estabelecimento_id == estab && x.contratoitem_ID == idContratoItem).ToList();

        }

        public List<ParcelaContrato> ObterTodosEmail(int estab)
        {
            DbControle db = new DbControle();
            var dataInicial = DateTime.Now.AddDays(-1);
            var dataFinal = DateTime.Now.AddDays(7);
            return db.ParcelaContrato.Where(x => x.ContratoItem.Contrato.estabelecimento_id == estab 
                            && x.data >= dataInicial
                            && x.data <= dataFinal).ToList();
        }



        public List<ParcelaContrato> ObterEntreData(DateTime DataInicial, DateTime DataFinal, ParamBase pb, int estab = 0 )
        {
            DbControle db = new DbControle();

            if (estab == 0)
                estab = pb.estab_id;
            
            return db.ParcelaContrato.Where(x => x.ContratoItem.Contrato.estabelecimento_id == estab
                            && x.data >= DataInicial
                            && x.data <= DataFinal).ToList();

        }

        public List<ParcelaContrato> ObterEntreDataEmSituacaoEmitida(DateTime DataInicial, DateTime DataFinal, ParamBase pb, int estab = 0)
        {
            DbControle db = new DbControle();

            if (estab == 0)
                estab = pb.estab_id;
            var sitaux = StatusParcela.SituacaoEmitida();
            return db.ParcelaContrato.Where(x => x.ContratoItem.Contrato.estabelecimento_id == estab
                            && x.data >= DataInicial
                            && x.data <= DataFinal
                            && x.statusParcela_ID == sitaux).Include(p => p.ContratoItem.Contrato.Pessoa).ToList();

        }
        public List<ParcelaContrato> ObterTodosContratos(int idContrato, DateTime DataInicial, DateTime DataFinal)
        {
            DbControle db = new DbControle();
            return db.ParcelaContrato.Where(x => x.ContratoItem.Contrato.id == idContrato
                            && x.data >= DataInicial
                            && x.data <= DataFinal).ToList();

        }

        public List<ParcelaContrato> ObterTodosEstabAutomaticoEmAberto(DateTime data, DbControle db)
        {
            var dataFinal = data.AddHours(23).AddMinutes(59).AddSeconds(59);
            return db.ParcelaContrato.Where(x => x.data <= dataFinal && x.NFAutomatico == true && (x.statusParcela_ID == 4 || x.statusParcela_ID == 1)
                            ).ToList();
        }

        public bool Alterar(ParamBase paramBase, DbControle db = null)
        {
            return Alterar(this, paramBase, db);
        }
        public bool Alterar(ParcelaContrato obj, ParamBase paramBase, DbControle db = null)
        {
            if (db == null)
                db = new DbControle();

            var objAux = ObterPorId(obj.id, paramBase);
            if (objAux == null)
                return false;
            else
            {
                new LogMudanca().Incluir(obj, objAux, "", db, paramBase);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
        }

    }
}
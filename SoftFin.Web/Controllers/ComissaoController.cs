//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Sistema1.Classes;
//using Sistema1.Models;
//using Sistema1.Negocios;
//using System.Data.Entity;

//namespace Sistema1.Controllers
//{
//    public class ComissaoController : BaseController
//    {
//        public ActionResult listaComissoesAPagar()
//        {
//            CarregaViewData();
//            return View();
//        }
//        public JsonResult geraListaComissoesAPagar(string dataInicial, string dataFinal, int pessoa, int tipoPessoa, int status, int dataFiltro)
//        {
//            ViewBag.usuario = Acesso.UsuarioLogado();
//            ViewBag.perfil = Acesso.PerfilLogado();
//            int estab = Acesso.EstabLogado();

//            DateTime DataInicial = new DateTime();
//            DataInicial = DateTime.Parse(dataInicial);
//            DateTime DataFinal = new DateTime();
//            DataFinal = DateTime.Parse(dataFinal);

//            var banco = new DbControle();
//            var cp = new List<ListaContasAPagar>();

//            if (dataFiltro == 1)
//            {
//                if (status == 0)
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.DocumentoPagarMestre.dataDocumento) >= DataInicial.Date && DbFunctions.TruncateTime(m.DocumentoPagarMestre.dataDocumento) <= DataFinal.Date &&
//                              m.DocumentoPagarMestre.estabelecimento_id == estab && p.id == pessoa && m.DocumentoPagarMestre.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  //cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.DocumentoPagarMestre.dataDocumento) >= DataInicial.Date && DbFunctions.TruncateTime(m.DocumentoPagarMestre.dataDocumento) <= DataFinal.Date &&
//                                  m.DocumentoPagarMestre.estabelecimento_id == estab && m.DocumentoPagarMestre.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      //cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataDocumento) >= DataInicial && DbFunctions.TruncateTime(m.dataDocumento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//                else if (status == 1)
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.dataDocumento) >= DataInicial && DbFunctions.TruncateTime(m.dataDocumento) <= DataFinal &&
//                              m.estabelecimento_id == estab && (m.StatusPagamento == 1 || m.StatusPagamento == 2) &&
//                              p.id == pessoa && m.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataDocumento) >= DataInicial && DbFunctions.TruncateTime(m.dataDocumento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && (m.StatusPagamento == 1 || m.StatusPagamento == 2) && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataDocumento) >= DataInicial && DbFunctions.TruncateTime(m.dataDocumento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && (m.StatusPagamento == 1 || m.StatusPagamento == 2)
//                                   && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//                else
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.dataDocumento) >= DataInicial && DbFunctions.TruncateTime(m.dataDocumento) <= DataFinal &&
//                              m.estabelecimento_id == estab && (m.StatusPagamento == 3) &&
//                              p.id == pessoa && m.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataDocumento) >= DataInicial && DbFunctions.TruncateTime(m.dataDocumento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && (m.StatusPagamento == 3) && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataDocumento) >= DataInicial && DbFunctions.TruncateTime(m.dataDocumento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && (m.StatusPagamento == 3)
//                                   && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//            }
//            else if (dataFiltro == 2) //dataFiltro =2
//            {
//                if (status == 0)
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                              m.estabelecimento_id == estab && p.id == pessoa && m.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//                else if (status == 1)
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                              m.estabelecimento_id == estab && (m.StatusPagamento == 1 || m.StatusPagamento == 2) &&
//                              p.id == pessoa && m.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && (m.StatusPagamento == 1 || m.StatusPagamento == 2)
//                                   && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && (m.StatusPagamento == 1 || m.StatusPagamento == 2)
//                                   && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//                else
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                              m.estabelecimento_id == estab && (m.StatusPagamento == 3) &&
//                              p.id == pessoa && m.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && (m.StatusPagamento == 3) && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimento) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimento) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && (m.StatusPagamento == 3)
//                                   && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//            }
//            else if (dataFiltro == 3)
//            {
//                if (status == 0)
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                              m.estabelecimento_id == estab && p.id == pessoa && m.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                                  m.estabelecimento_id == estab && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//                else if (status == 1)
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                              m.estabelecimento_id == estab && (m.StatusPagamento == 1 || m.StatusPagamento == 2) &&
//                              p.id == pessoa && m.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                                  m.estabelecimento_id == estab && (m.StatusPagamento == 1 || m.StatusPagamento == 2)
//                                   && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && (m.StatusPagamento == 1 || m.StatusPagamento == 2)
//                                   && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//                else
//                {
//                    if (pessoa != 0)
//                    {
//                        cp = (from m in banco.DocumentoPagarParcela
//                              join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                              where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                              m.estabelecimento_id == estab && (m.StatusPagamento == 3) &&
//                              p.id == pessoa && m.PlanoDeConta.codigo == "04.01.05"
//                              select new ListaContasAPagar
//                              {
//                                  id = m.id,
//                                  fornecedor = p.nome,
//                                  tipoFornecedor = p.TipoPessoa.Descricao,
//                                  cnpj = p.cnpj,
//                                  dataDocumento = m.dataDocumento,
//                                  dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                  dataVencimento = m.dataVencimento,
//                                  statusPagamento = m.StatusPagamento,
//                                  contaContabil = m.PlanoDeConta.descricao,
//                                  numeroDocumento = m.numeroDocumento,
//                                  valorBruto = m.valorBruto,
//                                  valorPago = 0,
//                                  saldo = 0
//                              }).ToList();
//                    }
//                    else
//                    {
//                        if (tipoPessoa == 999999)
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                                  m.estabelecimento_id == estab && (m.StatusPagamento == 3) && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }

//                        else
//                        {
//                            cp = (from m in banco.DocumentoPagarParcela
//                                  join p in banco.Pessoa on m.DocumentoPagarMestre.pessoa_id equals p.id
//                                  where DbFunctions.TruncateTime(m.dataVencimentoOriginal) >= DataInicial && DbFunctions.TruncateTime(m.dataVencimentoOriginal) <= DataFinal &&
//                                  m.estabelecimento_id == estab && p.TipoPessoa_ID == tipoPessoa && (m.StatusPagamento == 3)
//                                   && m.PlanoDeConta.codigo == "04.01.05"
//                                  select new ListaContasAPagar
//                                  {
//                                      id = m.id,
//                                      fornecedor = p.nome,
//                                      tipoFornecedor = p.TipoPessoa.Descricao,
//                                      cnpj = p.cnpj,
//                                      dataDocumento = m.dataDocumento,
//                                      dataVencimentoOriginal = m.dataVencimentoOriginal,
//                                      dataVencimento = m.dataVencimento,
//                                      statusPagamento = m.StatusPagamento,
//                                      contaContabil = m.PlanoDeConta.descricao,
//                                      numeroDocumento = m.numeroDocumento,
//                                      valorBruto = m.valorBruto,
//                                      valorPago = 0,
//                                      saldo = 0
//                                  }).ToList();
//                        }
//                    }
//                }
//            }

//            //Fim do Case dataFiltro
//            Pagamento pagamento = new Pagamento();

//            for (int i = 0; i < cp.Count; i++)
//            {
//                cp[i].dataDocumentoS = cp[i].dataDocumento.ToString("dd/MM/yyyy");
//                cp[i].dataVencimentoOriginalS = cp[i].dataVencimentoOriginal.ToString("dd/MM/yyyy");
//                cp[i].dataVencimentoS = cp[i].dataVencimento.ToString("dd/MM/yyyy");
//                cp[i].numeroDocumentoS = cp[i].numeroDocumento.ToString();

//                if (cp[i].statusPagamento == 1)
//                    cp[i].statusPagamentoS = "Em Aberto";
//                if (cp[i].statusPagamento == 2)
//                    cp[i].statusPagamentoS = "Pago Parcial";
//                if (cp[i].statusPagamento == 3)
//                    cp[i].statusPagamentoS = "Pago Total";
//                cp[i].valorPago = pagamento.ObterValorPagoDocumento(cp[i].id);
//                cp[i].saldo = cp[i].valorBruto - cp[i].valorPago;
//            };

//            return Json(cp, JsonRequestBehavior.AllowGet);
//        }
//        private void CarregaViewData()
//        {
//            ViewData["TipoPessoa"] = new SelectList(new TipoPessoa().ObterTodos(), "id", "Descricao");
//            ViewData["Pessoa"] = new SelectList(new Pessoa().ObterTodos(), "id", "nome");

//            //Monta Lista Tipos de Data
//            var items = new List<SelectListItem>();
//            items.Add(new SelectListItem() { Value = "1", Text = "Data Documento" });
//            items.Add(new SelectListItem() { Value = "2", Text = "Data Vencimento" });
//            items.Add(new SelectListItem() { Value = "3", Text = "Vencimento Original" });
//            ViewData["DataFiltro"] = new SelectList(items, "Value", "Text");

//            //Monta Lista Status de Pagamento
//            var items2 = new List<SelectListItem>();
//            items2.Add(new SelectListItem() { Value = "1", Text = "Em Aberto" });
//            items2.Add(new SelectListItem() { Value = "2", Text = "Pago" });
//            ViewData["StatusPagamento"] = new SelectList(items2, "Value", "Text");


//        }


//    }
//}

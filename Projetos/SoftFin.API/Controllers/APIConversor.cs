using SoftFin.API.DTO;
using SoftFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.API
{
    public class APIConversor
    {
        internal List<DTOFechamentoCaixa> ToDTOFechamentoCaixa(List<LojaFechamento> objs)
        {
            var objRets = new List<DTOFechamentoCaixa>();

            foreach (var item in objs)
            {
                var objRet = new DTOFechamentoCaixa();
                objRet.codigo_estab = item.Loja.Estabelecimento.Codigo;
                objRet.data_fechamento = item.dataFechamento;
                objRet.sequencia = item.sequencia;
                objRet.descricao = item.descricao;
                objRet.codigo_loja = item.Loja.codigo;
                objRet.codigo_operador = item.LojaOperador.codigo;
                objRet.codigo_caixa = item.LojaCaixa.codigo;
                objRet.saldo_inicial = item.saldoInicial;
                objRet.saldo_final = item.saldoFinal;
                objRet.valor_bruto = item.valorBruto;
                objRet.valor_liquido = item.valorLiquido;
                objRet.valor_taxas = item.valorTaxas;
                objRets.Add(objRet);
                
            }

            return objRets;

        }

        internal List<DTOContaContabilLancamento> ToDTOContaContabilLancamento(List<LancamentoContabil> objs)
        {
            var objRets = new List<DTOContaContabilLancamento>();

            foreach (var item in objs)
            {
                var objRet = new DTOContaContabilLancamento();
                objRet.codigo_estab = item.Estabelecimento.Codigo;
                objRet.data_lancamento = item.data.ToString("o");
                objRet.historico_lancamento = item.historico;
                objRet.origem_lancamento = item.OrigemMovimento.Modulo;
                objRet.codigo_lancamento = item.codigoLancamento;

                foreach (var item2 in item.LancamentoContabilDetalhes)
                {
                    var aux = new DTOContaContabilLancamentoDetalhe();
                    aux.codigo_conta_contabil = item2.ContaContabil.codigo;
                    aux.descricao_conta_contabil = item2.ContaContabil.descricao;
                    aux.valor_lancamento = item2.valor;
                    
                    aux.flag_debito_credito = item2.DebitoCredito;

                    objRet.DTOContaContabilLancamentoDetalhes.Add(aux);
                }

                objRets.Add(objRet);
            }

            return objRets;

        }
    }
}
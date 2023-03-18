using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.NFSe.DTO
{
    public class tpNFSe
    {
        public tpNFSe()
        {
            ChaveRPS = new tpChaveRPS();
            ChaveNFe = new tpChaveNFe();
            CPFCNPJTomador = new tpCPFCNPJ();
            EnderecoTomador = new tpEndereco();
            CPFCNPJPrestador = new tpCPFCNPJ();
            EnderecoPrestador = new tpEndereco();
        }

        public string Assinatura { get; set; }
        public tpChaveRPS ChaveRPS { get; set; }
        public tpChaveNFe ChaveNFe { get; set; }
        public string TipoRPS { get; set; }
        public string DataEmissao { get; set; }
        public string StatusRPS { get; set; }
        public string TributacaoRPS { get; set; }
        public string ValorServicos { get; set; }
        public string ValorDeducoes { get; set; }
        public string ValorPIS { get; set; }
        public string ValorCOFINS { get; set; }
        public string ValorINSS { get; set; }
        public string ValorIR { get; set; }
        public string ValorCSLL { get; set; }
        public string CodigoServicos { get; set; }
        public string AliquotaServicos { get; set; }
        public tpCPFCNPJ CPFCNPJTomador { get; set; }
        public tpCPFCNPJ CPFCNPJPrestador { get; set; }
        public string InscricaoMunicipalTomador { get; set; }
        public string InscricaoEstadualTomador { get; set; }
        public string RazaoSocialTomador { get; set; }
        public tpEndereco EnderecoTomador { get; set; }
        public tpEndereco EnderecoPrestador { get; set; }
        public string EmailTomador { get; set; }
        public string Discriminacao { get; set; }
        
        public string ISSRetido { get; set; }

        public string NaturezaOperaco { get; set; }

        public string OptanteSimplesNacional { get; set; }

        public string IncentivoCultural { get; set; }

        public string Status { get; set; }

        public string ValorIss { get; set; }

        public string ValorIssRetido { get; set; }

        public string valorOutrasDeducoes { get; set; }

        public string BaseCalculo { get; set; }

        public string ValorLiquidoNfse { get; set; }

        public string ItemListaServico { get; set; }

        public string CodigoMunicipioPrestador { get; set; }

        public string CnpjPrestador { get; set; }

        public string InscricaoMunicipalPretador { get; set; }

        public string CodigoTributacaoMunicipio { get; set; }

        public string DataEmissaoNFe { get; set; }

        public string StatusNFe { get; set; }

        public string DataEmissaoRPS { get; set; }

        public string TributacaoNFe { get; set; }

        public string OpcaoSimples { get; set; }

        public string NumeroLote { get; set; }

        public string RazaoSocialPrestador { get; set; }

        public string CodigoServico { get; set; }

        public string ValorISS { get; set; }

        public string ValorCredito { get; set; }

        public string FonteCargaTributaria { get; set; }

        public string CodigoCEI { get; set; }

        public string MatriculaObra { get; set; }
        public string PercentualCargaTributaria { get; set; }
        public string ValorCargaTributaria { get; set; }
        public string ObraNumEncapsulamento { get; set; }
    }


}
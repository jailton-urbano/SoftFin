using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFSe.Business.DTO.Cidade.Barueri
{
    public class NFTexto
    {
        public NFTexto()
        {
            impostos = new List<imposto>();
        }
        public string razaoSocial { get; set; }
        public int InscricaoMunicipal { get; set; }

        public string identificacaoRemessa { get; set; }

        public string serieRPS { get; set; }

        public string serieNFE { get; set; }

        public int numeroRPS { get; set; }

        public DateTime dataEmissaoRps { get; set; }

        public string situacaoRPS { get; set; }

        public string codigomotivocancelamento { get; set; }

        public string numeroNFE { get; set; }

        public DateTime? dataEmissaoNFe { get; set; }

        public string descricaoCancelamento { get; set; }

        public string codigoServico { get; set; }

        public string localservicoprestado { get; set; }

        public string prestadoViasPublicas { get; set; }

        public string prestadoViasPublicasLogradouro { get; set; }

        public string prestadoViasPublicasNumero { get; set; }

        public string prestadoViasPublicasComplemento { get; set; }

        public string prestadoViasPublicasBairro { get; set; }

        public string prestadoViasPublicasCidade { get; set; }

        public string prestadoViasPublicasUF { get; set; }

        public string prestadoViasPublicasCEP { get; set; }

        public string QuantidadeServico { get; set; }

        public decimal ValorServico { get; set; }

        public decimal ValorTotalRetencoes { get; set; }

        public string TomadorEstrangeiro { get; set; }

        public string codigoPaisTomadorEstrangeiro { get; set; }

        public string servicoPrestadoImportacao { get; set; }



        public string indicadorCPFCNPJ { get; set; }

        public string cnpjCpf { get; set; }

        public string tomadorEndereco { get; set; }

        public string tomadorNumero { get; set; }

        public string tomadorComplemento { get; set; }

        public string tomadorCidade { get; set; }

        public string tomadorUF { get; set; }

        public string tomadorCep { get; set; }

        public string tomadorEmail { get; set; }

        public string fatura { get; set; }

        public decimal valorfatura { get; set; }

        public string discriminacaoServico { get; set; }
        public List<imposto> impostos { get; set; }


        public string tomadorBairro { get; set; }
    }

    public class imposto
    {
        public string codigoOutrosValores { get; set; }
        public decimal valor { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SoftFin.NFSe.Business.GeracaoArquivoTexto.Cidades
{
    public class Barueri
    {
        private int _contaLinha = 0;
        private decimal _ValorServicos = 0;
        private decimal _ValorDeducoes = 0;
        public StringBuilder MakeFileRPS(List<SoftFin.NFSe.Business.DTO.Cidade.Barueri.NFTexto> nf, int inscricaoMunicipal, string identificacaoRemessa)
        {
            var sb = new StringBuilder();

            Tipo1(sb, inscricaoMunicipal, identificacaoRemessa);
            Tipo2(sb, nf);
            Tipo9(sb);

            return sb;
            //var byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            //var stream = new MemoryStream(byteArray);
            //return File(stream, "text/plain", "RPS_" + apelidoEstabelecimento + "_" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".txt");
        }

        private void Tipo1(StringBuilder sb, int inscricaoMunicipal, string identificacaoRemessa)
        {
            sb.Append("1");
            sb.Append(inscricaoMunicipal.ToString("D8"));
            sb.Append("PMB002");
            sb.Append(identificacaoRemessa.PadLeft(11, ' '));
            sb.AppendLine();
            _contaLinha += 1;
        }


        private void Tipo2(StringBuilder sb, List<SoftFin.NFSe.Business.DTO.Cidade.Barueri.NFTexto> nfs)
        {
            foreach (var item in nfs)
            {
                sb.Append("1");
                sb.Append("RPS");
                sb.AppendFormat("{0,-4}", item.serieRPS);
                sb.AppendFormat("{0,-5}", " ");
                sb.AppendFormat("{0,-10}", item.numeroRPS.ToString().PadLeft(10, '0'));
                sb.AppendFormat("{0,-14:yyyyMMddhhmmss}", item.dataEmissaoRps);
                sb.AppendFormat("{0,-1}", item.situacaoRPS);
                if (item.situacaoRPS == "C")
                {
                    sb.AppendFormat("{0,-2}", item.codigomotivocancelamento);
                    sb.AppendFormat("{0,-7}", item.numeroNFE);
                    sb.AppendFormat("{0,-5}", item.serieNFE);
                    sb.AppendFormat("{0,-14:yyyyMMdd}", item.dataEmissaoNFe);
                    sb.AppendFormat("{0,-180}", item.descricaoCancelamento);
                }
                else
                {
                    sb.AppendFormat("{0,-2}", "");
                    sb.AppendFormat("{0,-7}", "");
                    sb.AppendFormat("{0,-5}", "");
                    sb.AppendFormat("{0,-14}", "");
                    sb.AppendFormat("{0,-180}", "");
                }
                sb.AppendFormat("{0,-9}", item.codigoServico);
                sb.AppendFormat("{0,-1}", item.localservicoprestado);
                sb.AppendFormat("{0,-1}", item.prestadoViasPublicas);
                if (item.prestadoViasPublicas == "1")
                {
                    sb.AppendFormat("{0,-75}", item.prestadoViasPublicasLogradouro);
                    sb.AppendFormat("{0,-9}", item.prestadoViasPublicasNumero);
                    sb.AppendFormat("{0,-30}", item.prestadoViasPublicasComplemento);
                    sb.AppendFormat("{0,-40}", item.prestadoViasPublicasBairro);
                    sb.AppendFormat("{0,-40}", item.prestadoViasPublicasCidade);
                    sb.AppendFormat("{0,-2}", item.prestadoViasPublicasUF);
                    sb.AppendFormat("{0,-8}", item.prestadoViasPublicasCEP.Replace("-", ""));
                }
                else
                {
                    sb.AppendFormat("{0,-75}", "");
                    sb.AppendFormat("{0,-9}", "");
                    sb.AppendFormat("{0,-30}", "");
                    sb.AppendFormat("{0,-40}", "");
                    sb.AppendFormat("{0,-40}", "");
                    sb.AppendFormat("{0,-2}", "");
                    sb.AppendFormat("{0,-8}", "");
                }
                sb.AppendFormat("{0,-6}", item.QuantidadeServico);
                sb.AppendFormat("{0,-15}", item.ValorServico.ToString("0.00").Replace(",", "").PadLeft(15, '0'));
                _ValorServicos += item.ValorServico;

                sb.AppendFormat("{0,-5}", "");
                sb.AppendFormat("{0,-15}", item.ValorTotalRetencoes.ToString("0.00").Replace(",", "").PadLeft(15, '0'));
                _ValorDeducoes += item.ValorTotalRetencoes;

                sb.AppendFormat("{0,-1}", item.TomadorEstrangeiro);

                if (item.TomadorEstrangeiro == "1")
                {
                    sb.AppendFormat("{0,-3}", item.codigoPaisTomadorEstrangeiro);
                    sb.AppendFormat("{0,-1}", item.servicoPrestadoImportacao);
                    sb.AppendFormat("{0,-1}", "");
                    sb.AppendFormat("{0,-14}", "");
                    sb.AppendFormat("{0,-60}", item.razaoSocial);
                    sb.AppendFormat("{0,-75}", "");
                    sb.AppendFormat("{0,-9}", "");
                    sb.AppendFormat("{0,-30}", "");
                    sb.AppendFormat("{0,-40}", "");
                    sb.AppendFormat("{0,-40}", "");
                    sb.AppendFormat("{0,-2}", "");
                    sb.AppendFormat("{0,-8}", "");
                    sb.AppendFormat("{0,-152}", "");
                }
                else
                {
                    sb.AppendFormat("{0,-3}", "");
                    sb.AppendFormat("{0,-1}", "");
                    sb.AppendFormat("{0,-1}", item.indicadorCPFCNPJ);
                    sb.AppendFormat("{0,-14}", item.cnpjCpf);
                    sb.AppendFormat("{0,-60}", item.razaoSocial);
                    sb.AppendFormat("{0,-75}", item.tomadorEndereco);
                    sb.AppendFormat("{0,-9}", item.tomadorNumero);
                    sb.AppendFormat("{0,-30}", item.tomadorComplemento);
                    sb.AppendFormat("{0,-40}", item.tomadorBairro);
                    sb.AppendFormat("{0,-40}", item.tomadorCidade);
                    sb.AppendFormat("{0,-2}", item.tomadorUF);
                    sb.AppendFormat("{0,-8}", item.tomadorCep.Replace("-", ""));
                    sb.AppendFormat("{0,-152}", item.tomadorEmail);
                }


                sb.AppendFormat("{0,-6}", item.fatura);
                sb.AppendFormat("{0,-15}", item.valorfatura.ToString("0.00").Replace(",", "").PadLeft(15, '0'));
                sb.AppendFormat("{0,-1000}", item.discriminacaoServico.Replace("\n", "|").Replace("\r", ""));
                sb.AppendLine();
                _contaLinha += 1;

                Tipo3(sb, item);
            }

        }

        private void Tipo3(StringBuilder sb, SoftFin.NFSe.Business.DTO.Cidade.Barueri.NFTexto nf)
        {
            foreach (var item in nf.impostos)
            {
                sb.Append("3");
                sb.AppendFormat("{0,-2}", item.codigoOutrosValores);
                sb.AppendFormat("{0,-15}", item.valor.ToString("0.00").Replace(",", "").PadLeft(15, '0')); 
                sb.AppendLine();
                _contaLinha += 1;
            }
            
        }


        private void Tipo9(StringBuilder sb)
        {
            _contaLinha += 1;
            sb.Append("9");
            sb.AppendFormat("{0,-7}", _contaLinha.ToString().PadLeft(7, '0'));//8) Valor das Deduções
            sb.AppendFormat("{0,-15}", _ValorServicos.ToString("0.00").Replace(",", "").PadLeft(15, '0'));//8) Valor dos serviços
            sb.AppendFormat("{0,-15}", _ValorDeducoes.ToString("0.00").Replace(",", "").PadLeft(15, '0'));//8) Valor das Deduções
            sb.AppendLine();

        }



    }
}

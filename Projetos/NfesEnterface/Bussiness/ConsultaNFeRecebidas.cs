using SoftFin.NFSe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoftFin.NFSe.SaoPaulo.Bussiness
{
    public class ConsultaNFeRecebidas
    {
        public DTORetornoNFEs Execute(DTONotaFiscal dTOConsultaNFSe, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            return Processar(dTOConsultaNFSe, cert);
        }


        private DTORetornoNFEs Processar(DTONotaFiscal pedidoConsultaNFe, X509Certificate2 certificadoPrestador)
        {
            XmlDocument nfes = GeraXMLNFSe(pedidoConsultaNFe, certificadoPrestador);

            XmlDocument retornoAssinado = new SoftFin.NFSe.SaoPaulo.Utils.AssinaturaDigital().AplicaAssinatura(
                                                                    nfes, "p1:PedidoConsultaNFePeriodo", certificadoPrestador);

            //retornoAssinado.Save("c:\\lixo\\nfConsulta.xml");

            var retorno = EnviarPeloWebServico(certificadoPrestador, nfes);

            var retornofinal = ProcessaRetornoXML(retorno);
            retornofinal.xml = retornoAssinado.InnerXml;
            return retornofinal;
        }

        private static string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
        {
            var servico = new SoftFin.NFSe.SaoPaulo.InterfaceService.LoteNFe();
            servico.ClientCertificates.Add(certificadoPrestador);
            var retorno = servico.ConsultaNFeRecebidas(1, nfes.OuterXml);
            return retorno;
        }

        private DTORetornoNFEs ProcessaRetornoXML(string retornoEnvioLoteRPS)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(retornoEnvioLoteRPS));
            XmlNodeList procEventoNFe = xmlDocument.GetElementsByTagName("RetornoConsulta");
            XmlNodeList cabecalho = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Cabecalho");
            XmlNodeList NFes = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("NFe");
            XmlNodeList erros = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Erro");
            XmlNodeList alertas = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Alerta");

            var resultingMessage = new DTORetornoNFEs();

            resultingMessage.Cabecalho.Sucesso = cabecalho[0].ChildNodes[0].InnerText;
            
            foreach (XmlElement item in NFes)
            {
                var nFe = new tpNFSe();

                var chaveNFe = new tpChaveNFe();
                var chaveNFeNode = BuscaElementoXML(item, "ChaveNFe");
                chaveNFe.CodigoVerificacao = BuscaElementoXML(chaveNFeNode, "CodigoVerificacao").InnerText;
                chaveNFe.InscricaoPrestador = BuscaElementoXML(chaveNFeNode, "InscricaoPrestador").InnerText;
                chaveNFe.NumeroNFe = BuscaElementoXML(chaveNFeNode, "NumeroNFe").InnerText;
                nFe.ChaveNFe = chaveNFe;


                var chaveRPS = new tpChaveRPS();
                var chaveRPSNode = BuscaElementoXML(item, "ChaveRPS");
                if (chaveRPSNode != null)
                {
                    chaveRPS.InscricaoPrestador = BuscaElementoXMLToString(chaveRPSNode, "InscricaoPrestador");
                    chaveRPS.NumeroRPS = BuscaElementoXMLToString(chaveRPSNode, "NumeroRPS");
                    chaveRPS.SerieRPS = BuscaElementoXMLToString(chaveRPSNode, "SerieRPS");
                    nFe.ChaveRPS = chaveRPS;
                }

                var enderecoTomador = new tpEndereco();
                var enderecoTomadorNode = BuscaElementoXML(item, "EnderecoTomador");
                enderecoTomador.Bairro = BuscaElementoXMLToString(enderecoTomadorNode, "Bairro");
                enderecoTomador.CEP = BuscaElementoXMLToString(enderecoTomadorNode, "CEP");
                enderecoTomador.Cidade = BuscaElementoXMLToString(enderecoTomadorNode, "Cidade");
                enderecoTomador.ComplementoEndereco = BuscaElementoXMLToString(enderecoTomadorNode, "ComplementoEndereco");
                enderecoTomador.Logradouro = BuscaElementoXMLToString(enderecoTomadorNode, "Logradouro");
                enderecoTomador.NumeroEndereco = BuscaElementoXMLToString(enderecoTomadorNode, "NumeroEndereco");
                enderecoTomador.TipoLogradouro = BuscaElementoXMLToString(enderecoTomadorNode, "TipoLogradouro");
                enderecoTomador.UF = BuscaElementoXMLToString(enderecoTomadorNode, "UF");
                nFe.EnderecoTomador = enderecoTomador;


                var enderecoPrestador = new tpEndereco();
                var enderecoPrestadorNode = BuscaElementoXML(item, "EnderecoPrestador");
                enderecoPrestador.Bairro = BuscaElementoXMLToString(enderecoPrestadorNode, "Bairro");
                enderecoPrestador.CEP = BuscaElementoXMLToString(enderecoPrestadorNode, "CEP");
                enderecoPrestador.Cidade = BuscaElementoXMLToString(enderecoPrestadorNode, "Cidade");
                enderecoPrestador.ComplementoEndereco = BuscaElementoXMLToString(enderecoPrestadorNode, "ComplementoEndereco");
                enderecoPrestador.Logradouro = BuscaElementoXMLToString(enderecoPrestadorNode, "Logradouro");
                enderecoPrestador.NumeroEndereco = BuscaElementoXMLToString(enderecoPrestadorNode, "NumeroEndereco");
                enderecoPrestador.TipoLogradouro = BuscaElementoXMLToString(enderecoPrestadorNode, "TipoLogradouro");
                enderecoPrestador.UF = BuscaElementoXMLToString(enderecoPrestadorNode, "UF");
                nFe.EnderecoPrestador = enderecoPrestador;


                var cpfcnpjPrestador = new tpCPFCNPJ();
                var cpfcnpjPrestadorNode = BuscaElementoXML(item, "CPFCNPJPrestador");
                cpfcnpjPrestador.CNPJ = BuscaElementoXMLToString(cpfcnpjPrestadorNode, "CNPJ");
                cpfcnpjPrestador.CPF = BuscaElementoXMLToString(cpfcnpjPrestadorNode, "CPF");
                nFe.CPFCNPJPrestador = cpfcnpjPrestador;

                var cpfcnpjTomador = new tpCPFCNPJ();
                var cpfcnpjTomadorNode = BuscaElementoXML(item, "CPFCNPJTomador");
                cpfcnpjTomador.CNPJ = BuscaElementoXMLToString(cpfcnpjTomadorNode, "CNPJ");
                cpfcnpjTomador.CPF = BuscaElementoXMLToString(cpfcnpjTomadorNode, "CPF");
                nFe.CPFCNPJTomador = cpfcnpjTomador;

                nFe.Assinatura = BuscaElementoXMLToString(item, "Assinatura");
                nFe.DataEmissaoNFe = BuscaElementoXMLToString(item, "DataEmissaoNFe");
                nFe.NumeroLote = BuscaElementoXMLToString(item, "NumeroLote");
                nFe.TipoRPS = BuscaElementoXMLToString(item, "TipoRPS");
                nFe.DataEmissaoRPS = BuscaElementoXMLToString(item, "DataEmissaoRPS");
                nFe.RazaoSocialPrestador = BuscaElementoXMLToString(item, "RazaoSocialPrestador");
                nFe.StatusNFe = BuscaElementoXMLToString(item, "StatusNFe");
                nFe.TributacaoNFe = BuscaElementoXMLToString(item, "TributacaoNFe");
                nFe.OpcaoSimples = BuscaElementoXMLToString(item, "OpcaoSimples");
                nFe.ValorServicos = BuscaElementoXMLToString(item, "ValorServicos");
                nFe.ValorDeducoes = BuscaElementoXMLToString(item, "ValorDeducoes");
                nFe.ValorPIS = BuscaElementoXMLToString(item, "ValorPIS");
                nFe.ValorCOFINS = BuscaElementoXMLToString(item, "ValorCOFINS");
                nFe.ValorINSS = BuscaElementoXMLToString(item, "ValorINSS");
                nFe.ValorIR = BuscaElementoXMLToString(item, "ValorIR");
                nFe.ValorCSLL = BuscaElementoXMLToString(item, "ValorCSLL");
                nFe.CodigoServico = BuscaElementoXMLToString(item, "CodigoServico");
                nFe.AliquotaServicos = BuscaElementoXMLToString(item, "AliquotaServicos");
                nFe.ValorISS = BuscaElementoXMLToString(item, "ValorISS");
                nFe.ValorCredito = BuscaElementoXMLToString(item, "ValorCredito");
                nFe.ISSRetido = BuscaElementoXMLToString(item, "ISSRetido");
                nFe.EmailTomador = BuscaElementoXMLToString(item, "EmailTomador");
                nFe.Discriminacao = BuscaElementoXMLToString(item, "Discriminacao");
                nFe.FonteCargaTributaria = BuscaElementoXMLToString(item, "FonteCargaTributaria");

                
                resultingMessage.NFe.Add(nFe);
            }


            foreach (XmlElement item in erros)
            {
                resultingMessage.Erro.Add(new TPErro
                {
                    Codigo = item.ChildNodes[0].InnerText,
                    Descricao = item.ChildNodes[1].InnerText
                });
            }

            foreach (XmlElement item in alertas)
            {
                resultingMessage.Alerta.Add(new tpAlerta
                {
                    Codigo = item.ChildNodes[0].InnerText,
                    Descricao = item.ChildNodes[1].InnerText
                });
            }
            return resultingMessage;
        }

        public string BuscaElementoXMLToString(XmlElement nodes, String nomeNome)
        {
            foreach (XmlElement item in nodes)
            {
                if (item.Name == nomeNome)
                    return item.InnerText.ToString();
            }
            return "";
        }

        public XmlElement BuscaElementoXML(XmlElement nodes, String nomeNome)
        {
            foreach (XmlElement item in nodes)
            {
                if (item.Name == nomeNome)
                    return item;
            }
            return null;
        }

        private XmlDocument GeraXMLNFSe(DTONotaFiscal pedidoConsultaNFe, X509Certificate2 certificadoPrestador)
        {
            List<int> nfs = new List<int>();

            var envio = pedidoConsultaNFe;

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;

            XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xml_declaration, document_element);
            XmlNode nodePrincipal = xmlDocument.CreateElement("p1", "PedidoConsultaNFePeriodo", "http://www.prefeitura.sp.gov.br/nfe");

            XmlAttribute attribute = xmlDocument.CreateAttribute("xmlns:xsi");
            attribute.Value = "http://www.w3.org/2001/XMLSchema-instance";
            nodePrincipal.Attributes.Append(attribute);

            //XmlAttribute attribute2 = xmlDocument.CreateAttribute("xmlns:xsd");
            //attribute2.Value = "http://www.w3.org/2001/XMLSchema";
            //nodePrincipal.Attributes.Append(attribute2);

            //XmlAttribute attribute3 = xmlDocument.CreateAttribute("xmlns:p1");
            //attribute3.Value = "http://www.prefeitura.sp.gov.br/nfe";
            //nodePrincipal.Attributes.Append(attribute3);

            var nodeCabecalho = xmlDocument.CreateElement("Cabecalho");
            nodePrincipal.AppendChild(nodeCabecalho);
            xmlDocument.AppendChild(nodePrincipal);

            if ((!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ)) ||
                (!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CPF)))
            {
                var nodeCPFCNPJRemetente = xmlDocument.CreateElement("CPFCNPJRemetente");
                nodeCabecalho.AppendChild(nodeCPFCNPJRemetente);
                if (!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ))
                {
                    SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCPFCNPJRemetente, "CNPJ", pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ);
                }
                else
                {
                    SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCPFCNPJRemetente, "CPF", pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CPF);
                }

                if ((!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ))
                    ||
                    (!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CPF)))
                {
                    var nodeCPFCNPJ = xmlDocument.CreateElement("CPFCNPJ");
                    nodeCabecalho.AppendChild(nodeCPFCNPJ);
                    if (!string.IsNullOrEmpty(pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ))
                    {
                        SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCPFCNPJ, "CNPJ", pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CNPJ);
                    }
                    else
                    {
                        SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCPFCNPJ, "CPF", pedidoConsultaNFe.Cabecalho.CPFCNPJRemetente.CPF);
                    }
                }

                SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCabecalho, "Inscricao", pedidoConsultaNFe.Cabecalho.Inscricao);
                SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCabecalho, "dtInicio", pedidoConsultaNFe.Cabecalho.dtInicio);
                SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCabecalho, "dtFim", pedidoConsultaNFe.Cabecalho.dtFim);
                SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCabecalho, "NumeroPagina", "1");

            
            }




            SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoAtributo(xmlDocument, nodeCabecalho, "Versao", "1");
            SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoAtributo(xmlDocument, nodeCabecalho, "xmlns", "");
            return xmlDocument;
        }



    }
}

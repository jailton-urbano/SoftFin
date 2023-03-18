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
    public class CancelaNFe
    {
        public DTORetornoNFEs Execute(DTONotaFiscal pedidoCancelamentoNFe, X509Certificate2 certificadoPrestador)
        {
            return Processar(pedidoCancelamentoNFe, certificadoPrestador);
        }


        private DTORetornoNFEs Processar(DTONotaFiscal pedidoCancelamentoNFe, X509Certificate2 certificadoPrestador)
        {
            XmlDocument nfes = GeraXMLNFSe(pedidoCancelamentoNFe, certificadoPrestador);

            XmlDocument retornoAssinado = new SoftFin.NFSe.SaoPaulo.Utils.AssinaturaDigital().AplicaAssinatura(
                                                                    nfes, "PedidoCancelamentoNFe", certificadoPrestador);

            //retornoAssinado.Save("c:\\lixo\\nfCancelamento.xml");

            var retorno = EnviarPeloWebServico(certificadoPrestador, nfes);
            var retornofinal =  ProcessaRetornoXML(retorno);
            retornofinal.xml = retornoAssinado.InnerXml;
            return retornofinal;
            
        }

        private static string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
        {
            var servico = new SoftFin.NFSe.SaoPaulo.InterfaceService.LoteNFe();
            servico.ClientCertificates.Add(certificadoPrestador);
            var retorno = servico.CancelamentoNFe(1, nfes.OuterXml);
            return retorno;
        }

        private DTORetornoNFEs ProcessaRetornoXML(string retornoEnvioLoteRPS)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(retornoEnvioLoteRPS));
            XmlNodeList procEventoNFe = xmlDocument.GetElementsByTagName("RetornoCancelamentoNFe");
            XmlNodeList cabecalho = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Cabecalho");
            XmlNodeList NFes = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("NFe");
            XmlNodeList erros = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Erro");
            XmlNodeList alertas = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Alerta");

            var resultingMessage = new DTORetornoNFEs();

            resultingMessage.Cabecalho.Sucesso = cabecalho[0].ChildNodes[0].InnerText;

            foreach (XmlElement item in NFes)
            {
                var chaveNFe = new tpChaveNFe();
                var chaveNFeNode = BuscaElementoXML(item, "ChaveNFe");
                chaveNFe.CodigoVerificacao = BuscaElementoXML(chaveNFeNode, "CodigoVerificacao").InnerText;
                chaveNFe.InscricaoPrestador = BuscaElementoXML(chaveNFeNode, "InscricaoPrestador").InnerText;
                chaveNFe.NumeroNFe = BuscaElementoXML(chaveNFeNode, "NumeroNFe").InnerText;


                var nFe = new tpNFSe();
                nFe.StatusNFe = BuscaElementoXML(item, "StatusNFe").InnerText;
                nFe.DataEmissaoNFe = BuscaElementoXML(item, "DataEmissaoNFe").InnerText;
                nFe.ChaveNFe = chaveNFe;
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

        public XmlElement BuscaElementoXML(XmlElement nodes, String nomeNome)
        {
            foreach (XmlElement item in nodes)
            {
                if (item.Name == nomeNome)
                    return item;
            }
            return null;
        }

        private XmlDocument GeraXMLNFSe(DTONotaFiscal pedidoCancelamentoNFe, X509Certificate2 certificadoPrestador)
        {
            List<int> nfs = new List<int>();

            var envio = new DTORetornoNFEs();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = true;

            XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = xmlDocument.DocumentElement;
            xmlDocument.InsertBefore(xml_declaration, document_element);
            XmlNode nodePrincipal = xmlDocument.CreateElement("PedidoCancelamentoNFe", "http://www.prefeitura.sp.gov.br/nfe");

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

            if ((!string.IsNullOrEmpty(pedidoCancelamentoNFe.Cabecalho.CPFCNPJRemetente.CNPJ)) ||
                (!string.IsNullOrEmpty(pedidoCancelamentoNFe.Cabecalho.CPFCNPJRemetente.CPF)))
            {
                var nodeCPFCNPJRemetente = xmlDocument.CreateElement("CPFCNPJRemetente");
                nodeCabecalho.AppendChild(nodeCPFCNPJRemetente);
                if (!string.IsNullOrEmpty(pedidoCancelamentoNFe.NFSe.First().CPFCNPJPrestador.CNPJ))
                {
                    SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCPFCNPJRemetente, "CNPJ", pedidoCancelamentoNFe.NFSe.First().CPFCNPJPrestador.CNPJ);
                }
                else
                {
                    SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCPFCNPJRemetente, "CPF", pedidoCancelamentoNFe.NFSe.First().CPFCNPJPrestador.CPF);

                }
            }

            SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeCabecalho, "transacao", "false");

            //var nodeDetalheNFe = xmlDocument.CreateElement("Detalhe");
            //nodePrincipal.AppendChild(nodeDetalheNFe);
            //SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeDetalheNFe, "InscricaoPrestador", pedidoCancelamentoNFe.Detalhe.ChaveNFe.InscricaoPrestador);
            //SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeDetalheNFe, "NumeroNFe", pedidoCancelamentoNFe.Detalhe.ChaveNFe.NumeroNFe);


            var nodeDetalhe = xmlDocument.CreateElement("Detalhe");
            nodePrincipal.AppendChild(nodeDetalhe);


            var nodeChaveNFSe = xmlDocument.CreateElement("ChaveNFe");
            nodeDetalhe.AppendChild(nodeChaveNFSe);

            SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeChaveNFSe, "InscricaoPrestador", pedidoCancelamentoNFe.NFSe.First().CPFCNPJPrestador.InscricaoMunicipal);
            SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoNode(xmlDocument, nodeChaveNFSe, "NumeroNFe", pedidoCancelamentoNFe.NFSe.First().ChaveNFe.NumeroNFe);


            var nodeAssinaturaCancelamento = xmlDocument.CreateElement("AssinaturaCancelamento");

            var assinatura = GeraAssinaturaNota(certificadoPrestador, pedidoCancelamentoNFe);
            nodeAssinaturaCancelamento.InnerText = assinatura;
            nodeDetalhe.AppendChild(nodeAssinaturaCancelamento);

            SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoAtributo(xmlDocument, nodeCabecalho, "Versao", "1");
            SoftFin.NFSe.SaoPaulo.Utils.Util.AdicionaNovoAtributo(xmlDocument, nodeCabecalho, "xmlns", "");
            return xmlDocument;
        }

        private string GeraAssinaturaNota(X509Certificate2 certificadoPrestador, DTONotaFiscal itemNF)
        {
            StringBuilder hashAssinatura = new StringBuilder();
            hashAssinatura.Append(itemNF.NFSe.First().ChaveNFe.InscricaoPrestador.PadRight(8, '0')); //1- Inscrição Municipal do Prestador
            hashAssinatura.Append(itemNF.NFSe.First().ChaveNFe.NumeroNFe.PadLeft(12, '0')); //3- Número do RPS
            var assinatura = new SoftFin.NFSe.SaoPaulo.Utils.Assinador().AssinarRPSSP(certificadoPrestador, hashAssinatura.ToString());
            return assinatura;
        }


    }
}

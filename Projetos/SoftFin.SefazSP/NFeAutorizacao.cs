using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SoftFin.Sefaz
{
	public class NFeAutorizacao
	{
		string _UrlWebservice = "";
		string _UrlWebserviceRet = ""; 

		public DTORetornoNFe Execute(DTONfe dTONfe, System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
				bool simulaXML = false, 
				string caminhoXSD="", 
				string urlServico = "",
				string urlServicoRet = "")
		{
			_UrlWebservice = urlServico;
			_UrlWebserviceRet = urlServicoRet;
			return Processar(dTONfe, cert,simulaXML, caminhoXSD);
		}


		public XmlDocument AplicaAssinaturaNFe(XmlDocument docXML, string grupo, string uri, X509Certificate2 cert)
		{
			try
			{
				XmlDocument docRequest = new XmlDocument();
				docRequest.PreserveWhitespace = true;
				docRequest.LoadXml(docXML.InnerXml.ToString());
				

				XmlNodeList ListNFe = docRequest.GetElementsByTagName("infNFe");

				foreach (XmlElement itemNFe in ListNFe)
				{

					Reference reference = new Reference();

					reference.Uri = "#" + itemNFe.Attributes.GetNamedItem("Id").InnerText;
					// Create a SignedXml object.
					SignedXml signedXml = new SignedXml(docRequest);
					// Add the key to the SignedXml document
					signedXml.SigningKey = cert.PrivateKey;

					// Add an enveloped transformation to the reference.
					XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
					reference.AddTransform(env);
					XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
					reference.AddTransform(c14);
					// Add the reference to the SignedXml object.
					signedXml.AddReference(reference);
					// Create a new KeyInfo object
					KeyInfo keyInfo = new KeyInfo();
					// Load the certificate into a KeyInfoX509Data object
					// and add it to the KeyInfo object.
					keyInfo.AddClause(new KeyInfoX509Data(cert));
					// Add the KeyInfo object to the SignedXml object.
					signedXml.KeyInfo = keyInfo;
					signedXml.ComputeSignature();
					// Get the XML representation of the signature and save
					// it to an XmlElement object.
					XmlElement xmlDigitalSignature = signedXml.GetXml();



					//var signedXml = new SignedXml(itemNFe);
					//signedXml.SigningKey = cert.PrivateKey;

					//Reference reference = new Reference("#" + id);
					//reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
					//reference.AddTransform(new XmlDsigC14NTransform());
					//signedXml.AddReference(reference);

					//KeyInfo keyInfo = new KeyInfo();
					//keyInfo.AddClause(new KeyInfoX509Data(cert));

					//signedXml.KeyInfo = keyInfo;

					//signedXml.ComputeSignature();

					//XmlElement xmlSignature = docRequest.CreateElement("Signature", "http://www.w3.org/2000/09/xmldsig#");
					//XmlElement xmlSignedInfo = signedXml.SignedInfo.GetXml();
					//XmlElement xmlKeyInfo = signedXml.KeyInfo.GetXml();

					//XmlElement xmlSignatureValue = docRequest.CreateElement("SignatureValue", xmlSignature.NamespaceURI);
					//string signBase64 = Convert.ToBase64String(signedXml.Signature.SignatureValue);
					//XmlText text = docRequest.CreateTextNode(signBase64);
					//xmlSignatureValue.AppendChild(text);

					//xmlSignature.AppendChild(docRequest.ImportNode(xmlSignedInfo, true));
					//xmlSignature.AppendChild(xmlSignatureValue);
					//xmlSignature.AppendChild(docRequest.ImportNode(xmlKeyInfo, true));

					var evento = docRequest.GetElementsByTagName(uri);
					evento[0].AppendChild(xmlDigitalSignature);
					//infNFe.AppendChild(xmlSignature);

				}



				return docRequest;
			}
			catch (Exception erro) { throw erro; }
		}


		private DTORetornoNFe Processar(DTONfe dTONfe, X509Certificate2 certificadoPrestador,
				bool simulaXML = false, string caminhoXSD="")
		{
			XmlDocument nfe = GeraXMLNFSe(dTONfe, certificadoPrestador);

			XmlDocument retornoAssinado = AplicaAssinaturaNFe(
			   nfe, "InfNFe", "NFe", certificadoPrestador);

			var retorno = "";
			var retornofinal = new DTORetornoNFe();
			

			if (!simulaXML)
			{
				DTORetornoNFe retornoAgendamento = new DTORetornoNFe();
				retornoAgendamento.Erros = new List<DTOErro>();
				//@"C:\Users\Ricardo\OneDrive\Projeto2\XSDDOCS\NFe\3_10\enviNFe_v3.10.xsd"

				retornoAssinado.Schemas.Add("http://www.portalfiscal.inf.br/nfe", caminhoXSD);

				retornoAgendamento.Sucesso = true;
				retornoAssinado.Validate((sender, args) =>
				{
					var exception = (args.Exception as XmlSchemaValidationException);

					if (exception != null)
					{

						retornoAgendamento.Sucesso = false;
						retornoAgendamento.Erros.Add(new DTOErro { codigo = "XSDVALIDACAO", descricao = exception.Message });
					 }
				});


				if (retornoAgendamento.Sucesso)
				{

					retorno = EnviarPeloWebServico(certificadoPrestador, retornoAssinado);

					retornoAgendamento = ProcessaRetornoXMLAgendamento(retorno);

					var tentativa = 0;
					while (tentativa < 5)
					{
						XmlDocument nferet = GeraXMLNFSeRet(retornoAgendamento.nRec, certificadoPrestador);
						retorno = EnviarPeloWebServiceRetorno(certificadoPrestador, nferet);
						retornofinal = ProcessaRetornoXML(retorno);
						

						if (retornofinal.Erros.Count() > 0)
						{
							if (retornofinal.Erros.First().codigo != "105")
							{
								break;
							}
						}
						else
						{
							break;
						}
						System.Threading.Thread.Sleep(5000);
						tentativa = +1;
					}
				}
				else
				{
					retornofinal = retornoAgendamento;
				}
			}
			retornofinal.xml = retornoAssinado;
			retornofinal.Sucesso = retornofinal.Sucesso;
			
			retornofinal.tipo = "NFe";
			return retornofinal;
		}

		private XmlDocument GeraXMLNFSeRet(string  nRec, X509Certificate2 certificadoPrestador)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;

			XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
			XmlElement document_element = xmlDocument.DocumentElement;
			xmlDocument.InsertBefore(xml_declaration, document_element);

			XmlNode nodePrincipal = xmlDocument.CreateElement("consReciNFe");
			xmlDocument.AppendChild(nodePrincipal);

			XmlAttribute attribute = xmlDocument.CreateAttribute("xmlns");
			attribute.Value = "http://www.portalfiscal.inf.br/nfe";
			nodePrincipal.Attributes.Append(attribute);



			XmlAttribute attributev = xmlDocument.CreateAttribute("versao");
			attributev.Value = "4.00";
			nodePrincipal.Attributes.Append(attributev);
			if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
			{
				AdicionaNovoNode(xmlDocument, nodePrincipal, "tpAmb", "1");
			}
			else
			{
				AdicionaNovoNode(xmlDocument, nodePrincipal, "tpAmb", "2");
			}
			AdicionaNovoNode(xmlDocument, nodePrincipal, "nRec", nRec);

			return xmlDocument;
		}

		private string EnviarPeloWebServico(X509Certificate2 certificadoPrestador, XmlDocument nfes)
		{

			var servico = new SoftFin.Sefaz.srwsNFeAutorizacao.NFeAutorizacao4Soap12Client("NFeAutorizacao4Soap121", _UrlWebservice);

            servico.ClientCredentials.ServiceCertificate.DefaultCertificate = certificadoPrestador;
			servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;

            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(nfes.InnerXml.ToString());
            var retorno = servico.nfeAutorizacaoLote(xmld);

			return retorno.InnerXml;
		}


		private string EnviarPeloWebServiceRetorno(X509Certificate2 certificadoPrestador, XmlDocument nfes)
		{


			var servico = new SoftFin.Sefaz.srwsNFeRetornoAutorizacao.NFeRetAutorizacao4Soap12Client("NFeRetAutorizacao4Soap12", _UrlWebserviceRet);
			servico.ClientCredentials.ClientCertificate.Certificate = certificadoPrestador;

			XmlDocument xmld = new XmlDocument();
			xmld.LoadXml(nfes.InnerXml.ToString());
			var retorno = servico.nfeRetAutorizacaoLote(xmld);

			return retorno.InnerXml;
		}

		private DTORetornoNFe ProcessaRetornoXMLAgendamento(string retornoEnvioLoteRPS)
		{
			XmlDocument xmld = new XmlDocument();
			retornoEnvioLoteRPS = "<retorno>" + retornoEnvioLoteRPS + "</retorno>";
			xmld.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(retornoEnvioLoteRPS));
			XmlNodeList procEventoNFe = xmld.GetElementsByTagName("infRec");
			var xMotivo = xmld.GetElementsByTagName("xMotivo")[0].InnerText;

			

			var dTORetornoNFe = new DTORetornoNFe();

			dTORetornoNFe.Sucesso = (xMotivo == "Lote recebido com sucesso")? true: false;

			if (dTORetornoNFe.Sucesso)
			{
				XmlNodeList cabecalho = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("nRec");
				dTORetornoNFe.nRec = cabecalho[0].InnerText;
			}
			else
			{
				var cstat = xmld.GetElementsByTagName("cStat")[0].InnerText;
				dTORetornoNFe.Erros = new List<DTOErro>();
				dTORetornoNFe.Erros.Add(new DTOErro { codigo = cstat, descricao = xMotivo });
			}

			return dTORetornoNFe;

			//XmlElement codigoMensagem = (procEventoNFe).GetElementsByTagName("351000110879738");

			//if (retornoEnvioLoteRPS.Contains("Nenhum documento localizado para o destinatario"))
			//{
			//    resultingMessage.Cabecalho.Sucesso = "true";
			//    resultingMessage.Alerta.Add(new tpAlerta { Codigo = "137", Descricao = "Nenhum documento localizado para o destinatario" });
			//}

		}
		

		private DTORetornoNFe ProcessaRetornoXML(string retornoEnvioLoteRPS)
		{
			var resultingMessage = new DTORetornoNFe();

			XmlDocument xmlDocument = new XmlDocument();;
			retornoEnvioLoteRPS = "<retorno>" + retornoEnvioLoteRPS + "</retorno>";
			resultingMessage.xmlRetorno = retornoEnvioLoteRPS;
			xmlDocument.Load(SoftFin.Utils.UtilSoftFin.GenerateStreamFromString(retornoEnvioLoteRPS));
			var cstat = xmlDocument.GetElementsByTagName("cStat")[0].InnerText;
			var xMotivo = xmlDocument.GetElementsByTagName("xMotivo")[0].InnerText;


			if (cstat == "100")
			{
				resultingMessage.Sucesso = true;
			}
			else 
			{
				if (cstat == "104")
				{
					resultingMessage.Sucesso = false;
					resultingMessage.Erros = new List<DTOErro>();
					resultingMessage.Alertas = new List<DTOErro>();
					resultingMessage.Alertas.Add(new DTOErro { codigo = cstat, descricao = xMotivo });

					XmlNodeList infProt = xmlDocument.GetElementsByTagName("infProt");
					resultingMessage.Erros = new List<DTOErro>();
					foreach (XmlElement item in infProt)
					{
						var cStat2 = item.GetElementsByTagName("cStat")[0].InnerText;
						var xMotivo2 = item.GetElementsByTagName("xMotivo")[0].InnerText;
						if (cStat2 == "100")
						{
							var chNFe = xmlDocument.GetElementsByTagName("chNFe")[0].InnerText;
							var nProt = xmlDocument.GetElementsByTagName("nProt")[0].InnerText;
							resultingMessage.chaveAcesso = chNFe;
							resultingMessage.protocoloAutorizacao = nProt;
							resultingMessage.Sucesso = true;
							resultingMessage.Alertas.Add(new DTOErro { codigo = cStat2, descricao = xMotivo2 });
						}
						else
						{
							resultingMessage.Erros.Add(new DTOErro { codigo = cStat2, descricao = xMotivo2 });
						}
					}



				}
				else
				{
					resultingMessage.Sucesso = false;
					resultingMessage.Erros = new List<DTOErro>();
					resultingMessage.Erros.Add(new DTOErro { codigo = cstat, descricao = xMotivo });
				}
			}
			return resultingMessage;


			//XmlElement codigoMensagem = (cabecalho).GetElementsByTagName("cStat"); // BuscaElementoXML(cabecalho, "cStat");

			//resultingMessage.Cabecalho.Sucesso = cabecalho[0].ChildNodes[0].InnerText;

			//foreach (XmlElement item in NFes)
			//{
			//    var nFe = new tpNFSe();

			//    var chaveNFe = new tpChaveNFe();
			//    var chaveNFeNode = BuscaElementoXML(item, "ChaveNFe");
			//    chaveNFe.CodigoVerificacao = BuscaElementoXML(chaveNFeNode, "CodigoVerificacao").InnerText;
			//    chaveNFe.InscricaoPrestador = BuscaElementoXML(chaveNFeNode, "InscricaoPrestador").InnerText;
			//    chaveNFe.NumeroNFe = BuscaElementoXML(chaveNFeNode, "NumeroNFe").InnerText;
			//    nFe.ChaveNFe = chaveNFe;


			//    var chaveRPS = new tpChaveRPS();
			//    var chaveRPSNode = BuscaElementoXML(item, "ChaveRPS");
			//    if (chaveRPSNode != null)
			//    {
			//        chaveRPS.InscricaoPrestador = BuscaElementoXMLToString(chaveRPSNode, "InscricaoPrestador");
			//        chaveRPS.NumeroRPS = BuscaElementoXMLToString(chaveRPSNode, "NumeroRPS");
			//        chaveRPS.SerieRPS = BuscaElementoXMLToString(chaveRPSNode, "SerieRPS");
			//        nFe.ChaveRPS = chaveRPS;
			//    }

			//    var enderecoTomador = new tpEndereco();
			//    var enderecoTomadorNode = BuscaElementoXML(item, "EnderecoTomador");
			//    enderecoTomador.Bairro = BuscaElementoXMLToString(enderecoTomadorNode, "Bairro");
			//    enderecoTomador.CEP = BuscaElementoXMLToString(enderecoTomadorNode, "CEP");
			//    enderecoTomador.Cidade = BuscaElementoXMLToString(enderecoTomadorNode, "Cidade");
			//    enderecoTomador.ComplementoEndereco = BuscaElementoXMLToString(enderecoTomadorNode, "ComplementoEndereco");
			//    enderecoTomador.Logradouro = BuscaElementoXMLToString(enderecoTomadorNode, "Logradouro");
			//    enderecoTomador.NumeroEndereco = BuscaElementoXMLToString(enderecoTomadorNode, "NumeroEndereco");
			//    enderecoTomador.TipoLogradouro = BuscaElementoXMLToString(enderecoTomadorNode, "TipoLogradouro");
			//    enderecoTomador.UF = BuscaElementoXMLToString(enderecoTomadorNode, "UF");
			//    nFe.EnderecoTomador = enderecoTomador;


			//    var enderecoPrestador = new tpEndereco();
			//    var enderecoPrestadorNode = BuscaElementoXML(item, "EnderecoPrestador");
			//    enderecoPrestador.Bairro = BuscaElementoXMLToString(enderecoPrestadorNode, "Bairro");
			//    enderecoPrestador.CEP = BuscaElementoXMLToString(enderecoPrestadorNode, "CEP");
			//    enderecoPrestador.Cidade = BuscaElementoXMLToString(enderecoPrestadorNode, "Cidade");
			//    enderecoPrestador.ComplementoEndereco = BuscaElementoXMLToString(enderecoPrestadorNode, "ComplementoEndereco");
			//    enderecoPrestador.Logradouro = BuscaElementoXMLToString(enderecoPrestadorNode, "Logradouro");
			//    enderecoPrestador.NumeroEndereco = BuscaElementoXMLToString(enderecoPrestadorNode, "NumeroEndereco");
			//    enderecoPrestador.TipoLogradouro = BuscaElementoXMLToString(enderecoPrestadorNode, "TipoLogradouro");
			//    enderecoPrestador.UF = BuscaElementoXMLToString(enderecoPrestadorNode, "UF");
			//    nFe.EnderecoPrestador = enderecoPrestador;


			//    var cpfcnpjPrestador = new tpCPFCNPJ();
			//    var cpfcnpjPrestadorNode = BuscaElementoXML(item, "CPFCNPJPrestador");
			//    cpfcnpjPrestador.CNPJ = BuscaElementoXMLToString(cpfcnpjPrestadorNode, "CNPJ");
			//    cpfcnpjPrestador.CPF = BuscaElementoXMLToString(cpfcnpjPrestadorNode, "CPF");
			//    nFe.CPFCNPJPrestador = cpfcnpjPrestador;

			//    var cpfcnpjTomador = new tpCPFCNPJ();
			//    var cpfcnpjTomadorNode = BuscaElementoXML(item, "CPFCNPJTomador");
			//    cpfcnpjTomador.CNPJ = BuscaElementoXMLToString(cpfcnpjTomadorNode, "CNPJ");
			//    cpfcnpjTomador.CPF = BuscaElementoXMLToString(cpfcnpjTomadorNode, "CPF");
			//    nFe.CPFCNPJTomador = cpfcnpjTomador;

			//    nFe.Assinatura = BuscaElementoXMLToString(item, "Assinatura");
			//    nFe.DataEmissaoNFe = BuscaElementoXMLToString(item, "DataEmissaoNFe");
			//    nFe.NumeroLote = BuscaElementoXMLToString(item, "NumeroLote");
			//    nFe.TipoRPS = BuscaElementoXMLToString(item, "TipoRPS");
			//    nFe.DataEmissaoRPS = BuscaElementoXMLToString(item, "DataEmissaoRPS");
			//    nFe.RazaoSocialPrestador = BuscaElementoXMLToString(item, "RazaoSocialPrestador");
			//    nFe.StatusNFe = BuscaElementoXMLToString(item, "StatusNFe");
			//    nFe.TributacaoNFe = BuscaElementoXMLToString(item, "TributacaoNFe");
			//    nFe.OpcaoSimples = BuscaElementoXMLToString(item, "OpcaoSimples");
			//    nFe.ValorServicos = BuscaElementoXMLToString(item, "ValorServicos");
			//    nFe.ValorDeducoes = BuscaElementoXMLToString(item, "ValorDeducoes");
			//    nFe.ValorPIS = BuscaElementoXMLToString(item, "ValorPIS");
			//    nFe.ValorCOFINS = BuscaElementoXMLToString(item, "ValorCOFINS");
			//    nFe.ValorINSS = BuscaElementoXMLToString(item, "ValorINSS");
			//    nFe.ValorIR = BuscaElementoXMLToString(item, "ValorIR");
			//    nFe.ValorCSLL = BuscaElementoXMLToString(item, "ValorCSLL");
			//    nFe.CodigoServico = BuscaElementoXMLToString(item, "CodigoServico");
			//    nFe.AliquotaServicos = BuscaElementoXMLToString(item, "AliquotaServicos");
			//    nFe.ValorISS = BuscaElementoXMLToString(item, "ValorISS");
			//    nFe.ValorCredito = BuscaElementoXMLToString(item, "ValorCredito");
			//    nFe.ISSRetido = BuscaElementoXMLToString(item, "ISSRetido");
			//    nFe.EmailTomador = BuscaElementoXMLToString(item, "EmailTomador");
			//    nFe.Discriminacao = BuscaElementoXMLToString(item, "Discriminacao");
			//    nFe.FonteCargaTributaria = BuscaElementoXMLToString(item, "FonteCargaTributaria");


			//    resultingMessage.NFe.Add(nFe);
			//}


			//foreach (XmlElement item in erros)
			//{
			//    resultingMessage.Erro.Add(new tpErro
			//    {
			//        Codigo = item.ChildNodes[0].InnerText,
			//        Descricao = item.ChildNodes[1].InnerText
			//    });
			//}

			//foreach (XmlElement item in alertas)
			//{
			//    resultingMessage.Alerta.Add(new tpAlerta
			//    {
			//        Codigo = item.ChildNodes[0].InnerText,
			//        Descricao = item.ChildNodes[1].InnerText
			//    });
			//}
 
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

		private XmlDocument GeraXMLNFSe(DTONfe dTONfe, X509Certificate2 certificadoPrestador)
		{
			List<int> nfs = new List<int>();


			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;

			XmlDeclaration xml_declaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
			XmlElement document_element = xmlDocument.DocumentElement;
			xmlDocument.InsertBefore(xml_declaration, document_element);

			XmlNode nodeEnviNFe = xmlDocument.CreateElement("enviNFe");
			xmlDocument.AppendChild(nodeEnviNFe);

			XmlAttribute attribute = xmlDocument.CreateAttribute("xmlns");
			attribute.Value = "http://www.portalfiscal.inf.br/nfe";
			nodeEnviNFe.Attributes.Append(attribute);


			XmlAttribute attributeVersao = xmlDocument.CreateAttribute("versao");
			attributeVersao.Value = "4.00";
			nodeEnviNFe.Attributes.Append(attributeVersao);
			
			AdicionaNovoNode(xmlDocument, nodeEnviNFe, "idLote", dTONfe.idLote);
			AdicionaNovoNode(xmlDocument, nodeEnviNFe, "indSinc", "0"); 


			XmlNode nodeNFe = xmlDocument.CreateElement("NFe");
			nodeEnviNFe.AppendChild(nodeNFe);

			XmlNode nodeInfNFe = xmlDocument.CreateElement("infNFe");
			nodeNFe.AppendChild(nodeInfNFe);

			XmlAttribute attributev = xmlDocument.CreateAttribute("versao");
			attributev.Value = "4.00";
			nodeInfNFe.Attributes.Append(attributev);

			XmlAttribute attributeId = xmlDocument.CreateAttribute("Id");
			attributeId.Value = dTONfe.InfNFe.Id;
			nodeInfNFe.Attributes.Append(attributeId);


			XmlNode nodeide = xmlDocument.CreateElement("ide");
			nodeInfNFe.AppendChild(nodeide);
			
			AdicionaNovoNode(xmlDocument, nodeide, "cUF", dTONfe.InfNFe.Ide.cUF);
			AdicionaNovoNode(xmlDocument, nodeide, "cNF", dTONfe.InfNFe.Ide.cNF);
			AdicionaNovoNode(xmlDocument, nodeide, "natOp", dTONfe.InfNFe.Ide.natOp);
			//AdicionaNovoNode(xmlDocument, nodeide, "indPag", dTONfe.InfNFe.Ide.indPag);
			AdicionaNovoNode(xmlDocument, nodeide, "mod", dTONfe.InfNFe.Ide.mod);
			AdicionaNovoNode(xmlDocument, nodeide, "serie", dTONfe.InfNFe.Ide.serie);
			AdicionaNovoNode(xmlDocument, nodeide, "nNF", dTONfe.InfNFe.Ide.nNF);
			AdicionaNovoNode(xmlDocument, nodeide, "dhEmi", dTONfe.InfNFe.Ide.dhEmi);
			AdicionaNovoNode(xmlDocument, nodeide, "dhSaiEnt", dTONfe.InfNFe.Ide.dhSaiEnt);
			AdicionaNovoNode(xmlDocument, nodeide, "tpNF", dTONfe.InfNFe.Ide.tpNF);
			AdicionaNovoNode(xmlDocument, nodeide, "idDest", dTONfe.InfNFe.Ide.idDest);
			AdicionaNovoNode(xmlDocument, nodeide, "cMunFG", dTONfe.InfNFe.Ide.cMunFG);
			AdicionaNovoNode(xmlDocument, nodeide, "tpImp", dTONfe.InfNFe.Ide.tpImp);
			AdicionaNovoNode(xmlDocument, nodeide, "tpEmis", dTONfe.InfNFe.Ide.tpEmis);
			AdicionaNovoNode(xmlDocument, nodeide, "cDV", dTONfe.InfNFe.Ide.cDV);
			if (ConfigurationManager.AppSettings["ProductionServiceNF"].ToLower().Equals("true"))
			{
				AdicionaNovoNode(xmlDocument, nodeide, "tpAmb", "1");
			}
			else
			{
				AdicionaNovoNode(xmlDocument, nodeide, "tpAmb", "2");
			}
			AdicionaNovoNode(xmlDocument, nodeide, "finNFe", dTONfe.InfNFe.Ide.finNFe);
			AdicionaNovoNode(xmlDocument, nodeide, "indFinal", dTONfe.InfNFe.Ide.indFinal);
			AdicionaNovoNode(xmlDocument, nodeide, "indPres", dTONfe.InfNFe.Ide.indPres);
			AdicionaNovoNode(xmlDocument, nodeide, "procEmi", dTONfe.InfNFe.Ide.procEmi);
			AdicionaNovoNode(xmlDocument, nodeide, "verProc", dTONfe.InfNFe.Ide.verProc);

			#region NFref
			if (dTONfe.InfNFe.Ide.NFref.Count() > 0 )
			{
				foreach (var item in dTONfe.InfNFe.Ide.NFref)
				{
					XmlNode nodeNFref = xmlDocument.CreateElement("NFref");
					nodeide.AppendChild(nodeNFref);
					AdicionaNovoNode(xmlDocument, nodeNFref, "refNFe", item.refNFe);

					XmlNode noderefNF = xmlDocument.CreateElement("refNF");
					nodeNFref.AppendChild(nodeNFref);
					AdicionaNovoNode(xmlDocument, noderefNF, "cUF", item.refNF.cUF);
					AdicionaNovoNode(xmlDocument, noderefNF, "AAMM", item.refNF.AAMM);
					AdicionaNovoNode(xmlDocument, noderefNF, "CPF", item.refNF.CPF);
					AdicionaNovoNode(xmlDocument, noderefNF, "mod", item.refNF.mod);
					AdicionaNovoNode(xmlDocument, noderefNF, "serie", item.refNF.serie);
					AdicionaNovoNode(xmlDocument, noderefNF, "nNF", item.refNF.nNF);

					XmlNode noderefNFP = xmlDocument.CreateElement("refNFP");
					nodeNFref.AppendChild(noderefNFP);
					AdicionaNovoNode(xmlDocument, noderefNFP, "cUF", item.refNFP.cUF);
					AdicionaNovoNode(xmlDocument, noderefNFP, "AAMM", item.refNFP.AAMM);
					AdicionaNovoNode(xmlDocument, noderefNFP, "CPF", item.refNFP.CPF);
					AdicionaNovoNode(xmlDocument, noderefNFP, "IE", item.refNFP.IE);
					AdicionaNovoNode(xmlDocument, noderefNFP, "mod", item.refNFP.mod);
					AdicionaNovoNode(xmlDocument, noderefNFP, "serie", item.refNFP.serie);
					AdicionaNovoNode(xmlDocument, noderefNFP, "nNF", item.refNFP.nNF);

					XmlNode noderefECF = xmlDocument.CreateElement("refECF");
					nodeNFref.AppendChild(noderefECF);
					AdicionaNovoNode(xmlDocument, noderefECF, "mod", item.refECF.mod);
					AdicionaNovoNode(xmlDocument, noderefECF, "nECF", item.refECF.nECF);
					AdicionaNovoNode(xmlDocument, noderefECF, "nCOO", item.refECF.nCOO);
				}
			}
			#endregion

			#region Emit
			XmlNode nodeemit = xmlDocument.CreateElement("emit");
			nodeInfNFe.AppendChild(nodeemit);
			if (!string.IsNullOrEmpty(dTONfe.InfNFe.Emi.CNPJ))
				AdicionaNovoNode(xmlDocument, nodeemit, "CNPJ", dTONfe.InfNFe.Emi.CNPJ);
			else    
				AdicionaNovoNode(xmlDocument, nodeemit, "CPF", dTONfe.InfNFe.Emi.CPF);

			AdicionaNovoNode(xmlDocument, nodeemit, "xNome", dTONfe.InfNFe.Emi.xNome);
			AdicionaNovoNode(xmlDocument, nodeemit, "xFant", dTONfe.InfNFe.Emi.xFant);

			XmlNode nodeenderEmit = xmlDocument.CreateElement("enderEmit");
			nodeemit.AppendChild(nodeenderEmit);

			AdicionaNovoNode(xmlDocument, nodeenderEmit, "xLgr", dTONfe.InfNFe.Emi.EnderEmit.xLgr);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "nro", dTONfe.InfNFe.Emi.EnderEmit.nro);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "xBairro", dTONfe.InfNFe.Emi.EnderEmit.xBairro);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "cMun", dTONfe.InfNFe.Emi.EnderEmit.cMun);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "xMun", dTONfe.InfNFe.Emi.EnderEmit.xMun);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "UF", dTONfe.InfNFe.Emi.EnderEmit.UF);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "CEP", dTONfe.InfNFe.Emi.EnderEmit.CEP);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "cPais", dTONfe.InfNFe.Emi.EnderEmit.cPais);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "xPais", dTONfe.InfNFe.Emi.EnderEmit.xPais);
			AdicionaNovoNode(xmlDocument, nodeenderEmit, "fone", dTONfe.InfNFe.Emi.EnderEmit.fone);

			AdicionaNovoNode(xmlDocument, nodeemit, "IE", dTONfe.InfNFe.Emi.IE);
			AdicionaNovoNode(xmlDocument, nodeemit, "CRT", dTONfe.InfNFe.Emi.CRT);

			#endregion

			XmlNode nodedest = xmlDocument.CreateElement("dest");
			nodeInfNFe.AppendChild(nodedest);
			if (!string.IsNullOrEmpty(dTONfe.InfNFe.Dest.CNPJ))
				AdicionaNovoNode(xmlDocument, nodedest, "CNPJ", dTONfe.InfNFe.Dest.CNPJ);
			else
				AdicionaNovoNode(xmlDocument, nodedest, "CPF", dTONfe.InfNFe.Dest.CPF);

			AdicionaNovoNode(xmlDocument, nodedest, "xNome", dTONfe.InfNFe.Dest.xNome);

			#region enderDest

			XmlNode nodeenderDest = xmlDocument.CreateElement("enderDest");
			nodedest.AppendChild(nodeenderDest);

			AdicionaNovoNode(xmlDocument, nodeenderDest, "xLgr", dTONfe.InfNFe.Dest.EnderDest.xLgr);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "nro", dTONfe.InfNFe.Dest.EnderDest.nro);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "xBairro", dTONfe.InfNFe.Dest.EnderDest.xBairro);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "cMun", dTONfe.InfNFe.Dest.EnderDest.cMun);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "xMun", dTONfe.InfNFe.Dest.EnderDest.xMun);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "UF", dTONfe.InfNFe.Dest.EnderDest.UF);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "CEP", dTONfe.InfNFe.Dest.EnderDest.CEP);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "cPais", dTONfe.InfNFe.Dest.EnderDest.cPais);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "xPais", dTONfe.InfNFe.Dest.EnderDest.xPais);
			AdicionaNovoNode(xmlDocument, nodeenderDest, "fone", dTONfe.InfNFe.Dest.EnderDest.fone);

			AdicionaNovoNode(xmlDocument, nodedest, "indIEDest", dTONfe.InfNFe.Dest.indIEDest);

			if (dTONfe.InfNFe.Dest.indIEDest == "1")
				AdicionaNovoNode(xmlDocument, nodedest, "IE", dTONfe.InfNFe.Dest.IE);

			#endregion

			#region retirada
			if (!string.IsNullOrEmpty(dTONfe.InfNFe.Retirada.xLgr))
			{
				XmlNode noderetirada = xmlDocument.CreateElement("retirada");
				nodedest.AppendChild(noderetirada);

				if (!string.IsNullOrEmpty(dTONfe.InfNFe.Retirada.CNPJ))
					AdicionaNovoNode(xmlDocument, noderetirada, "CNPJ", dTONfe.InfNFe.Retirada.CNPJ);
				else
					AdicionaNovoNode(xmlDocument, noderetirada, "CPF", dTONfe.InfNFe.Retirada.CPF);


				AdicionaNovoNode(xmlDocument, noderetirada, "xLgr", dTONfe.InfNFe.Retirada.xLgr);
				AdicionaNovoNode(xmlDocument, noderetirada, "nro", dTONfe.InfNFe.Retirada.nro);
				AdicionaNovoNode(xmlDocument, noderetirada, "xCpl", dTONfe.InfNFe.Retirada.xCpl);
				AdicionaNovoNode(xmlDocument, noderetirada, "xBairro", dTONfe.InfNFe.Retirada.xBairro);
				AdicionaNovoNode(xmlDocument, noderetirada, "cMun", dTONfe.InfNFe.Retirada.cMun);
				AdicionaNovoNode(xmlDocument, noderetirada, "xMun", dTONfe.InfNFe.Retirada.xMun);
				AdicionaNovoNode(xmlDocument, noderetirada, "UF", dTONfe.InfNFe.Retirada.UF);
			}
			#endregion
			
			#region entrega
			if (!string.IsNullOrEmpty(dTONfe.InfNFe.Entrega.xLgr))
			{
				XmlNode nodeentrega = xmlDocument.CreateElement("entrega");
				nodedest.AppendChild(nodeentrega);

				if (!string.IsNullOrEmpty(dTONfe.InfNFe.Entrega.CNPJ))
					AdicionaNovoNode(xmlDocument, nodeentrega, "CNPJ", dTONfe.InfNFe.Entrega.CNPJ);
				else
					AdicionaNovoNode(xmlDocument, nodeentrega, "CPF", dTONfe.InfNFe.Entrega.CPF);


				AdicionaNovoNode(xmlDocument, nodeentrega, "xLgr", dTONfe.InfNFe.Entrega.xLgr);
				AdicionaNovoNode(xmlDocument, nodeentrega, "nro", dTONfe.InfNFe.Entrega.nro);
				AdicionaNovoNode(xmlDocument, nodeentrega, "xCpl", dTONfe.InfNFe.Entrega.xCpl);
				AdicionaNovoNode(xmlDocument, nodeentrega, "xBairro", dTONfe.InfNFe.Entrega.xBairro);
				AdicionaNovoNode(xmlDocument, nodeentrega, "cMun", dTONfe.InfNFe.Entrega.cMun);
				AdicionaNovoNode(xmlDocument, nodeentrega, "xMun", dTONfe.InfNFe.Entrega.xMun);
				AdicionaNovoNode(xmlDocument, nodeentrega, "UF", dTONfe.InfNFe.Entrega.UF);
			}
			#endregion

			#region autXML
			foreach (var item in dTONfe.InfNFe.AutXML)
			{

				XmlNode nodeautXML = xmlDocument.CreateElement("autXML");
				nodeInfNFe.AppendChild(nodeautXML);
				if (!string.IsNullOrEmpty(item.CNPJ))
					AdicionaNovoNode(xmlDocument, nodeautXML, "CNPJ", item.CNPJ);
				else
					AdicionaNovoNode(xmlDocument, nodeautXML, "CPF", item.CPF);
			}
			#endregion


			#region Det
			foreach (var item in dTONfe.InfNFe.Det)
			{
				XmlNode nodedet = xmlDocument.CreateElement("det");
				nodeInfNFe.AppendChild(nodedet);

				XmlAttribute attributenItem = xmlDocument.CreateAttribute("nItem");
				attributenItem.Value = item.nItem;
				nodedet.Attributes.Append(attributenItem);

				XmlNode nodeprod = xmlDocument.CreateElement("prod");
				nodedet.AppendChild(nodeprod);

				AdicionaNovoNode(xmlDocument, nodeprod, "cProd", item.Prod.cProd);
				AdicionaNovoNode(xmlDocument, nodeprod, "cEAN", item.Prod.cEAN);
				AdicionaNovoNode(xmlDocument, nodeprod, "xProd", item.Prod.xProd);
				AdicionaNovoNode(xmlDocument, nodeprod, "NCM", item.Prod.NCM);
				if (!string.IsNullOrEmpty(item.Prod.CEST))
					 AdicionaNovoNode(xmlDocument, nodeprod, "CEST", item.Prod.CEST);

				AdicionaNovoNode(xmlDocument, nodeprod, "CFOP", item.Prod.CFOP);
				AdicionaNovoNode(xmlDocument, nodeprod, "uCom", item.Prod.uCom);
				AdicionaNovoNode(xmlDocument, nodeprod, "qCom", item.Prod.qCom);
				AdicionaNovoNode(xmlDocument, nodeprod, "vUnCom", item.Prod.vUnCom);
				AdicionaNovoNode(xmlDocument, nodeprod, "vProd", item.Prod.vProd);
				AdicionaNovoNode(xmlDocument, nodeprod, "cEANTrib", item.Prod.cEANTrib);
				AdicionaNovoNode(xmlDocument, nodeprod, "uTrib", item.Prod.uTrib);
				AdicionaNovoNode(xmlDocument, nodeprod, "qTrib", item.Prod.qTrib);
				AdicionaNovoNode(xmlDocument, nodeprod, "vUnTrib", item.Prod.vUnTrib);
				if (item.Prod.vDesc != "0.00")
					AdicionaNovoNode(xmlDocument, nodeprod, "vDesc", item.Prod.vDesc);
				AdicionaNovoNode(xmlDocument, nodeprod, "indTot", item.Prod.indTot);
				AdicionaNovoNode(xmlDocument, nodeprod, "nItemPed", item.Prod.nItemPed);

				XmlNode nodeimposto = xmlDocument.CreateElement("imposto");
				nodedet.AppendChild(nodeimposto);

				if (!string.IsNullOrEmpty(item.Imposto.vTotTrib))
					AdicionaNovoNode(xmlDocument, nodeimposto, "vTotTrib", item.Imposto.vTotTrib);

				if (dTONfe.InfNFe.Emi.CRT == "1")
				{
					switch (item.Prod.CSOSN)
					{
                        case "101":
                        case "102":
						case "103":
						case "300":
						case "400":
							XmlNode nodeICMS = xmlDocument.CreateElement("ICMS");
							nodeimposto.AppendChild(nodeICMS);
							XmlNode nodeICMSSN101 = xmlDocument.CreateElement("ICMSSN102");
							nodeICMS.AppendChild(nodeICMSSN101);
							AdicionaNovoNode(xmlDocument, nodeICMSSN101, "orig", item.Prod.orig);
							AdicionaNovoNode(xmlDocument, nodeICMSSN101, "CSOSN", item.Prod.CSOSN);
							break;
						default:
							throw new Exception("ICMS não configurado");
					}
				}

				XmlNode nodePIS = xmlDocument.CreateElement("PIS");
				nodeimposto.AppendChild(nodePIS);
				switch (item.Imposto.PIS.CST)
				{
					case "01":
						XmlNode nodePISAliq = xmlDocument.CreateElement("PISAliq");
						nodePIS.AppendChild(nodePISAliq);
						AdicionaNovoNode(xmlDocument, nodePISAliq, "CST", item.Imposto.PIS.CST);
						AdicionaNovoNode(xmlDocument, nodePISAliq, "vBC", item.Imposto.PIS.vBC);
						AdicionaNovoNode(xmlDocument, nodePISAliq, "pPIS", item.Imposto.PIS.pPIS);
						AdicionaNovoNode(xmlDocument, nodePISAliq, "vPIS", item.Imposto.PIS.vPIS);
						break;
					case "07":
					case "08":
						XmlNode nodePISNT = xmlDocument.CreateElement("PISNT");
						nodePIS.AppendChild(nodePISNT);
						AdicionaNovoNode(xmlDocument, nodePISNT, "CST", item.Imposto.PIS.CST);
						break;
					default:
						throw new Exception("ICMS não configurado");
				}





				XmlNode nodeCOFINS = xmlDocument.CreateElement("COFINS");
				nodeimposto.AppendChild(nodeCOFINS);



				switch (item.Imposto.COFINS.CST)
				{
					case "01":
						XmlNode nodeCOFINSAliq = xmlDocument.CreateElement("COFINSAliq");
						nodeCOFINS.AppendChild(nodeCOFINSAliq);
						AdicionaNovoNode(xmlDocument, nodeCOFINSAliq, "CST", item.Imposto.COFINS.CST);
						AdicionaNovoNode(xmlDocument, nodeCOFINSAliq, "vBC", item.Imposto.COFINS.vBC);
						AdicionaNovoNode(xmlDocument, nodeCOFINSAliq, "pCOFINS", item.Imposto.COFINS.pCOFINS);
						AdicionaNovoNode(xmlDocument, nodeCOFINSAliq, "vCOFINS", item.Imposto.COFINS.vCOFINS);
						break;
					case "07":
					case "08":
						XmlNode nodeCOFINSNT = xmlDocument.CreateElement("COFINSNT");
						nodeCOFINS.AppendChild(nodeCOFINSNT);
						AdicionaNovoNode(xmlDocument, nodeCOFINSNT, "CST", item.Imposto.COFINS.CST);
						break;
					default:
						throw new Exception("ICMS não configurado");
				}



				
				AdicionaNovoNode(xmlDocument, nodedet, "infAdProd", item.Prod.infAdProd);






			}
			#endregion



			#region total
			XmlNode nodetotal = xmlDocument.CreateElement("total");
			nodeInfNFe.AppendChild(nodetotal);

			XmlNode nodeICMSTot = xmlDocument.CreateElement("ICMSTot");
			nodetotal.AppendChild(nodeICMSTot);

			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vBC", dTONfe.InfNFe.Total.ICMSTot.vBC);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vICMS", dTONfe.InfNFe.Total.ICMSTot.vICMS);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vICMSDeson", dTONfe.InfNFe.Total.ICMSTot.vICMSDeson);
			
            AdicionaNovoNode(xmlDocument, nodeICMSTot, "vFCP", dTONfe.InfNFe.Total.ICMSTot.vFCP); // v4.0

			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vBCST", dTONfe.InfNFe.Total.ICMSTot.vBCST);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vST", dTONfe.InfNFe.Total.ICMSTot.vST);

            AdicionaNovoNode(xmlDocument, nodeICMSTot, "vFCPST", dTONfe.InfNFe.Total.ICMSTot.vFCPST); // v4.0
            AdicionaNovoNode(xmlDocument, nodeICMSTot, "vFCPSTRet", dTONfe.InfNFe.Total.ICMSTot.vFCPSTRet); // v4.0

            AdicionaNovoNode(xmlDocument, nodeICMSTot, "vProd", dTONfe.InfNFe.Total.ICMSTot.vProd);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vFrete", dTONfe.InfNFe.Total.ICMSTot.vFrete);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vSeg", dTONfe.InfNFe.Total.ICMSTot.vSeg);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vDesc", dTONfe.InfNFe.Total.ICMSTot.vDesc);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vII", dTONfe.InfNFe.Total.ICMSTot.vII);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vIPI", dTONfe.InfNFe.Total.ICMSTot.vIPI);

            AdicionaNovoNode(xmlDocument, nodeICMSTot, "vIPIDevol", dTONfe.InfNFe.Total.ICMSTot.vIPIDevol); // v4.0

            AdicionaNovoNode(xmlDocument, nodeICMSTot, "vPIS", dTONfe.InfNFe.Total.ICMSTot.vPIS);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vCOFINS", dTONfe.InfNFe.Total.ICMSTot.vCOFINS);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vOutro", dTONfe.InfNFe.Total.ICMSTot.vOutro);
			AdicionaNovoNode(xmlDocument, nodeICMSTot, "vNF", dTONfe.InfNFe.Total.ICMSTot.vNF);
			if (!string.IsNullOrEmpty(dTONfe.InfNFe.Total.ICMSTot.vTotTrib))
				AdicionaNovoNode(xmlDocument, nodeICMSTot, "vTotTrib", dTONfe.InfNFe.Total.ICMSTot.vTotTrib);
			#endregion

			#region transp

			XmlNode nodetransp = xmlDocument.CreateElement("transp");
			nodeInfNFe.AppendChild(nodetransp);
			AdicionaNovoNode(xmlDocument, nodetransp, "modFrete",  dTONfe.InfNFe.Transp.modFrete);

			if (!string.IsNullOrEmpty(dTONfe.InfNFe.Transp.Transporta.xNome) )
			{
				XmlNode nodetransporta = xmlDocument.CreateElement("transporta");
				nodetransp.AppendChild(nodetransporta);
				if (dTONfe.InfNFe.Transp.Transporta.CNPJ != "")
					AdicionaNovoNode(xmlDocument, nodetransporta, "CNPJ", dTONfe.InfNFe.Transp.Transporta.CNPJ);
				else
					AdicionaNovoNode(xmlDocument, nodetransporta, "CPF", dTONfe.InfNFe.Transp.Transporta.CPF);

				AdicionaNovoNode(xmlDocument, nodetransporta, "xNome", dTONfe.InfNFe.Transp.Transporta.xNome);
				if (dTONfe.InfNFe.Transp.Transporta.IE == null)
					AdicionaNovoNode(xmlDocument, nodetransporta, "IE", "ISENTO");
				else
					AdicionaNovoNode(xmlDocument, nodetransporta, "IE", dTONfe.InfNFe.Transp.Transporta.IE);


				AdicionaNovoNode(xmlDocument, nodetransporta, "xEnder", dTONfe.InfNFe.Transp.Transporta.xEnder);
				AdicionaNovoNode(xmlDocument, nodetransporta, "xMun", dTONfe.InfNFe.Transp.Transporta.xMun);
				AdicionaNovoNode(xmlDocument, nodetransporta, "UF", dTONfe.InfNFe.Transp.Transporta.UF);
			}

			foreach (var item in dTONfe.InfNFe.Transp.Vol)
			{
				XmlNode nodevol = xmlDocument.CreateElement("vol");
				nodetransp.AppendChild(nodevol);
				AdicionaNovoNode(xmlDocument, nodevol, "qVol", item.qVol);
				AdicionaNovoNode(xmlDocument, nodevol, "esp", item.esp);
				AdicionaNovoNode(xmlDocument, nodevol, "marca", item.marca);
				AdicionaNovoNode(xmlDocument, nodevol, "nVol", item.nVol);
				AdicionaNovoNode(xmlDocument, nodevol, "pesoL", item.pesoL);
				AdicionaNovoNode(xmlDocument, nodevol, "pesoB", item.pesoB);

				foreach (var item2 in item.lacres)
				{
					XmlNode nodelacres = xmlDocument.CreateElement("lacres");
					nodevol.AppendChild(nodelacres);
					AdicionaNovoNode(xmlDocument, nodelacres, "nLacre", item2.nLacre);
				}
			}


			if (!string.IsNullOrEmpty(dTONfe.InfNFe.Transp.VeicTransp.placa))
			{
				XmlNode nodeveicTransp = xmlDocument.CreateElement("veicTransp");
				nodetransp.AppendChild(nodeveicTransp);
				AdicionaNovoNode(xmlDocument, nodeveicTransp, "placa", dTONfe.InfNFe.Transp.VeicTransp.placa);
				AdicionaNovoNode(xmlDocument, nodeveicTransp, "UF", dTONfe.InfNFe.Transp.VeicTransp.UF);
				AdicionaNovoNode(xmlDocument, nodeveicTransp, "RNTC", dTONfe.InfNFe.Transp.VeicTransp.RNTC);
			}


			if (dTONfe.InfNFe.Transp.RetTransp.vServ != null)
			{
				XmlNode noderetTransp = xmlDocument.CreateElement("retTransp");
				nodetransp.AppendChild(noderetTransp);


				AdicionaNovoNode(xmlDocument, noderetTransp, "vServ", dTONfe.InfNFe.Transp.RetTransp.vServ);
				AdicionaNovoNode(xmlDocument, noderetTransp, "vBCRet", dTONfe.InfNFe.Transp.RetTransp.vBCRet);
				AdicionaNovoNode(xmlDocument, noderetTransp, "pICMSRet", dTONfe.InfNFe.Transp.RetTransp.pICMSRet);
				AdicionaNovoNode(xmlDocument, noderetTransp, "vICMSRet", dTONfe.InfNFe.Transp.RetTransp.vICMSRet);
				AdicionaNovoNode(xmlDocument, noderetTransp, "CFOP", dTONfe.InfNFe.Transp.RetTransp.CFOP);
				AdicionaNovoNode(xmlDocument, noderetTransp, "cMunFG", dTONfe.InfNFe.Transp.RetTransp.cMunFG);

			}

			#endregion

			#region cobr
			if (!string.IsNullOrEmpty(dTONfe.InfNFe.Cobr.Fat.nFat))
			{
				XmlNode nodecobr = xmlDocument.CreateElement("cobr");
				nodeInfNFe.AppendChild(nodecobr);

				XmlNode nodefat = xmlDocument.CreateElement("fat");
				nodecobr.AppendChild(nodefat);

				AdicionaNovoNode(xmlDocument, nodefat, "nFat", dTONfe.InfNFe.Cobr.Fat.nFat);
				AdicionaNovoNode(xmlDocument, nodefat, "vOrig", dTONfe.InfNFe.Cobr.Fat.vOrig);
				AdicionaNovoNode(xmlDocument, nodefat, "vDesc", dTONfe.InfNFe.Cobr.Fat.vDesc);
				AdicionaNovoNode(xmlDocument, nodefat, "vLiq", dTONfe.InfNFe.Cobr.Fat.vLiq);

				foreach (var item in dTONfe.InfNFe.Cobr.Dup)
				{
					XmlNode nodedup = xmlDocument.CreateElement("dup");
					nodecobr.AppendChild(nodedup);

					AdicionaNovoNode(xmlDocument, nodedup, "nDup", item.nDup);
					AdicionaNovoNode(xmlDocument, nodedup, "dVenc", item.dVenc);
					AdicionaNovoNode(xmlDocument, nodedup, "vDup", item.vDup);
				}


			}
			#endregion

			#region infAdic
			XmlNode nodepag = xmlDocument.CreateElement("pag");
			nodeInfNFe.AppendChild(nodepag);

            XmlNode nodedetPag = xmlDocument.CreateElement("detPag");
            nodepag.AppendChild(nodedetPag);

            XmlNode nodecard = xmlDocument.CreateElement("card");
            //nodepag.AppendChild(nodecard);

            foreach (var item in dTONfe.InfNFe.Pagamento)
            {
                AdicionaNovoNode(xmlDocument, nodedetPag, "indPag", item.indPag.ToString());
                AdicionaNovoNode(xmlDocument, nodedetPag, "tPag", item.tPag);
                AdicionaNovoNode(xmlDocument, nodedetPag, "vPag", item.vPag);
                if (item.tpIntegra != null)
                    AdicionaNovoNode(xmlDocument, nodecard, "tpIntegra", item.tpIntegra.ToString());
                if (item.CNPJ != null)
                    AdicionaNovoNode(xmlDocument, nodecard, "CNPJ", item.CNPJ);

                if (item.tBand != null)
                    AdicionaNovoNode(xmlDocument, nodecard, "tBand", item.tBand);

                if (item.cAut != null)
                    AdicionaNovoNode(xmlDocument, nodecard, "cAut", item.cAut.ToString());

                if (item.vTroco != null)
                    AdicionaNovoNode(xmlDocument, nodecard, "vTroco", item.vTroco);

            }



            #endregion

            #region infAdic
            XmlNode nodeinfAdic = xmlDocument.CreateElement("infAdic");
            nodeInfNFe.AppendChild(nodeinfAdic);
            AdicionaNovoNode(xmlDocument, nodeinfAdic, "infAdFisco", dTONfe.InfNFe.InfAdic.infAdFisco);
            AdicionaNovoNode(xmlDocument, nodeinfAdic, "infCpl", dTONfe.InfNFe.InfAdic.infCpl);
            #endregion
            return xmlDocument;
		}
		public void AdicionaNovoNode(XmlDocument xmlDocument, XmlNode nodeDestino, string nomeNovoNode, string valorNovoNode)
		{
			var novoNode = xmlDocument.CreateElement(nomeNovoNode);
			novoNode.InnerText = valorNovoNode;
			nodeDestino.AppendChild(novoNode);
		}

		public void AdicionaNovoAtributo(XmlDocument xmlDocument, XmlNode nodeDestino, string nomeNovoAtributo, string valorNovoAtributo)
		{
			var attribute = xmlDocument.CreateAttribute(nomeNovoAtributo);
			attribute.Value = valorNovoAtributo;
			nodeDestino.Attributes.Append(attribute);
		}
	}
}

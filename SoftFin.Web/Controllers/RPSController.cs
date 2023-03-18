using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using SoftFin.Web.Negocios;
using SoftFin.NFSe.DTO;
using SoftFin.NFSe.SaoPaulo.InterfaceService;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace SoftFin.Web.Controllers
{
	public class RPSXMLController : BaseController
	{
		public ActionResult Index()
		{
			@ViewData["DataInicial"] = DateTime.Now.AddDays(-30).Date.ToString("dd/MM/yyyy");
			@ViewData["DataFinal"] = DateTime.Now.Date.ToString("dd/MM/yyyy");
            ViewData["xml"] = null;
			return View();
		}


		[HttpPost]
		public ActionResult Index(string dataInicial, string dataFinal)
		{
            try
            {
                @ViewData["DataInicial"] = dataInicial;
                @ViewData["DataFinal"] = dataFinal;

                var dadosEstab = new Estabelecimento().ObterPorId(_estab, _paramBase);
                string nomeArquivo = "CNPJ_" + dadosEstab.CNPJ.Replace("/", "").Replace(".", "") + "_Data_" + DateTime.Now.ToString("yyyyMMdd") + "_Envio.xml";

                var arquivo = GeraArquivoLote(dataInicial, dataFinal, nomeArquivo, dadosEstab);
                return View();

            }
            catch(Exception ex)
            {
                @ViewBag.msg = ex.Message;
                return View();
            }
		}

        private bool ValidarXSD(string arquivo)
        {
            var errors = false;
            var errorMensage = "";

            using (StreamReader oReader = new StreamReader(arquivo, Encoding.GetEncoding("ISO-8859-1")))
            {
                XDocument docXML = XDocument.Load(oReader);

                var filePath = Server.MapPath("/XSDPREFSP");
                System.Xml.Schema.XmlSchemaSet schemaXsd = new System.Xml.Schema.XmlSchemaSet();
                schemaXsd.Add(null, filePath + @"\PedidoEnvioLoteRPS_v01.xsd");
                schemaXsd.Add(null, filePath + @"\TiposNFe_v01.xsd");
                schemaXsd.Add(null, filePath + @"\xmldsig-core-schema_v01.xsd");
                schemaXsd.Compile();
                
                docXML.Validate(schemaXsd, (o, e) =>
                {
                    errorMensage = e.Message;
                    errors = true;
                }
                );
            }



            return !errors;
        }

        private string GeraArquivoLote(string dataInicial, string dataFinal, string nomearquivo, Estabelecimento dadosEstab)
        {
            List<int> nfs = new List<int>();
            List<NotaFiscal> listaAux = new List<NotaFiscal>();
            var FormatoData = "yyyy-MM-dd";
            decimal valorTotalDeducoes = 0;
            decimal valorTotalServicos = 0;

            FiltraNotas(nfs, listaAux);



            var uploadPath = Server.MapPath("~/CertificadoRPSTMP/");
            Directory.CreateDirectory(uploadPath);

            var nomearquivonovo = Guid.NewGuid().ToString();

            string caminhoArquivo = Path.Combine(@uploadPath, nomearquivonovo);

 
            //AzureStorage.DownloadFile(caminhoArquivo,
            //            "Certificados/" + dadosEstab.id.ToString() + "/cert.pfx",
            //            ConfigurationManager.AppSettings["StorageAtendimento"].ToString());



            X509Certificate2 cert = new X509Certificate2();
            Certificado certificado = new Certificado();
            cert = certificado.BuscaCerttificado(dadosEstab.id, dadosEstab.senhaCertificado, caminhoArquivo);

            XmlDocument nfes = new XmlDocument();
            XmlDeclaration xml_declaration = nfes.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement document_element = nfes.DocumentElement;
            nfes.InsertBefore(xml_declaration, document_element);
            XmlNode nodePedidoEnvioLoteRPS = nfes.CreateElement("PedidoEnvioLoteRPS");

            XmlAttribute attribute = nfes.CreateAttribute("xmlns:xsi");
            attribute.Value = "http://www.w3.org/2001/XMLSchema-instance";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute);

            XmlAttribute attribute2 = nfes.CreateAttribute("xmlns:xsd");
            attribute2.Value = "http://www.w3.org/2001/XMLSchema";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute2);

            XmlAttribute attribute3 = nfes.CreateAttribute("xmlns");
            attribute3.Value = "http://www.prefeitura.sp.gov.br/nfe";
            nodePedidoEnvioLoteRPS.Attributes.Append(attribute3);

            var nodeCabecalho = nfes.CreateElement("Cabecalho");
            nodePedidoEnvioLoteRPS.AppendChild(nodeCabecalho);
            nfes.AppendChild(nodePedidoEnvioLoteRPS);
            foreach (var itemNF in listaAux)
            {
                XmlNode nodeRPS = nfes.CreateElement("RPS");
                XmlAttribute RPSxmlnsattribute = nfes.CreateAttribute("xmlns");
                RPSxmlnsattribute.Value = "";
                nodeRPS.Attributes.Append(RPSxmlnsattribute);

                var nodeAssinatura = nfes.CreateElement("Assinatura");
                nodeRPS.AppendChild(nodeAssinatura);

                var nodeChaveRPS = nfes.CreateElement("ChaveRPS");

                var nodeInscricaoPrestador = nfes.CreateElement("InscricaoPrestador");
                nodeInscricaoPrestador.InnerText = dadosEstab.InscricaoMunicipal.ToString();
                nodeChaveRPS.AppendChild(nodeInscricaoPrestador);

                var nodeSerieRPS = nfes.CreateElement("SerieRPS");
                nodeSerieRPS.InnerText = itemNF.serieRps.ToString();
                nodeChaveRPS.AppendChild(nodeSerieRPS);
                
                var nodeNumeroRPS = nfes.CreateElement("NumeroRPS");
                nodeNumeroRPS.InnerText = itemNF.numeroRps.ToString();
                nodeChaveRPS.AppendChild(nodeNumeroRPS);

                nodeRPS.AppendChild(nodeChaveRPS);

                PreecheNode(nfes, nodeRPS, "TipoRPS", itemNF.Operacao.tipoRPS.codigo);
                PreecheNode(nfes, nodeRPS, "DataEmissao", itemNF.dataEmissaoNfse.ToString(FormatoData));
                PreecheNode(nfes, nodeRPS, "StatusRPS", "N");
                PreecheNode(nfes, nodeRPS, "TributacaoRPS", itemNF.Operacao.situacaoTributariaNota.codigo);
                PreecheNode(nfes, nodeRPS, "ValorServicos", FormataNumero(itemNF.valorNfse));
                PreecheNode(nfes, nodeRPS, "ValorDeducoes", FormataNumero(itemNF.valorDeducoes));
                PreecheNode(nfes, nodeRPS, "ValorPIS", FormataNumero(itemNF.pisRetido));
                PreecheNode(nfes, nodeRPS, "ValorCOFINS", FormataNumero(itemNF.cofinsRetida));
                PreecheNode(nfes, nodeRPS, "ValorINSS", "0.00");
                PreecheNode(nfes, nodeRPS, "ValorIR", FormataNumero(itemNF.irrf));
                PreecheNode(nfes, nodeRPS, "ValorCSLL", FormataNumero(itemNF.csllRetida));

                PreecheNode(nfes, nodeRPS, "CodigoServico", itemNF.codigoServico);
                PreecheNode(nfes, nodeRPS, "AliquotaServicos", FormataNumero(itemNF.aliquotaISS));

                PreecheNode(nfes, nodeRPS, "ISSRetido", "false");


                
                
                
                var nodeCPFCNPJTomador = nfes.CreateElement("CPFCNPJTomador");
                PreecheNode(nfes, nodeCPFCNPJTomador, "CNPJ", itemNF.NotaFiscalPessoaTomador.cnpjCpf.Replace("/", "").Replace(".", "").Replace("-", ""));
                nodeRPS.AppendChild(nodeCPFCNPJTomador);

                PreecheNode(nfes, nodeRPS, "RazaoSocialTomador", itemNF.NotaFiscalPessoaTomador.razao);

                var nodeEnderecoTomador = nfes.CreateElement("EnderecoTomador");
                PreecheNode(nfes, nodeEnderecoTomador, "TipoLogradouro", itemNF.NotaFiscalPessoaTomador.tipoEndereco);
                PreecheNode(nfes, nodeEnderecoTomador, "Logradouro", itemNF.NotaFiscalPessoaTomador.endereco);
                PreecheNode(nfes, nodeEnderecoTomador, "NumeroEndereco", itemNF.NotaFiscalPessoaTomador.numero);
                PreecheNode(nfes, nodeEnderecoTomador, "ComplementoEndereco", itemNF.NotaFiscalPessoaTomador.complemento);
                PreecheNode(nfes, nodeEnderecoTomador, "Bairro", itemNF.NotaFiscalPessoaTomador.bairro);
                PreecheNode(nfes, nodeEnderecoTomador, "Cidade", "3550308");
                PreecheNode(nfes, nodeEnderecoTomador, "UF", itemNF.NotaFiscalPessoaTomador.uf);
                PreecheNode(nfes, nodeEnderecoTomador, "CEP", itemNF.NotaFiscalPessoaTomador.cep.Replace("-", ""));
                nodeRPS.AppendChild(nodeEnderecoTomador);

                PreecheNode(nfes, nodeRPS, "EmailTomador", itemNF.NotaFiscalPessoaTomador.email);
                PreecheNode(nfes, nodeRPS, "Discriminacao", itemNF.discriminacaoServico.Replace("\n", "|").Replace("\r", ""));
                


                nodePedidoEnvioLoteRPS.AppendChild(nodeRPS);
                StringBuilder hashAssinatura = new StringBuilder();

                hashAssinatura.Append(dadosEstab.InscricaoMunicipal.ToString().PadRight(8, '0')); //1- Inscrição Municipal do Prestador
                hashAssinatura.Append((itemNF.serieRps.PadRight(5, ' '))); // 2= Série do RPS
                hashAssinatura.Append(itemNF.numeroRps.ToString().PadLeft(12,'0')); //3- Número do RPS
                hashAssinatura.Append(itemNF.dataEmissaoRps.ToString("yyyyMMdd")); //4 - Data de Emissão do RPS
                hashAssinatura.Append(itemNF.Operacao.situacaoTributariaNota.codigo);//5 - Tipo de Tributação do RPS
                hashAssinatura.Append("N"); //6 - Status do RPS
                hashAssinatura.Append("N"); //7 - ISS Retido
                hashAssinatura.Append(FormataNumero2(itemNF.valorNfse).PadLeft(15, '0'));//8 - Valor dos Serviços
                hashAssinatura.Append(FormataNumero2(itemNF.valorDeducoes).PadLeft(15, '0'));//9 - Valor das Deduções
                hashAssinatura.Append((itemNF.codigoServico + "000000000000").Substring(0, 5));//10 - Código do Serviço Prestado 
                hashAssinatura.Append("2");//11 - Indicador de CPF/CNPJ do Tomador 
                hashAssinatura.Append(itemNF.NotaFiscalPessoaTomador.cnpjCpf.Replace("/", "").Replace(".", "").Replace("-", ""));//12 - CPF/CNPJ do Tomador  
                //hashAssinatura.Append("2");//13 - Indicador de CPF/CNPJ do Intermediário 
                //hashAssinatura.Append(dadosEstab.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""));//14 - CPF/CNPJ do Intermediário CPF 
                //hashAssinatura.Append("N");//15 - SS Retido Intermediário 

                var assinatura = new Assinador().AssinarRPSSP(cert, hashAssinatura.ToString());

                nodeAssinatura.InnerText = assinatura;


                valorTotalDeducoes = itemNF.valorDeducoes;
                valorTotalServicos = itemNF.valorNfse;

                nodePedidoEnvioLoteRPS.AppendChild(nodeRPS);
            }

            //PedidoEnvioLoteRPS.xsd
            
            var nodeCPFCNPJRemetente = nfes.CreateElement("CPFCNPJRemetente");
            PreecheNode(nfes, nodeCPFCNPJRemetente, "CNPJ", dadosEstab.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""));
            nodeCabecalho.AppendChild(nodeCPFCNPJRemetente);
            PreecheNode(nfes, nodeCabecalho, "transacao", "false");
            PreecheNode(nfes, nodeCabecalho, "dtInicio", DateTime.Parse(dataInicial).ToString(FormatoData));
            PreecheNode(nfes, nodeCabecalho, "dtFim", DateTime.Parse(dataFinal).ToString(FormatoData));
            PreecheNode(nfes, nodeCabecalho, "QtdRPS", listaAux.Count().ToString());
            PreecheNode(nfes, nodeCabecalho, "ValorTotalServicos", FormataNumero(valorTotalServicos));
            PreecheNode(nfes, nodeCabecalho, "ValorTotalDeducoes", FormataNumero(valorTotalDeducoes));
            PreecheAtributo(nfes, nodeCabecalho, "Versao", "1");
            PreecheAtributo(nfes, nodeCabecalho, "xmlns", "");


            String filePath;
            filePath = Server.MapPath("/XMLDOCS");
            Directory.CreateDirectory(filePath);


            string filePathComplete = filePath + "\\" + nomearquivo;

            XmlDocument retornoAssinado = new AssinaturaDigital().AplicaAssinatura(
                    nfes, "PedidoEnvioLoteRPS", cert); 


            //retornoAssinado.DocumentElement.SetAttribute("xmlns", "http://www.prefeitura.sp.gov.br/nfe");
            retornoAssinado.Save(filePathComplete);
            
            using (var streamReader = new StreamReader(filePathComplete, Encoding.UTF8))
            {
                //var nfes = streamReader.ReadToEnd().Replace("\r", "").Replace("\n", "");
                    
                var x = new LoteNFe();
                x.ClientCertificates.Add(cert);

                var y = x.TesteEnvioLoteRPS(1, retornoAssinado.OuterXml);
                //throw new Exception(y.ToString());



                var xmlDocument = new XmlDocument();

                xmlDocument.Load(GenerateStreamFromString(y));
                XmlNodeList procEventoNFe = xmlDocument.GetElementsByTagName("RetornoEnvioLoteRPS");
                XmlNodeList cabecalho = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Cabecalho");
                XmlNodeList erros = ((XmlElement)procEventoNFe[0]).GetElementsByTagName("Erro");

                var resultingMessage = new DTORetornoNFEs();

                resultingMessage.Cabecalho.Sucesso = cabecalho[0].ChildNodes[0].InnerText;

                foreach (XmlElement item in erros)
                {

                    resultingMessage.Erro.Add(new TPErro
                    {
                        Codigo = item.ChildNodes[0].InnerText,
                        Descricao= item.ChildNodes[1].InnerText
                    });
                }

                ViewData["xml"] = resultingMessage;
            }
           
            return filePathComplete;
        }
        public string FormataNumero(decimal valor)
        {
            return valor.ToString("n").Replace(".", "").Replace(",", ".");
        }
        public string FormataNumero2(decimal valor)
        {
            return valor.ToString("n").Replace(".", "").Replace(",", "");
        }
        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        private static void PreecheNode(XmlDocument xmlDocument, XmlNode nodeRPS, string nameElement, string value)
        {
            var node = xmlDocument.CreateElement(nameElement);
            node.InnerText = value;
            nodeRPS.AppendChild(node);
        }
        private static void PreecheAtributo(XmlDocument xmlDocument, XmlNode nodeRPS, string nameElement, string value)
        {
            var attribute = xmlDocument.CreateAttribute(nameElement);
            attribute.Value = value;
            nodeRPS.Attributes.Append(attribute);
        }


		private void FiltraNotas(List<int> nfs, List<NotaFiscal> listaAux)
		{
			int estab = _paramBase.estab_id;
			var db = new DbControle();
			foreach (var item in Request.Form)
			{
				if (item.ToString().Substring(0, 2) == "nf")
				{
					nfs.Add(int.Parse(item.ToString().Substring(2)));
				}
			}
			foreach (var item in nfs)
			{
				var nfe = db.NotaFiscal.Where(nf => nf.id == item && nf.estabelecimento_id == estab).FirstOrDefault();
				if (nfe != null)
					listaAux.Add(nfe);
			}


		}
		[HttpPost]
		public JsonResult ConsultaNota(string dataInicial, string dataFinal)
		{

			DateTime DataInicial = new DateTime();
			DataInicial = DateTime.Parse(dataInicial);


			DateTime DataFinal = new DateTime();
			DataFinal = DateTime.Parse(dataFinal);



			var nfs = new NotaFiscal().ObterTodos(DataInicial, DataFinal, _paramBase).Where(p => p.situacaoPrefeitura_id == Models.NotaFiscal.RPS_NF_EMITIDANAOENVIADA
										|| p.situacaoPrefeitura_id == Models.NotaFiscal.NFCANCELADAEMCONF);
			List<RetornoNota> rets = new List<RetornoNota>();

			foreach (var item in nfs)
			{
				rets.Add(new RetornoNota
				{
                    Cliente = item.NotaFiscalPessoaTomador.razao,
					id = item.id,
					RPS = item.numeroRps.ToString(),
					Valor = item.valorNfse.ToString("0.00"),
					Data = item.dataEmissaoRps.ToString("dd/MM/yyyy"),
					Situacao = Models.NotaFiscal.CarregaSituacao(item.situacaoPrefeitura_id)
				});
			}

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(rets);

			return Json(rets, JsonRequestBehavior.AllowGet);
		}


        public class Assinador
        {

            public string AssinarRPSSP(X509Certificate2 cert, string original)
            {

                //recebe o certificado e a string a ser assinada 
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                //pega a chave privada do certificado digital 
                rsa = cert.PrivateKey as RSACryptoServiceProvider;
                //cria o array de bytes e realiza a conversao da string em array de bytes 
                byte[] sAssinaturaByte = enc.GetBytes(original);

                RSAPKCS1SignatureFormatter rsaf = new RSAPKCS1SignatureFormatter(rsa);
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

                //cria a variavel hash que armazena o resultado do sha1 
                byte[] hash;
                hash = sha1.ComputeHash(sAssinaturaByte);

                //definimos o metodo a ser utilizado na criptografia e assinamos 
                rsaf.SetHashAlgorithm("SHA1");
                sAssinaturaByte = rsaf.CreateSignature(hash);

                //por fim fazemos a conversao do array de bytes para string 
                var criptografada = Convert.ToBase64String(sAssinaturaByte);
                return criptografada;
            }

        }
        public class AssinaturaDigital
        {
            public XmlDocument AplicaAssinatura(XmlDocument docXML, string uri, X509Certificate2 cert)
            {
                try
                {
                    // Obtem o certificado
                    X509Certificate2 X509Cert = cert;
                    // Cria um documento XML para carregar o XML
                    //XmlDocument docXML = new XmlDocument();
                    //docXML.PreserveWhitespace = true;
                    //xml = xml.Replace("\r", "").Replace("\n", "");
                    // Carrega o documento XML
                    //docXML.LoadXml(xml);


                    // Cria o objeto XML assinado
                    SignedXml signedXml = new SignedXml(docXML);
                    // Assina com a chave privada
                    signedXml.SigningKey = X509Cert.PrivateKey;
                    // Atribui o método de canonização
                    signedXml.SignedInfo.CanonicalizationMethod = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
                    // Atribui o método para assinatura
                    signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
                    // Cria a referencia
                    Reference reference = new Reference("");
                    // Pega a URI para ser assinada
                    XmlAttributeCollection _Uri = docXML.GetElementsByTagName(uri).Item(0).Attributes;
                    foreach (XmlAttribute _atributo in _Uri)
                    {
                        if (_atributo.Name == "Id")
                            reference.Uri = "#" + _atributo.InnerText;
                    }
                    // Adiciona o envelope à referência
                    XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                    reference.AddTransform(env);
                    // Atribui o método do Hash
                    reference.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
                    // Adiciona a referencia ao XML assinado
                    signedXml.AddReference(reference);
                    // Cria o objeto keyInfo
                    KeyInfo keyInfo = new KeyInfo();
                    // Carrega a informação da KeyInfo
                    KeyInfoClause rsaKeyVal = new RSAKeyValue((RSA)X509Cert.PrivateKey);
                    KeyInfoX509Data x509Data = new KeyInfoX509Data(X509Cert);
                    //x509Data.AddSubjectName(X509Cert.SubjectName.Name.ToString());
                    keyInfo.AddClause(x509Data);
                    //keyInfo.AddClause(rsaKeyVal);
                    // Adiciona a KeyInfo
                    signedXml.KeyInfo = keyInfo;
                    // Atribui uma ID à assinatura
                    //signedXml.Signature.Id = "#" + uri;
                    // Efetiva a assinatura
                    signedXml.ComputeSignature();
                    bool signed = signedXml.CheckSignature(cert, true);
                    // Obtem o XML assinado
                    XmlElement xmlDigitalSignature = signedXml.GetXml();
                    // Adiciona o elemento assinado ao XML
                    docXML.DocumentElement.AppendChild(docXML.ImportNode(xmlDigitalSignature, true));

                    // Retorna o XML
                    return docXML;
                }
                catch (Exception erro) { throw erro; }
            }
        }
        public class Certificado
        {
            public X509Certificate2 BuscaCerttificado(int estab,string senha, string localCertificadoTMP)
            {
                
                AzureStorage.DownloadFile(localCertificadoTMP, "Certificados/" + estab + "/cert.pfx",
                        ConfigurationManager.AppSettings["StorageAtendimento"].ToString());


                // Create a collection object and populate it using the PFX file
                X509Certificate2Collection collection = new X509Certificate2Collection();
                collection.Import(localCertificadoTMP, senha, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                return collection[3];


            }
        }

	}
}

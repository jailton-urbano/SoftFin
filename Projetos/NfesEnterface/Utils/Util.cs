using SoftFin.Utils;
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

namespace SoftFin.NFSe.SaoPaulo.Utils
{
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
        public X509Certificate2 BuscaCerttificado(int estab, string senha, string localCertificadoTMP)
        {

            var cert = AzureStorage.DownloadFile("Certificados/" + estab + "/cert.pfx",
                    ConfigurationManager.AppSettings["StorageAtendimento"].ToString());


            // Create a collection object and populate it using the PFX file
            X509Certificate2Collection collection = new X509Certificate2Collection();
            collection.Import(localCertificadoTMP, senha, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            return collection[3];


        }
    }
    public static class Util
    {
        public static void AdicionaNovoNode(XmlDocument xmlDocument, XmlNode nodeDestino, string nomeNovoNode, string valorNovoNode)
        {
            var novoNode = xmlDocument.CreateElement(nomeNovoNode);
            novoNode.InnerText = valorNovoNode;
            nodeDestino.AppendChild(novoNode);
        }

        public static void AdicionaNovoAtributo(XmlDocument xmlDocument, XmlNode nodeDestino, string nomeNovoAtributo, string valorNovoAtributo)
        {
            var attribute = xmlDocument.CreateAttribute(nomeNovoAtributo);
            attribute.Value = valorNovoAtributo;
            nodeDestino.Attributes.Append(attribute);
        }
    }
}

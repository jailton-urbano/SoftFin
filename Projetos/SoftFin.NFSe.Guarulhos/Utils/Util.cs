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

namespace SoftFin.NFSe.Guarulhos.Utils
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
        public XmlDocument AplicaAssinatura(XmlDocument docXML, string grupo , string uri , X509Certificate2 cert)
        {
            try
            {
                var docRequest  = docXML;

                XmlNodeList ListInfNFe = docRequest.GetElementsByTagName(grupo);

                foreach (XmlElement infNFe in ListInfNFe)
 
                {
 
                     string id = infNFe.Attributes.GetNamedItem("Id").InnerText;
                     var signedXml = new SignedXml(infNFe);
                     signedXml.SigningKey = cert.PrivateKey;
 
                     Reference reference = new Reference("#" + id);
                     reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                     reference.AddTransform(new XmlDsigC14NTransform());
                     signedXml.AddReference(reference);
 
                     KeyInfo keyInfo = new KeyInfo();
                     keyInfo.AddClause(new KeyInfoX509Data(cert));
 
                     signedXml.KeyInfo = keyInfo;
 
                     signedXml.ComputeSignature();
 
                     XmlElement xmlSignature = docRequest.CreateElement("dsig","Signature", "http://www.w3.org/2000/09/xmldsig#");
                     XmlElement xmlSignedInfo = signedXml.SignedInfo.GetXml();
                     XmlElement xmlKeyInfo = signedXml.KeyInfo.GetXml();

                     XmlElement xmlSignatureValue = docRequest.CreateElement("dsig", "SignatureValue", xmlSignature.NamespaceURI);
                     string signBase64 = Convert.ToBase64String(signedXml.Signature.SignatureValue);
                     XmlText text = docRequest.CreateTextNode(signBase64);
                     xmlSignatureValue.AppendChild(text);
 
                     xmlSignature.AppendChild(docRequest.ImportNode(xmlSignedInfo, true));
                     xmlSignature.AppendChild(xmlSignatureValue);
                     xmlSignature.AppendChild(docRequest.ImportNode(xmlKeyInfo, true));

                     var evento = docRequest.GetElementsByTagName(uri);
                     evento[0].AppendChild(xmlSignature);
                     //infNFe.AppendChild(xmlSignature);
                }

                return docRequest;
            }
            catch (Exception erro) { throw erro; }
        }


        public XmlDocument AplicaAssinaturaNFe(XmlDocument docXML, string grupo, string uri, X509Certificate2 cert)
        {
            try
            {
                var docRequest = docXML;

                XmlNodeList ListInfNFe = docRequest.GetElementsByTagName(grupo);

                foreach (XmlElement infNFe in ListInfNFe)
                {

                    string id = infNFe.Attributes.GetNamedItem("Id").InnerText;
                    var signedXml = new SignedXml(infNFe);
                    signedXml.SigningKey = cert.PrivateKey;

                    Reference reference = new Reference("#" + id);
                    reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                    reference.AddTransform(new XmlDsigC14NTransform());
                    signedXml.AddReference(reference);

                    KeyInfo keyInfo = new KeyInfo();
                    keyInfo.AddClause(new KeyInfoX509Data(cert));

                    signedXml.KeyInfo = keyInfo;

                    signedXml.ComputeSignature();

                    XmlElement xmlSignature = docRequest.CreateElement("dsig", "Signature", "http://www.w3.org/2000/09/xmldsig#");
                    XmlElement xmlSignedInfo = signedXml.SignedInfo.GetXml();
                    XmlElement xmlKeyInfo = signedXml.KeyInfo.GetXml();

                    XmlElement xmlSignatureValue = docRequest.CreateElement("dsig", "SignatureValue", xmlSignature.NamespaceURI);
                    string signBase64 = Convert.ToBase64String(signedXml.Signature.SignatureValue);
                    XmlText text = docRequest.CreateTextNode(signBase64);
                    xmlSignatureValue.AppendChild(text);

                    xmlSignature.AppendChild(docRequest.ImportNode(xmlSignedInfo, true));
                    xmlSignature.AppendChild(xmlSignatureValue);
                    xmlSignature.AppendChild(docRequest.ImportNode(xmlKeyInfo, true));

                    var evento = docRequest.GetElementsByTagName(uri);
                    evento[0].AppendChild(xmlSignature);
                    //infNFe.AppendChild(xmlSignature);
                }

                return docRequest;
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

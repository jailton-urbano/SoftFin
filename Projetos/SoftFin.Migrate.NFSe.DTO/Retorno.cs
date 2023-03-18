using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Migrate.NFSe.DTO.RetornoComum
{


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class Envelope
    {

        private EnvelopeBody bodyField;

        /// <remarks/>
        public EnvelopeBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }

        public string xml { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public partial class EnvelopeBody
    {

        private recepcaoExecuteResponse recepcaoExecuteResponseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("recepcao.ExecuteResponse", Namespace = "InvoiCy")]
        public recepcaoExecuteResponse recepcaoExecuteResponse
        {
            get
            {
                return this.recepcaoExecuteResponseField;
            }
            set
            {
                this.recepcaoExecuteResponseField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "InvoiCy")]
    [System.Xml.Serialization.XmlRootAttribute("recepcao.ExecuteResponse", Namespace = "InvoiCy", IsNullable = false)]
    public partial class recepcaoExecuteResponse
    {

        private recepcaoExecuteResponseInvoicyretorno invoicyretornoField;

        /// <remarks/>
        public recepcaoExecuteResponseInvoicyretorno Invoicyretorno
        {
            get
            {
                return this.invoicyretornoField;
            }
            set
            {
                this.invoicyretornoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "InvoiCy")]
    public partial class recepcaoExecuteResponseInvoicyretorno
    {

        private recepcaoExecuteResponseInvoicyretornoMensagem mensagemField;

        /// <remarks/>
        public recepcaoExecuteResponseInvoicyretornoMensagem Mensagem
        {
            get
            {
                return this.mensagemField;
            }
            set
            {
                this.mensagemField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "InvoiCy")]
    public partial class recepcaoExecuteResponseInvoicyretornoMensagem
    {

        private recepcaoExecuteResponseInvoicyretornoMensagemMensagemItem mensagemItemField;

        /// <remarks/>
        public recepcaoExecuteResponseInvoicyretornoMensagemMensagemItem MensagemItem
        {
            get
            {
                return this.mensagemItemField;
            }
            set
            {
                this.mensagemItemField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "InvoiCy")]
    public partial class recepcaoExecuteResponseInvoicyretornoMensagemMensagemItem
    {

        private string codigoField;

        private string descricaoField;

        private recepcaoExecuteResponseInvoicyretornoMensagemMensagemItemDocumentos documentosField;

        /// <remarks/>
        public string Codigo
        {
            get
            {
                return this.codigoField;
            }
            set
            {
                this.codigoField = value;
            }
        }

        /// <remarks/>
        public string Descricao
        {
            get
            {
                return this.descricaoField;
            }
            set
            {
                this.descricaoField = value;
            }
        }

        /// <remarks/>
        public recepcaoExecuteResponseInvoicyretornoMensagemMensagemItemDocumentos Documentos
        {
            get
            {
                return this.documentosField;
            }
            set
            {
                this.documentosField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "InvoiCy")]
    public partial class recepcaoExecuteResponseInvoicyretornoMensagemMensagemItemDocumentos
    {

        private recepcaoExecuteResponseInvoicyretornoMensagemMensagemItemDocumentosDocumentosItem documentosItemField;

        /// <remarks/>
        public recepcaoExecuteResponseInvoicyretornoMensagemMensagemItemDocumentosDocumentosItem DocumentosItem
        {
            get
            {
                return this.documentosItemField;
            }
            set
            {
                this.documentosItemField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "InvoiCy")]
    public partial class recepcaoExecuteResponseInvoicyretornoMensagemMensagemItemDocumentosDocumentosItem
    {

        private string documentoField;

        /// <remarks/>
        public string Documento
        {
            get
            {
                return this.documentoField;
            }
            set
            {
                this.documentoField = value;
            }
        }
    }


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Documento
    {

        private string docModeloField;

        private string docNumeroField;

        private string docSerieField;

        private string docProtocoloField;

        private string docPDFBase64Field;

        private string docPDFDownloadField;

        private string docXMLBase64Field;

        private string docXMLDownloadField;

        private string docArquivoField;

        private string docExtensaoArquivoField;

        private DocumentoNFSe nFSeField;

        private DocumentoSituacao situacaoField;

        /// <remarks/>
        public string DocModelo
        {
            get
            {
                return this.docModeloField;
            }
            set
            {
                this.docModeloField = value;
            }
        }

        /// <remarks/>
        public string DocNumero
        {
            get
            {
                return this.docNumeroField;
            }
            set
            {
                this.docNumeroField = value;
            }
        }

        /// <remarks/>
        public string DocSerie
        {
            get
            {
                return this.docSerieField;
            }
            set
            {
                this.docSerieField = value;
            }
        }

        /// <remarks/>
        public string DocProtocolo
        {
            get
            {
                return this.docProtocoloField;
            }
            set
            {
                this.docProtocoloField = value;
            }
        }

        /// <remarks/>
        public string DocPDFBase64
        {
            get
            {
                return this.docPDFBase64Field;
            }
            set
            {
                this.docPDFBase64Field = value;
            }
        }

        /// <remarks/>
        public string DocPDFDownload
        {
            get
            {
                return this.docPDFDownloadField;
            }
            set
            {
                this.docPDFDownloadField = value;
            }
        }

        /// <remarks/>
        public string DocXMLBase64
        {
            get
            {
                return this.docXMLBase64Field;
            }
            set
            {
                this.docXMLBase64Field = value;
            }
        }

        /// <remarks/>
        public string DocXMLDownload
        {
            get
            {
                return this.docXMLDownloadField;
            }
            set
            {
                this.docXMLDownloadField = value;
            }
        }

        /// <remarks/>
        public string DocArquivo
        {
            get
            {
                return this.docArquivoField;
            }
            set
            {
                this.docArquivoField = value;
            }
        }

        /// <remarks/>
        public string DocExtensaoArquivo
        {
            get
            {
                return this.docExtensaoArquivoField;
            }
            set
            {
                this.docExtensaoArquivoField = value;
            }
        }

        /// <remarks/>
        public DocumentoNFSe NFSe
        {
            get
            {
                return this.nFSeField;
            }
            set
            {
                this.nFSeField = value;
            }
        }

        /// <remarks/>
        public DocumentoSituacao Situacao
        {
            get
            {
                return this.situacaoField;
            }
            set
            {
                this.situacaoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentoNFSe
    {

        private string nFSeNumeroField;

        private string nFSeCodVerificacaoField;

        /// <remarks/>
        public string NFSeNumero
        {
            get
            {
                return this.nFSeNumeroField;
            }
            set
            {
                this.nFSeNumeroField = value;
            }
        }

        /// <remarks/>
        public string NFSeCodVerificacao
        {
            get
            {
                return this.nFSeCodVerificacaoField;
            }
            set
            {
                this.nFSeCodVerificacaoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentoSituacao
    {

        private string sitCodigoField;

        private string sitDescricaoField;

        /// <remarks/>
        public string SitCodigo
        {
            get
            {
                return this.sitCodigoField;
            }
            set
            {
                this.sitCodigoField = value;
            }
        }

        /// <remarks/>
        public string SitDescricao
        {
            get
            {
                return this.sitDescricaoField;
            }
            set
            {
                this.sitDescricaoField = value;
            }
        }
    }



}

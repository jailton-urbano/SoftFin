using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Migrate.NFSe.DTO.RetornoConsulta
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



}

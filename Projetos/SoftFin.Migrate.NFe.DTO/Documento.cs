using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Migrate.NFe.DTO
{

    // OBSERVAÇÃO: o código gerado pode exigir pelo menos .NET Framework 4.5 ou .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Documento
    {

        private string docModeloField;

        private ushort docNumeroField;

        private ushort docSerieField;

        private string docChaAcessoField;

        private ulong docProtocoloField;

        private byte docEvenSeqField;

        private byte docEveTpField;

        private object docEveIdField;

        private object docPDFBase64Field;

        private object docPDFDownloadField;

        private System.DateTime docDhAutField;

        private string docDigestValueField;

        private object docXMLBase64Field;

        private object docXMLDownloadField;

        private object docImpressoraField;

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
        public ushort DocNumero
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
        public ushort DocSerie
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
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer")]
        public string DocChaAcesso
        {
            get
            {
                return this.docChaAcessoField;
            }
            set
            {
                this.docChaAcessoField = value;
            }
        }

        /// <remarks/>
        public ulong DocProtocolo
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
        public byte DocEvenSeq
        {
            get
            {
                return this.docEvenSeqField;
            }
            set
            {
                this.docEvenSeqField = value;
            }
        }

        /// <remarks/>
        public byte DocEveTp
        {
            get
            {
                return this.docEveTpField;
            }
            set
            {
                this.docEveTpField = value;
            }
        }

        /// <remarks/>
        public object DocEveId
        {
            get
            {
                return this.docEveIdField;
            }
            set
            {
                this.docEveIdField = value;
            }
        }

        /// <remarks/>
        public object DocPDFBase64
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
        public object DocPDFDownload
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
        public System.DateTime DocDhAut
        {
            get
            {
                return this.docDhAutField;
            }
            set
            {
                this.docDhAutField = value;
            }
        }

        /// <remarks/>
        public string DocDigestValue
        {
            get
            {
                return this.docDigestValueField;
            }
            set
            {
                this.docDigestValueField = value;
            }
        }

        /// <remarks/>
        public object DocXMLBase64
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
        public object DocXMLDownload
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
        public object DocImpressora
        {
            get
            {
                return this.docImpressoraField;
            }
            set
            {
                this.docImpressoraField = value;
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

        public string xml { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class DocumentoSituacao
    {

        private byte sitCodigoField;

        private string sitDescricaoField;

        /// <remarks/>
        public byte SitCodigo
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

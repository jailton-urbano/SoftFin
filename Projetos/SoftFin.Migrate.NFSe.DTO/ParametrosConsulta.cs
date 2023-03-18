using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Migrate.NFSe.DTO
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class ParametrosConsulta
    {

        private string situacaoField;

        private string xMLCompletoField;

        private string xMLLinkField;

        private string pDFBase64Field;

        private string pDFLinkField;

        private string eventosField;

        /// <remarks/>
        public string Situacao
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

        /// <remarks/>
        public string XMLCompleto
        {
            get
            {
                return this.xMLCompletoField;
            }
            set
            {
                this.xMLCompletoField = value;
            }
        }

        /// <remarks/>
        public string XMLLink
        {
            get
            {
                return this.xMLLinkField;
            }
            set
            {
                this.xMLLinkField = value;
            }
        }

        /// <remarks/>
        public string PDFBase64
        {
            get
            {
                return this.pDFBase64Field;
            }
            set
            {
                this.pDFBase64Field = value;
            }
        }

        /// <remarks/>
        public string PDFLink
        {
            get
            {
                return this.pDFLinkField;
            }
            set
            {
                this.pDFLinkField = value;
            }
        }

        /// <remarks/>
        public string Eventos
        {
            get
            {
                return this.eventosField;
            }
            set
            {
                this.eventosField = value;
            }
        }
    }


}

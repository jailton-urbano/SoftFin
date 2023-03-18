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
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "")]
    public partial class Consulta
    {

        private string modeloDocumentoField;

        private string versaoField;

        private string tpAmbField;

        private string cnpjEmissorField;

        private string numeroInicialField;

        private string numeroFinalField;

        private string serieField;

        private string chaveAcessoField;

        private string dataEmissaoInicialField;

        private string dataEmissaoFinalField;

        /// <remarks/>
        public string ModeloDocumento
        {
            get
            {
                return this.modeloDocumentoField;
            }
            set
            {
                this.modeloDocumentoField = value;
            }
        }

        /// <remarks/>
        public string Versao
        {
            get
            {
                return this.versaoField;
            }
            set
            {
                this.versaoField = value;
            }
        }

        /// <remarks/>
        public string tpAmb
        {
            get
            {
                return this.tpAmbField;
            }
            set
            {
                this.tpAmbField = value;
            }
        }

        /// <remarks/>
        public string CnpjEmissor
        {
            get
            {
                return this.cnpjEmissorField;
            }
            set
            {
                this.cnpjEmissorField = value;
            }
        }

        /// <remarks/>
        public string NumeroInicial
        {
            get
            {
                return this.numeroInicialField;
            }
            set
            {
                this.numeroInicialField = value;
            }
        }

        /// <remarks/>
        public string NumeroFinal
        {
            get
            {
                return this.numeroFinalField;
            }
            set
            {
                this.numeroFinalField = value;
            }
        }

        /// <remarks/>
        public string Serie
        {
            get
            {
                return this.serieField;
            }
            set
            {
                this.serieField = value;
            }
        }

        /// <remarks/>
        public string ChaveAcesso
        {
            get
            {
                return this.chaveAcessoField;
            }
            set
            {
                this.chaveAcessoField = value;
            }
        }

        /// <remarks/>
        public string DataEmissaoInicial
        {
            get
            {
                return this.dataEmissaoInicialField;
            }
            set
            {
                this.dataEmissaoInicialField = value;
            }
        }

        /// <remarks/>
        public string DataEmissaoFinal
        {
            get
            {
                return this.dataEmissaoFinalField;
            }
            set
            {
                this.dataEmissaoFinalField = value;
            }
        }
    }


}

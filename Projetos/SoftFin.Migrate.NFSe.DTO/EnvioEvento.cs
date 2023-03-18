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
    public partial class EnvioEvento
    {

        private string modeloDocumentoField;

        private string versaoField;

        private EnvioEventoEvento eventoField;

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
        public EnvioEventoEvento Evento
        {
            get
            {
                return this.eventoField;
            }
            set
            {
                this.eventoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioEventoEvento
    {

        private string cNPJField;

        private string nFSeNumeroField;

        private string rPSNumeroField;

        private string rPSSerieField;

        private string eveTpField;

        private string tpAmbField;

        private string eveCodigoField;

        private string eveMotivoField;

        /// <remarks/>
        public string CNPJ
        {
            get
            {
                return this.cNPJField;
            }
            set
            {
                this.cNPJField = value;
            }
        }

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
        public string RPSNumero
        {
            get
            {
                return this.rPSNumeroField;
            }
            set
            {
                this.rPSNumeroField = value;
            }
        }

        /// <remarks/>
        public string RPSSerie
        {
            get
            {
                return this.rPSSerieField;
            }
            set
            {
                this.rPSSerieField = value;
            }
        }

        /// <remarks/>
        public string EveTp
        {
            get
            {
                return this.eveTpField;
            }
            set
            {
                this.eveTpField = value;
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
        public string EveCodigo
        {
            get
            {
                return this.eveCodigoField;
            }
            set
            {
                this.eveCodigoField = value;
            }
        }

        /// <remarks/>
        public string EveMotivo
        {
            get
            {
                return this.eveMotivoField;
            }
            set
            {
                this.eveMotivoField = value;
            }
        }
    }


}

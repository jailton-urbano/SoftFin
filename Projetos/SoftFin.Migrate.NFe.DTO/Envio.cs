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
    public partial class Envio
    {

        private string modeloDocumentoField;

        private string versaoField;

        private EnvioIde ideField;

        private EnvioEmit emitField;

        private EnvioDest destField;

        private EnvioAutXML autXMLField;

        private EnvioDet detField;

        private EnvioTotal totalField;

        private EnvioTransp transpField;

        private EnvioPag pagField;

        private string vTrocoField;

        private EnvioInfAdic infAdicField;

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
        public EnvioIde ide
        {
            get
            {
                return this.ideField;
            }
            set
            {
                this.ideField = value;
            }
        }

        /// <remarks/>
        public EnvioEmit emit
        {
            get
            {
                return this.emitField;
            }
            set
            {
                this.emitField = value;
            }
        }

        /// <remarks/>
        public EnvioDest dest
        {
            get
            {
                return this.destField;
            }
            set
            {
                this.destField = value;
            }
        }

        /// <remarks/>
        public EnvioAutXML autXML
        {
            get
            {
                return this.autXMLField;
            }
            set
            {
                this.autXMLField = value;
            }
        }

        /// <remarks/>
        public EnvioDet det
        {
            get
            {
                return this.detField;
            }
            set
            {
                this.detField = value;
            }
        }

        /// <remarks/>
        public EnvioTotal total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        public EnvioTransp transp
        {
            get
            {
                return this.transpField;
            }
            set
            {
                this.transpField = value;
            }
        }

        /// <remarks/>
        public EnvioPag pag
        {
            get
            {
                return this.pagField;
            }
            set
            {
                this.pagField = value;
            }
        }

        /// <remarks/>
        public string vTroco
        {
            get
            {
                return this.vTrocoField;
            }
            set
            {
                this.vTrocoField = value;
            }
        }

        /// <remarks/>
        public EnvioInfAdic infAdic
        {
            get
            {
                return this.infAdicField;
            }
            set
            {
                this.infAdicField = value;
            }
        }

        public retirada retirada { get; set; }
        public entrega entrega { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioIde
    {
        public string dhCont;
        private string cNFField;

        private string cUFField;

        private string natOpField;

        private string modField;

        private string serieField;

        private string nNFField;

        private string dhEmiField;

        private string fusoHorarioField;

        private string tpNfField;

        private string idDestField;

        private string indFinalField;

        private string indPresField;

        private string cMunFgField;

        private string tpImpField;

        private string tpEmisField;

        private string tpAmbField;

        private string finNFeField;

        private string emailArquivosField;

        /// <remarks/>
        public string cNF
        {
            get
            {
                return this.cNFField;
            }
            set
            {
                this.cNFField = value;
            }
        }

        /// <remarks/>
        public string cUF
        {
            get
            {
                return this.cUFField;
            }
            set
            {
                this.cUFField = value;
            }
        }

        /// <remarks/>
        public string natOp
        {
            get
            {
                return this.natOpField;
            }
            set
            {
                this.natOpField = value;
            }
        }

        /// <remarks/>
        public string mod
        {
            get
            {
                return this.modField;
            }
            set
            {
                this.modField = value;
            }
        }

        /// <remarks/>
        public string serie
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
        public string nNF
        {
            get
            {
                return this.nNFField;
            }
            set
            {
                this.nNFField = value;
            }
        }

        /// <remarks/>
        public string dhEmi
        {
            get
            {
                return this.dhEmiField;
            }
            set
            {
                this.dhEmiField = value;
            }
        }

        /// <remarks/>
        public string fusoHorario
        {
            get
            {
                return this.fusoHorarioField;
            }
            set
            {
                this.fusoHorarioField = value;
            }
        }

        /// <remarks/>
        public string tpNf
        {
            get
            {
                return this.tpNfField;
            }
            set
            {
                this.tpNfField = value;
            }
        }

        /// <remarks/>
        public string idDest
        {
            get
            {
                return this.idDestField;
            }
            set
            {
                this.idDestField = value;
            }
        }

        /// <remarks/>
        public string indFinal
        {
            get
            {
                return this.indFinalField;
            }
            set
            {
                this.indFinalField = value;
            }
        }

        /// <remarks/>
        public string indPres
        {
            get
            {
                return this.indPresField;
            }
            set
            {
                this.indPresField = value;
            }
        }

        /// <remarks/>
        public string cMunFg
        {
            get
            {
                return this.cMunFgField;
            }
            set
            {
                this.cMunFgField = value;
            }
        }

        /// <remarks/>
        public string tpImp
        {
            get
            {
                return this.tpImpField;
            }
            set
            {
                this.tpImpField = value;
            }
        }

        /// <remarks/>
        public string tpEmis
        {
            get
            {
                return this.tpEmisField;
            }
            set
            {
                this.tpEmisField = value;
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
        public string finNFe
        {
            get
            {
                return this.finNFeField;
            }
            set
            {
                this.finNFeField = value;
            }
        }

        /// <remarks/>
        public string EmailArquivos
        {
            get
            {
                return this.emailArquivosField;
            }
            set
            {
                this.emailArquivosField = value;
            }
        }

        public string dhSaiEnt { get; set; }
        public string xJust { get; set; }
        public string NumeroPedido { get; set; }
        public List<NFRefItem> NFRef { get; set; }
    }

    public class NFRefItem
    {
        public string refNFe { get; set; }
        public string cUF_refNFE { get; set; }
        public string AAMM { get; set; }
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string mod_refNFE { get; set; }
        public string serie_refNFE { get; set; }
        public string nNF_refNFE { get; set; }
        public string IE_refNFP { get; set; }
        public string RefCte { get; set; }
        public string mod_refECF { get; set; }
        public string nECF_refECF { get; set; }
        public string nCOO_refECF { get; set; }

    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioEmit
    {

        private string cNPJ_emitField;

        private string xNomeField;

        private string xFantField;

        private string ieField;

        private string cRTField;

        private EnvioEmitEnderEmit enderEmitField;

        /// <remarks/>
        public string CNPJ_emit
        {
            get
            {
                return this.cNPJ_emitField;
            }
            set
            {
                this.cNPJ_emitField = value;
            }
        }

        /// <remarks/>
        public string xNome
        {
            get
            {
                return this.xNomeField;
            }
            set
            {
                this.xNomeField = value;
            }
        }

        /// <remarks/>
        public string xFant
        {
            get
            {
                return this.xFantField;
            }
            set
            {
                this.xFantField = value;
            }
        }

        /// <remarks/>
        public string IE
        {
            get
            {
                return this.ieField;
            }
            set
            {
                this.ieField = value;
            }
        }

        /// <remarks/>
        public string CRT
        {
            get
            {
                return this.cRTField;
            }
            set
            {
                this.cRTField = value;
            }
        }

        /// <remarks/>
        public EnvioEmitEnderEmit enderEmit
        {
            get
            {
                return this.enderEmitField;
            }
            set
            {
                this.enderEmitField = value;
            }
        }

        public int IM { get; set; }
        public string CNAE { get; set; }
        public string IEST { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioEmitEnderEmit
    {

        private string xLgrField;

        private string nroField;

        private string xCplField;

        private string xBairroField;

        private string cMunField;

        private string xMunField;

        private string ufField;

        private string cEPField;

        private string cPaisField;

        private string xPaisField;

        private string foneField;

        private string faxField;

        /// <remarks/>
        public string xLgr
        {
            get
            {
                return this.xLgrField;
            }
            set
            {
                this.xLgrField = value;
            }
        }

        /// <remarks/>
        public string nro
        {
            get
            {
                return this.nroField;
            }
            set
            {
                this.nroField = value;
            }
        }

        /// <remarks/>
        public string xCpl
        {
            get
            {
                return this.xCplField;
            }
            set
            {
                this.xCplField = value;
            }
        }

        /// <remarks/>
        public string xBairro
        {
            get
            {
                return this.xBairroField;
            }
            set
            {
                this.xBairroField = value;
            }
        }

        /// <remarks/>
        public string cMun
        {
            get
            {
                return this.cMunField;
            }
            set
            {
                this.cMunField = value;
            }
        }

        /// <remarks/>
        public string xMun
        {
            get
            {
                return this.xMunField;
            }
            set
            {
                this.xMunField = value;
            }
        }

        /// <remarks/>
        public string UF
        {
            get
            {
                return this.ufField;
            }
            set
            {
                this.ufField = value;
            }
        }

        /// <remarks/>
        public string CEP
        {
            get
            {
                return this.cEPField;
            }
            set
            {
                this.cEPField = value;
            }
        }

        /// <remarks/>
        public string cPais
        {
            get
            {
                return this.cPaisField;
            }
            set
            {
                this.cPaisField = value;
            }
        }

        /// <remarks/>
        public string xPais
        {
            get
            {
                return this.xPaisField;
            }
            set
            {
                this.xPaisField = value;
            }
        }

        /// <remarks/>
        public string fone
        {
            get
            {
                return this.foneField;
            }
            set
            {
                this.foneField = value;
            }
        }

        /// <remarks/>
        public string fax
        {
            get
            {
                return this.faxField;
            }
            set
            {
                this.faxField = value;
            }
        }

        public string Email { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDest
    {

        private string cNPJ_destField;

        private string xNome_destField;

        private string indIEDestField;

        private EnvioDestEnderDest enderDestField;

        /// <remarks/>
        public string CNPJ_dest
        {
            get
            {
                return this.cNPJ_destField;
            }
            set
            {
                this.cNPJ_destField = value;
            }
        }

        /// <remarks/>
        public string xNome_dest
        {
            get
            {
                return this.xNome_destField;
            }
            set
            {
                this.xNome_destField = value;
            }
        }

        /// <remarks/>
        public string indIEDest
        {
            get
            {
                return this.indIEDestField;
            }
            set
            {
                this.indIEDestField = value;
            }
        }

        /// <remarks/>
        public EnvioDestEnderDest enderDest
        {
            get
            {
                return this.enderDestField;
            }
            set
            {
                this.enderDestField = value;
            }
        }

        public string CPF_dest { get; set; }
        public string IE_dest { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDestEnderDest
    {

        private string nro_destField;

        private string xCpl_destField;

        private string xBairro_destField;

        private string xEmail_destField;

        private string xLgr_destField;

        private string xPais_destField;

        private string cMun_destField;

        private string xMun_destField;

        private string uF_destField;

        private string cEP_destField;

        private string cPais_destField;

        private string fone_destField;

        /// <remarks/>
        public string nro_dest
        {
            get
            {
                return this.nro_destField;
            }
            set
            {
                this.nro_destField = value;
            }
        }

        /// <remarks/>
        public string xCpl_dest
        {
            get
            {
                return this.xCpl_destField;
            }
            set
            {
                this.xCpl_destField = value;
            }
        }

        /// <remarks/>
        public string xBairro_dest
        {
            get
            {
                return this.xBairro_destField;
            }
            set
            {
                this.xBairro_destField = value;
            }
        }

        /// <remarks/>
        public string xEmail_dest
        {
            get
            {
                return this.xEmail_destField;
            }
            set
            {
                this.xEmail_destField = value;
            }
        }

        /// <remarks/>
        public string xLgr_dest
        {
            get
            {
                return this.xLgr_destField;
            }
            set
            {
                this.xLgr_destField = value;
            }
        }

        /// <remarks/>
        public string xPais_dest
        {
            get
            {
                return this.xPais_destField;
            }
            set
            {
                this.xPais_destField = value;
            }
        }

        /// <remarks/>
        public string cMun_dest
        {
            get
            {
                return this.cMun_destField;
            }
            set
            {
                this.cMun_destField = value;
            }
        }

        /// <remarks/>
        public string xMun_dest
        {
            get
            {
                return this.xMun_destField;
            }
            set
            {
                this.xMun_destField = value;
            }
        }

        /// <remarks/>
        public string UF_dest
        {
            get
            {
                return this.uF_destField;
            }
            set
            {
                this.uF_destField = value;
            }
        }

        /// <remarks/>
        public string CEP_dest
        {
            get
            {
                return this.cEP_destField;
            }
            set
            {
                this.cEP_destField = value;
            }
        }

        /// <remarks/>
        public string cPais_dest
        {
            get
            {
                return this.cPais_destField;
            }
            set
            {
                this.cPais_destField = value;
            }
        }

        /// <remarks/>
        public string fone_dest
        {
            get
            {
                return this.fone_destField;
            }
            set
            {
                this.fone_destField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioAutXML
    {

        private EnvioAutXMLAutXMLItem autXMLItemField;

        /// <remarks/>
        public EnvioAutXMLAutXMLItem autXMLItem
        {
            get
            {
                return this.autXMLItemField;
            }
            set
            {
                this.autXMLItemField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioAutXMLAutXMLItem
    {

        private string cNPJ_autField;

        /// <remarks/>
        public string CNPJ_aut
        {
            get
            {
                return this.cNPJ_autField;
            }
            set
            {
                this.cNPJ_autField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDet
    {

        private List<EnvioDetDetItem> detItemField;

        /// <remarks/>
        public List<EnvioDetDetItem> detItem
        {
            get
            {
                return this.detItemField;
            }
            set
            {
                this.detItemField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDetDetItem
    {

        private EnvioDetDetItemProd prodField;

        private EnvioDetDetItemImposto impostoField;

        private string infADProdField;

        /// <remarks/>
        public EnvioDetDetItemProd prod
        {
            get
            {
                return this.prodField;
            }
            set
            {
                this.prodField = value;
            }
        }

        /// <remarks/>
        public EnvioDetDetItemImposto imposto
        {
            get
            {
                return this.impostoField;
            }
            set
            {
                this.impostoField = value;
            }
        }

        /// <remarks/>
        public string infADProd
        {
            get
            {
                return this.infADProdField;
            }
            set
            {
                this.infADProdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDetDetItemProd
    {

        private string cProdField;

        private object cEANField;

        private string xProdField;

        private string nCMField;

        private string eXTIPIField;

        private string cFOPField;

        private string uCOMField;

        private string qCOMField;

        private string vUnComField;

        private string vProdField;

        private object cEANTribField;

        private string uTribField;

        private string qTribField;

        private string vUnTribField;

        private string vFreteField;

        private string vSegField;

        private string vDescField;

        private string indTotField;

        private string nTipoItemField;

        private string nRECOPIField;

        private EnvioDetDetItemProdVeicProd veicProdField;

        /// <remarks/>
        public string cProd
        {
            get
            {
                return this.cProdField;
            }
            set
            {
                this.cProdField = value;
            }
        }

        /// <remarks/>
        public object cEAN
        {
            get
            {
                return this.cEANField;
            }
            set
            {
                this.cEANField = value;
            }
        }

        /// <remarks/>
        public string xProd
        {
            get
            {
                return this.xProdField;
            }
            set
            {
                this.xProdField = value;
            }
        }

        /// <remarks/>
        public string NCM
        {
            get
            {
                return this.nCMField;
            }
            set
            {
                this.nCMField = value;
            }
        }

        /// <remarks/>
        public string EXTIPI
        {
            get
            {
                return this.eXTIPIField;
            }
            set
            {
                this.eXTIPIField = value;
            }
        }

        /// <remarks/>
        public string CFOP
        {
            get
            {
                return this.cFOPField;
            }
            set
            {
                this.cFOPField = value;
            }
        }

        /// <remarks/>
        public string uCOM
        {
            get
            {
                return this.uCOMField;
            }
            set
            {
                this.uCOMField = value;
            }
        }

        /// <remarks/>
        public string qCOM
        {
            get
            {
                return this.qCOMField;
            }
            set
            {
                this.qCOMField = value;
            }
        }

        /// <remarks/>
        public string vUnCom
        {
            get
            {
                return this.vUnComField;
            }
            set
            {
                this.vUnComField = value;
            }
        }

        /// <remarks/>
        public string vProd
        {
            get
            {
                return this.vProdField;
            }
            set
            {
                this.vProdField = value;
            }
        }

        /// <remarks/>
        public object cEANTrib
        {
            get
            {
                return this.cEANTribField;
            }
            set
            {
                this.cEANTribField = value;
            }
        }

        /// <remarks/>
        public string uTrib
        {
            get
            {
                return this.uTribField;
            }
            set
            {
                this.uTribField = value;
            }
        }

        /// <remarks/>
        public string qTrib
        {
            get
            {
                return this.qTribField;
            }
            set
            {
                this.qTribField = value;
            }
        }

        /// <remarks/>
        public string vUnTrib
        {
            get
            {
                return this.vUnTribField;
            }
            set
            {
                this.vUnTribField = value;
            }
        }

        /// <remarks/>
        public string vFrete
        {
            get
            {
                return this.vFreteField;
            }
            set
            {
                this.vFreteField = value;
            }
        }

        /// <remarks/>
        public string vSeg
        {
            get
            {
                return this.vSegField;
            }
            set
            {
                this.vSegField = value;
            }
        }

        /// <remarks/>
        public string vDesc
        {
            get
            {
                return this.vDescField;
            }
            set
            {
                this.vDescField = value;
            }
        }

        /// <remarks/>
        public string indTot
        {
            get
            {
                return this.indTotField;
            }
            set
            {
                this.indTotField = value;
            }
        }

        /// <remarks/>
        public string nTipoItem
        {
            get
            {
                return this.nTipoItemField;
            }
            set
            {
                this.nTipoItemField = value;
            }
        }

        /// <remarks/>
        public string nRECOPI
        {
            get
            {
                return this.nRECOPIField;
            }
            set
            {
                this.nRECOPIField = value;
            }
        }

        /// <remarks/>
        public EnvioDetDetItemProdVeicProd veicProd
        {
            get
            {
                return this.veicProdField;
            }
            set
            {
                this.veicProdField = value;
            }
        }

        public string nItemPed { get; set; }
        public string dProd { get; set; }
        public string CEST { get; set; }
        public string indEscala { get; set; }
        public string CNPJFab { get; set; }
        public string cBenef { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDetDetItemProdVeicProd
    {

        private string tpOpField;

        private string chassiField;

        private string cCorField;

        private string xCorField;

        private string potField;

        private string cilinField;

        private string pesoLField;

        private string pesoBField;

        private string nSerieField;

        private string tpCombField;

        private string nMotorField;

        private string cMTField;

        private string distField;

        private string anoModField;

        private string anoFabField;

        private string tpPintField;

        private string tpVeicField;

        private string espVeicField;

        private string vINField;

        private string condVeicField;

        private string cModField;

        private string cCorDENATRANField;

        private string lotaField;

        private string tpRestField;

        /// <remarks/>
        public string tpOp
        {
            get
            {
                return this.tpOpField;
            }
            set
            {
                this.tpOpField = value;
            }
        }

        /// <remarks/>
        public string chassi
        {
            get
            {
                return this.chassiField;
            }
            set
            {
                this.chassiField = value;
            }
        }

        /// <remarks/>
        public string cCor
        {
            get
            {
                return this.cCorField;
            }
            set
            {
                this.cCorField = value;
            }
        }

        /// <remarks/>
        public string xCor
        {
            get
            {
                return this.xCorField;
            }
            set
            {
                this.xCorField = value;
            }
        }

        /// <remarks/>
        public string pot
        {
            get
            {
                return this.potField;
            }
            set
            {
                this.potField = value;
            }
        }

        /// <remarks/>
        public string cilin
        {
            get
            {
                return this.cilinField;
            }
            set
            {
                this.cilinField = value;
            }
        }

        /// <remarks/>
        public string PesoL
        {
            get
            {
                return this.pesoLField;
            }
            set
            {
                this.pesoLField = value;
            }
        }

        /// <remarks/>
        public string PesoB
        {
            get
            {
                return this.pesoBField;
            }
            set
            {
                this.pesoBField = value;
            }
        }

        /// <remarks/>
        public string nSerie
        {
            get
            {
                return this.nSerieField;
            }
            set
            {
                this.nSerieField = value;
            }
        }

        /// <remarks/>
        public string tpComb
        {
            get
            {
                return this.tpCombField;
            }
            set
            {
                this.tpCombField = value;
            }
        }

        /// <remarks/>
        public string nMotor
        {
            get
            {
                return this.nMotorField;
            }
            set
            {
                this.nMotorField = value;
            }
        }

        /// <remarks/>
        public string CMT
        {
            get
            {
                return this.cMTField;
            }
            set
            {
                this.cMTField = value;
            }
        }

        /// <remarks/>
        public string dist
        {
            get
            {
                return this.distField;
            }
            set
            {
                this.distField = value;
            }
        }

        /// <remarks/>
        public string anoMod
        {
            get
            {
                return this.anoModField;
            }
            set
            {
                this.anoModField = value;
            }
        }

        /// <remarks/>
        public string anoFab
        {
            get
            {
                return this.anoFabField;
            }
            set
            {
                this.anoFabField = value;
            }
        }

        /// <remarks/>
        public string tpPint
        {
            get
            {
                return this.tpPintField;
            }
            set
            {
                this.tpPintField = value;
            }
        }

        /// <remarks/>
        public string tpVeic
        {
            get
            {
                return this.tpVeicField;
            }
            set
            {
                this.tpVeicField = value;
            }
        }

        /// <remarks/>
        public string espVeic
        {
            get
            {
                return this.espVeicField;
            }
            set
            {
                this.espVeicField = value;
            }
        }

        /// <remarks/>
        public string VIN
        {
            get
            {
                return this.vINField;
            }
            set
            {
                this.vINField = value;
            }
        }

        /// <remarks/>
        public string condVeic
        {
            get
            {
                return this.condVeicField;
            }
            set
            {
                this.condVeicField = value;
            }
        }

        /// <remarks/>
        public string cMod
        {
            get
            {
                return this.cModField;
            }
            set
            {
                this.cModField = value;
            }
        }

        /// <remarks/>
        public string cCorDENATRAN
        {
            get
            {
                return this.cCorDENATRANField;
            }
            set
            {
                this.cCorDENATRANField = value;
            }
        }

        /// <remarks/>
        public string lota
        {
            get
            {
                return this.lotaField;
            }
            set
            {
                this.lotaField = value;
            }
        }

        /// <remarks/>
        public string tpRest
        {
            get
            {
                return this.tpRestField;
            }
            set
            {
                this.tpRestField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDetDetItemImposto
    {

        private EnvioDetDetItemImpostoICMS iCMSField;

        private EnvioDetDetItemImpostoPIS pISField;

        private EnvioDetDetItemImpostoCOFINS cOFINSField;

        /// <remarks/>
        public EnvioDetDetItemImpostoICMS ICMS
        {
            get
            {
                return this.iCMSField;
            }
            set
            {
                this.iCMSField = value;
            }
        }

        /// <remarks/>
        public EnvioDetDetItemImpostoPIS PIS
        {
            get
            {
                return this.pISField;
            }
            set
            {
                this.pISField = value;
            }
        }

        /// <remarks/>
        public EnvioDetDetItemImpostoCOFINS COFINS
        {
            get
            {
                return this.cOFINSField;
            }
            set
            {
                this.cOFINSField = value;
            }
        }

        public string vTotTrib { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDetDetItemImpostoICMS
    {

        private string origField;

        private string cSTField;

        private string modBCField;

        private string vBCField;

        private string pICMSField;

        private string vICMS_icmsField;

        private string pRedBCField;

        private string pBCOpField;

        private string pFCPField;

        private string vFCPField;

        private string vBCFCPField;

        private string vBCFCPSTField;

        private string pFCPSTField;

        private string vFCPSTField;

        private string pSTField;

        private string vBCFCPSTRetField;

        private string pFCPSTRetField;

        private string vFCPSTRetField;

        /// <remarks/>
        public string orig
        {
            get
            {
                return this.origField;
            }
            set
            {
                this.origField = value;
            }
        }

        /// <remarks/>
        public string CST
        {
            get
            {
                return this.cSTField;
            }
            set
            {
                this.cSTField = value;
            }
        }

        /// <remarks/>
        public string modBC
        {
            get
            {
                return this.modBCField;
            }
            set
            {
                this.modBCField = value;
            }
        }

        /// <remarks/>
        public string vBC
        {
            get
            {
                return this.vBCField;
            }
            set
            {
                this.vBCField = value;
            }
        }

        /// <remarks/>
        public string pICMS
        {
            get
            {
                return this.pICMSField;
            }
            set
            {
                this.pICMSField = value;
            }
        }

        /// <remarks/>
        public string vICMS_icms
        {
            get
            {
                return this.vICMS_icmsField;
            }
            set
            {
                this.vICMS_icmsField = value;
            }
        }

        /// <remarks/>
        public string pRedBC
        {
            get
            {
                return this.pRedBCField;
            }
            set
            {
                this.pRedBCField = value;
            }
        }

        /// <remarks/>
        public string pBCOp
        {
            get
            {
                return this.pBCOpField;
            }
            set
            {
                this.pBCOpField = value;
            }
        }

        /// <remarks/>
        public string pFCP
        {
            get
            {
                return this.pFCPField;
            }
            set
            {
                this.pFCPField = value;
            }
        }

        /// <remarks/>
        public string vFCP
        {
            get
            {
                return this.vFCPField;
            }
            set
            {
                this.vFCPField = value;
            }
        }

        /// <remarks/>
        public string vBCFCP
        {
            get
            {
                return this.vBCFCPField;
            }
            set
            {
                this.vBCFCPField = value;
            }
        }

        /// <remarks/>
        public string vBCFCPST
        {
            get
            {
                return this.vBCFCPSTField;
            }
            set
            {
                this.vBCFCPSTField = value;
            }
        }

        /// <remarks/>
        public string pFCPST
        {
            get
            {
                return this.pFCPSTField;
            }
            set
            {
                this.pFCPSTField = value;
            }
        }

        /// <remarks/>
        public string vFCPST
        {
            get
            {
                return this.vFCPSTField;
            }
            set
            {
                this.vFCPSTField = value;
            }
        }

        /// <remarks/>
        public string pST
        {
            get
            {
                return this.pSTField;
            }
            set
            {
                this.pSTField = value;
            }
        }

        /// <remarks/>
        public string vBCFCPSTRet
        {
            get
            {
                return this.vBCFCPSTRetField;
            }
            set
            {
                this.vBCFCPSTRetField = value;
            }
        }

        /// <remarks/>
        public string pFCPSTRet
        {
            get
            {
                return this.pFCPSTRetField;
            }
            set
            {
                this.pFCPSTRetField = value;
            }
        }

        /// <remarks/>
        public string vFCPSTRet
        {
            get
            {
                return this.vFCPSTRetField;
            }
            set
            {
                this.vFCPSTRetField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDetDetItemImpostoPIS
    {

        private string cST_pisField;

        /// <remarks/>
        public string CST_pis
        {
            get
            {
                return this.cST_pisField;
            }
            set
            {
                this.cST_pisField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioDetDetItemImpostoCOFINS
    {

        private string cST_cofinsField;

        /// <remarks/>
        public string CST_cofins
        {
            get
            {
                return this.cST_cofinsField;
            }
            set
            {
                this.cST_cofinsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioTotal
    {

        private EnvioTotalICMStot iCMStotField;

        /// <remarks/>
        public EnvioTotalICMStot ICMStot
        {
            get
            {
                return this.iCMStotField;
            }
            set
            {
                this.iCMStotField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioTotalICMStot
    {

        private string vBC_ttlnfeField;

        private string vICMS_ttlnfeField;

        private string vICMSDeson_ttlnfeField;

        private string vBCST_ttlnfeField;

        private string vST_ttlnfeField;

        private string vProd_ttlnfeField;

        private string vFrete_ttlnfeField;

        private string vSeg_ttlnfeField;

        private string vDesc_ttlnfeField;

        private string vII_ttlnfeField;

        private string vIPI_ttlnfeField;

        private string vPIS_ttlnfeField;

        private string vCOFINS_ttlnfeField;

        private string vOutroField;

        private string vNFField;

        private string vFCP_ttlnfeField;

        private string vFCPST_ttlnfeField;

        private string vFCPSTRet_ttlnfeField;

        private string vIPIDevol_ttlnfeField;

        /// <remarks/>
        public string vBC_ttlnfe
        {
            get
            {
                return this.vBC_ttlnfeField;
            }
            set
            {
                this.vBC_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vICMS_ttlnfe
        {
            get
            {
                return this.vICMS_ttlnfeField;
            }
            set
            {
                this.vICMS_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vICMSDeson_ttlnfe
        {
            get
            {
                return this.vICMSDeson_ttlnfeField;
            }
            set
            {
                this.vICMSDeson_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vBCST_ttlnfe
        {
            get
            {
                return this.vBCST_ttlnfeField;
            }
            set
            {
                this.vBCST_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vST_ttlnfe
        {
            get
            {
                return this.vST_ttlnfeField;
            }
            set
            {
                this.vST_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vProd_ttlnfe
        {
            get
            {
                return this.vProd_ttlnfeField;
            }
            set
            {
                this.vProd_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vFrete_ttlnfe
        {
            get
            {
                return this.vFrete_ttlnfeField;
            }
            set
            {
                this.vFrete_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vSeg_ttlnfe
        {
            get
            {
                return this.vSeg_ttlnfeField;
            }
            set
            {
                this.vSeg_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vDesc_ttlnfe
        {
            get
            {
                return this.vDesc_ttlnfeField;
            }
            set
            {
                this.vDesc_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vII_ttlnfe
        {
            get
            {
                return this.vII_ttlnfeField;
            }
            set
            {
                this.vII_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vIPI_ttlnfe
        {
            get
            {
                return this.vIPI_ttlnfeField;
            }
            set
            {
                this.vIPI_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vPIS_ttlnfe
        {
            get
            {
                return this.vPIS_ttlnfeField;
            }
            set
            {
                this.vPIS_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vCOFINS_ttlnfe
        {
            get
            {
                return this.vCOFINS_ttlnfeField;
            }
            set
            {
                this.vCOFINS_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vOutro
        {
            get
            {
                return this.vOutroField;
            }
            set
            {
                this.vOutroField = value;
            }
        }

        /// <remarks/>
        public string vNF
        {
            get
            {
                return this.vNFField;
            }
            set
            {
                this.vNFField = value;
            }
        }

        /// <remarks/>
        public string vFCP_ttlnfe
        {
            get
            {
                return this.vFCP_ttlnfeField;
            }
            set
            {
                this.vFCP_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vFCPST_ttlnfe
        {
            get
            {
                return this.vFCPST_ttlnfeField;
            }
            set
            {
                this.vFCPST_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vFCPSTRet_ttlnfe
        {
            get
            {
                return this.vFCPSTRet_ttlnfeField;
            }
            set
            {
                this.vFCPSTRet_ttlnfeField = value;
            }
        }

        /// <remarks/>
        public string vIPIDevol_ttlnfe
        {
            get
            {
                return this.vIPIDevol_ttlnfeField;
            }
            set
            {
                this.vIPIDevol_ttlnfeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioTransp
    {

        private string modFreteField;

        private EnvioTranspVeicTransp veicTranspField;

        private EnvioTranspReboqueItem[] reboqueField;

        /// <remarks/>
        public string modFrete
        {
            get
            {
                return this.modFreteField;
            }
            set
            {
                this.modFreteField = value;
            }
        }

        /// <remarks/>
        public EnvioTranspVeicTransp veicTransp
        {
            get
            {
                return this.veicTranspField;
            }
            set
            {
                this.veicTranspField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("reboqueItem", IsNullable = false)]
        public EnvioTranspReboqueItem[] reboque
        {
            get
            {
                return this.reboqueField;
            }
            set
            {
                this.reboqueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioTranspVeicTransp
    {

        private string placaField;

        private string uF_veictranspField;

        private string rNTCField;

        /// <remarks/>
        public string placa
        {
            get
            {
                return this.placaField;
            }
            set
            {
                this.placaField = value;
            }
        }

        /// <remarks/>
        public string UF_veictransp
        {
            get
            {
                return this.uF_veictranspField;
            }
            set
            {
                this.uF_veictranspField = value;
            }
        }

        /// <remarks/>
        public string RNTC
        {
            get
            {
                return this.rNTCField;
            }
            set
            {
                this.rNTCField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioTranspReboqueItem
    {

        private string placa_rebtranspField;

        private string uF_rebtranspField;

        private string rNTC_rebtranspField;

        /// <remarks/>
        public string placa_rebtransp
        {
            get
            {
                return this.placa_rebtranspField;
            }
            set
            {
                this.placa_rebtranspField = value;
            }
        }

        /// <remarks/>
        public string UF_rebtransp
        {
            get
            {
                return this.uF_rebtranspField;
            }
            set
            {
                this.uF_rebtranspField = value;
            }
        }

        /// <remarks/>
        public string RNTC_rebtransp
        {
            get
            {
                return this.rNTC_rebtranspField;
            }
            set
            {
                this.rNTC_rebtranspField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioPag
    {

        private List<EnvioPagPagItem> pagItemField;

        /// <remarks/>
        public List<EnvioPagPagItem> pagItem
        {
            get
            {
                return this.pagItemField;
            }
            set
            {
                this.pagItemField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioPagPagItem
    {

        private string tPagField;

        private string vPagField;

        /// <remarks/>
        public string tPag
        {
            get
            {
                return this.tPagField;
            }
            set
            {
                this.tPagField = value;
            }
        }

        /// <remarks/>
        public string vPag
        {
            get
            {
                return this.vPagField;
            }
            set
            {
                this.vPagField = value;
            }
        }

        public string vTroco { get; set; }
        public string indPag_pag { get; set; }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioInfAdic
    {

        private string infAdFiscoField;

        private string infCplField;

        /// <remarks/>
        public string infAdFisco
        {
            get
            {
                return this.infAdFiscoField;
            }
            set
            {
                this.infAdFiscoField = value;
            }
        }

        /// <remarks/>
        public string infCpl
        {
            get
            {
                return this.infCplField;
            }
            set
            {
                this.infCplField = value;
            }
        }
    }


    public class retirada
    {
        public string CNPJ_ret { get; set; }
        public string CPF_ret { get; set; }
        public string xLgr_ret { get; set; }
        public string nro_ret { get; set; }
        public string xCpl_ret { get; set; }
        public string xBairro_ret { get; set; }
        public string xMun_ret { get; set; }
        public string cMun_ret { get; set; }
        public string UF_ret { get; set; }
    }

    public class entrega
    {
        public string CNPJ_entr { get; set; }
        public string CPF_entr { get; set; }
        public string xLgr_entr { get; set; }
        public string nro_entr { get; set; }
        public string xCpl_entr { get; set; }
        public string xBairro_entr { get; set; }
        public string cMun_entr { get; set; }
        public string xMun_entr { get; set; }
        public string UF_entr { get; set; }
    }




}

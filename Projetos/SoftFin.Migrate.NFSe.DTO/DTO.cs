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
    public partial class Envio
    {

        private string modeloDocumentoField;

        private string versaoField;

        private EnvioRPS rPSField;

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
        public EnvioRPS RPS
        {
            get
            {
                return this.rPSField;
            }
            set
            {
                this.rPSField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPS
    {

        private string rPSNumeroField;

        private string rPSSerieField;

        private string rPSTipoField;

        private string dEmisField;

        private string dCompetenciaField;

        private string localPrestServField;

        private string natOpField;

        private string operacaoField;

        private string numProcessoField;

        private string regEspTribField;

        private string optSNField;

        private string incCultField;

        private string statusField;

        private string cVerificaRPSField;

        private string empreitadaGlobalField;

        private string tpAmbField;

        private EnvioRPSRPSSubs rPSSubsField;

        private EnvioRPSPrestador prestadorField;

        private EnvioRPSListaItens listaItensField;

        private EnvioRPSListaParcelas listaParcelasField;

        private EnvioRPSServico servicoField;

        private EnvioRPSTomador tomadorField;

        private EnvioRPSIntermServico intermServicoField;

        private EnvioRPSConstCivil constCivilField;

        private EnvioRPSListaDed listaDedField;

        private EnvioRPSTransportadora transportadoraField;

        private string nFSOutrasinformacoesField;

        private string rPSCanhotoField;

        private string arquivoField;

        private string extensaoArquivoField;

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
        public string RPSTipo
        {
            get
            {
                return this.rPSTipoField;
            }
            set
            {
                this.rPSTipoField = value;
            }
        }

        /// <remarks/>
        public string dEmis
        {
            get
            {
                return this.dEmisField;
            }
            set
            {
                this.dEmisField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public string dCompetencia
        {
            get
            {
                return this.dCompetenciaField;
            }
            set
            {
                this.dCompetenciaField = value;
            }
        }

        /// <remarks/>
        public string LocalPrestServ
        {
            get
            {
                return this.localPrestServField;
            }
            set
            {
                this.localPrestServField = value;
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
        public string Operacao
        {
            get
            {
                return this.operacaoField;
            }
            set
            {
                this.operacaoField = value;
            }
        }

        /// <remarks/>
        public string NumProcesso
        {
            get
            {
                return this.numProcessoField;
            }
            set
            {
                this.numProcessoField = value;
            }
        }

        /// <remarks/>
        public string RegEspTrib
        {
            get
            {
                return this.regEspTribField;
            }
            set
            {
                this.regEspTribField = value;
            }
        }

        /// <remarks/>
        public string OptSN
        {
            get
            {
                return this.optSNField;
            }
            set
            {
                this.optSNField = value;
            }
        }

        /// <remarks/>
        public string IncCult
        {
            get
            {
                return this.incCultField;
            }
            set
            {
                this.incCultField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string cVerificaRPS
        {
            get
            {
                return this.cVerificaRPSField;
            }
            set
            {
                this.cVerificaRPSField = value;
            }
        }

        /// <remarks/>
        public string EmpreitadaGlobal
        {
            get
            {
                return this.empreitadaGlobalField;
            }
            set
            {
                this.empreitadaGlobalField = value;
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
        public EnvioRPSRPSSubs RPSSubs
        {
            get
            {
                return this.rPSSubsField;
            }
            set
            {
                this.rPSSubsField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSPrestador Prestador
        {
            get
            {
                return this.prestadorField;
            }
            set
            {
                this.prestadorField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSListaItens ListaItens
        {
            get
            {
                return this.listaItensField;
            }
            set
            {
                this.listaItensField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSListaParcelas ListaParcelas
        {
            get
            {
                return this.listaParcelasField;
            }
            set
            {
                this.listaParcelasField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSServico Servico
        {
            get
            {
                return this.servicoField;
            }
            set
            {
                this.servicoField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSTomador Tomador
        {
            get
            {
                return this.tomadorField;
            }
            set
            {
                this.tomadorField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSIntermServico IntermServico
        {
            get
            {
                return this.intermServicoField;
            }
            set
            {
                this.intermServicoField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSConstCivil ConstCivil
        {
            get
            {
                return this.constCivilField;
            }
            set
            {
                this.constCivilField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSListaDed ListaDed
        {
            get
            {
                return this.listaDedField;
            }
            set
            {
                this.listaDedField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSTransportadora Transportadora
        {
            get
            {
                return this.transportadoraField;
            }
            set
            {
                this.transportadoraField = value;
            }
        }

        /// <remarks/>
        public string NFSOutrasinformacoes
        {
            get
            {
                return this.nFSOutrasinformacoesField;
            }
            set
            {
                this.nFSOutrasinformacoesField = value;
            }
        }

        /// <remarks/>
        public string RPSCanhoto
        {
            get
            {
                return this.rPSCanhotoField;
            }
            set
            {
                this.rPSCanhotoField = value;
            }
        }

        /// <remarks/>
        public string Arquivo
        {
            get
            {
                return this.arquivoField;
            }
            set
            {
                this.arquivoField = value;
            }
        }

        /// <remarks/>
        public string ExtensaoArquivo
        {
            get
            {
                return this.extensaoArquivoField;
            }
            set
            {
                this.extensaoArquivoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSRPSSubs
    {

        private string subsNumeroField;

        private string subsSerieField;

        private string subsTipoField;

        private string subsNFSeNumeroField;

        private string subsDEmisNFSeField;

        /// <remarks/>
        public string SubsNumero
        {
            get
            {
                return this.subsNumeroField;
            }
            set
            {
                this.subsNumeroField = value;
            }
        }

        /// <remarks/>
        public string SubsSerie
        {
            get
            {
                return this.subsSerieField;
            }
            set
            {
                this.subsSerieField = value;
            }
        }

        /// <remarks/>
        public string SubsTipo
        {
            get
            {
                return this.subsTipoField;
            }
            set
            {
                this.subsTipoField = value;
            }
        }

        /// <remarks/>
        public string SubsNFSeNumero
        {
            get
            {
                return this.subsNFSeNumeroField;
            }
            set
            {
                this.subsNFSeNumeroField = value;
            }
        }

        /// <remarks/>
        public string SubsDEmisNFSe
        {
            get
            {
                return this.subsDEmisNFSeField;
            }
            set
            {
                this.subsDEmisNFSeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSPrestador
    {

        private string cNPJ_prestField;

        private string cPF_prestField;

        private string xNomeField;

        private string xFantField;

        private string imField;

        private string ieField;

        private string cMCField;

        private EnvioRPSPrestadorEnderPrest enderPrestField;

        /// <remarks/>
        public string CNPJ_prest
        {
            get
            {
                return this.cNPJ_prestField;
            }
            set
            {
                this.cNPJ_prestField = value;
            }
        }

        /// <remarks/>
        public string CPF_prest
        {
            get
            {
                return this.cPF_prestField;
            }
            set
            {
                this.cPF_prestField = value;
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
        public string IM
        {
            get
            {
                return this.imField;
            }
            set
            {
                this.imField = value;
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
        public string CMC
        {
            get
            {
                return this.cMCField;
            }
            set
            {
                this.cMCField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSPrestadorEnderPrest enderPrest
        {
            get
            {
                return this.enderPrestField;
            }
            set
            {
                this.enderPrestField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSPrestadorEnderPrest
    {

        private string tPEndField;

        private string xLgrField;

        private string nroField;

        private string xCplField;

        private string xBairroField;

        private string cMunField;

        private string ufField;

        private string cEPField;

        private string foneField;

        private string emailField;

        /// <remarks/>
        public string TPEnd
        {
            get
            {
                return this.tPEndField;
            }
            set
            {
                this.tPEndField = value;
            }
        }

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
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSListaItens
    {

        private EnvioRPSListaItensItem itemField;

        /// <remarks/>
        public EnvioRPSListaItensItem Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSListaItensItem
    {

        private string itemSeqField;

        private string itemCodField;

        private string itemDescField;

        private string itemQtdeField;

        private string itemvUnitField;

        private string itemuMedField;

        private string itemvlDedField;

        private string itemTributavelField;

        private string itemcCnaeField;

        private string itemTributMunicipioField;

        private string itemnAlvaraField;

        private string itemvIssField;

        private string itemvDescontoField;

        private string itemAliquotaField;

        private string itemVlrTotalField;

        private string itemBaseCalculoField;

        private string itemvlrISSRetidoField;

        private string itemIssRetidoField;

        private string itemRespRetencaoField;

        private string itemIteListServicoField;

        private string itemExigibilidadeISSField;

        private string itemcMunIncidenciaField;

        private string itemNumProcessoField;

        private string itemDedTipoField;

        private string itemDedCPFRefField;

        private string itemDedCNPJRefField;

        private string itemDedNFRefField;

        private string itemDedvlTotRefField;

        private string itemDedPerField;

        private string itemVlrLiquidoField;

        private string itemValAliqINSSField;

        private string itemValINSSField;

        private string itemValAliqIRField;

        private string itemValIRField;

        private string itemValAliqCOFINSField;

        private string itemValCOFINSField;

        private string itemValAliqCSLLField;

        private string itemValCSLLField;

        private string itemValAliqPISField;

        private string itemValPISField;

        private string itemRedBCRetidoField;

        private string itemBCRetidoField;

        private string itemPaisImpDevidoField;

        private string itemValAliqISSRetidoField;

        private string itemJustDedField;

        /// <remarks/>
        public string ItemSeq
        {
            get
            {
                return this.itemSeqField;
            }
            set
            {
                this.itemSeqField = value;
            }
        }

        /// <remarks/>
        public string ItemCod
        {
            get
            {
                return this.itemCodField;
            }
            set
            {
                this.itemCodField = value;
            }
        }

        /// <remarks/>
        public string ItemDesc
        {
            get
            {
                return this.itemDescField;
            }
            set
            {
                this.itemDescField = value;
            }
        }

        /// <remarks/>
        public string ItemQtde
        {
            get
            {
                return this.itemQtdeField;
            }
            set
            {
                this.itemQtdeField = value;
            }
        }

        /// <remarks/>
        public string ItemvUnit
        {
            get
            {
                return this.itemvUnitField;
            }
            set
            {
                this.itemvUnitField = value;
            }
        }

        /// <remarks/>
        public string ItemuMed
        {
            get
            {
                return this.itemuMedField;
            }
            set
            {
                this.itemuMedField = value;
            }
        }

        /// <remarks/>
        public string ItemvlDed
        {
            get
            {
                return this.itemvlDedField;
            }
            set
            {
                this.itemvlDedField = value;
            }
        }

        /// <remarks/>
        public string ItemTributavel
        {
            get
            {
                return this.itemTributavelField;
            }
            set
            {
                this.itemTributavelField = value;
            }
        }

        /// <remarks/>
        public string ItemcCnae
        {
            get
            {
                return this.itemcCnaeField;
            }
            set
            {
                this.itemcCnaeField = value;
            }
        }

        /// <remarks/>
        public string ItemTributMunicipio
        {
            get
            {
                return this.itemTributMunicipioField;
            }
            set
            {
                this.itemTributMunicipioField = value;
            }
        }

        /// <remarks/>
        public string ItemnAlvara
        {
            get
            {
                return this.itemnAlvaraField;
            }
            set
            {
                this.itemnAlvaraField = value;
            }
        }

        /// <remarks/>
        public string ItemvIss
        {
            get
            {
                return this.itemvIssField;
            }
            set
            {
                this.itemvIssField = value;
            }
        }

        /// <remarks/>
        public string ItemvDesconto
        {
            get
            {
                return this.itemvDescontoField;
            }
            set
            {
                this.itemvDescontoField = value;
            }
        }

        /// <remarks/>
        public string ItemAliquota
        {
            get
            {
                return this.itemAliquotaField;
            }
            set
            {
                this.itemAliquotaField = value;
            }
        }

        /// <remarks/>
        public string ItemVlrTotal
        {
            get
            {
                return this.itemVlrTotalField;
            }
            set
            {
                this.itemVlrTotalField = value;
            }
        }

        /// <remarks/>
        public string ItemBaseCalculo
        {
            get
            {
                return this.itemBaseCalculoField;
            }
            set
            {
                this.itemBaseCalculoField = value;
            }
        }

        /// <remarks/>
        public string ItemvlrISSRetido
        {
            get
            {
                return this.itemvlrISSRetidoField;
            }
            set
            {
                this.itemvlrISSRetidoField = value;
            }
        }

        /// <remarks/>
        public string ItemIssRetido
        {
            get
            {
                return this.itemIssRetidoField;
            }
            set
            {
                this.itemIssRetidoField = value;
            }
        }

        /// <remarks/>
        public string ItemRespRetencao
        {
            get
            {
                return this.itemRespRetencaoField;
            }
            set
            {
                this.itemRespRetencaoField = value;
            }
        }

        /// <remarks/>
        public string ItemIteListServico
        {
            get
            {
                return this.itemIteListServicoField;
            }
            set
            {
                this.itemIteListServicoField = value;
            }
        }

        /// <remarks/>
        public string ItemExigibilidadeISS
        {
            get
            {
                return this.itemExigibilidadeISSField;
            }
            set
            {
                this.itemExigibilidadeISSField = value;
            }
        }

        /// <remarks/>
        public string ItemcMunIncidencia
        {
            get
            {
                return this.itemcMunIncidenciaField;
            }
            set
            {
                this.itemcMunIncidenciaField = value;
            }
        }

        /// <remarks/>
        public string ItemNumProcesso
        {
            get
            {
                return this.itemNumProcessoField;
            }
            set
            {
                this.itemNumProcessoField = value;
            }
        }

        /// <remarks/>
        public string ItemDedTipo
        {
            get
            {
                return this.itemDedTipoField;
            }
            set
            {
                this.itemDedTipoField = value;
            }
        }

        /// <remarks/>
        public string ItemDedCPFRef
        {
            get
            {
                return this.itemDedCPFRefField;
            }
            set
            {
                this.itemDedCPFRefField = value;
            }
        }

        /// <remarks/>
        public string ItemDedCNPJRef
        {
            get
            {
                return this.itemDedCNPJRefField;
            }
            set
            {
                this.itemDedCNPJRefField = value;
            }
        }

        /// <remarks/>
        public string ItemDedNFRef
        {
            get
            {
                return this.itemDedNFRefField;
            }
            set
            {
                this.itemDedNFRefField = value;
            }
        }

        /// <remarks/>
        public string ItemDedvlTotRef
        {
            get
            {
                return this.itemDedvlTotRefField;
            }
            set
            {
                this.itemDedvlTotRefField = value;
            }
        }

        /// <remarks/>
        public string ItemDedPer
        {
            get
            {
                return this.itemDedPerField;
            }
            set
            {
                this.itemDedPerField = value;
            }
        }

        /// <remarks/>
        public string ItemVlrLiquido
        {
            get
            {
                return this.itemVlrLiquidoField;
            }
            set
            {
                this.itemVlrLiquidoField = value;
            }
        }

        /// <remarks/>
        public string ItemValAliqINSS
        {
            get
            {
                return this.itemValAliqINSSField;
            }
            set
            {
                this.itemValAliqINSSField = value;
            }
        }

        /// <remarks/>
        public string ItemValINSS
        {
            get
            {
                return this.itemValINSSField;
            }
            set
            {
                this.itemValINSSField = value;
            }
        }

        /// <remarks/>
        public string ItemValAliqIR
        {
            get
            {
                return this.itemValAliqIRField;
            }
            set
            {
                this.itemValAliqIRField = value;
            }
        }

        /// <remarks/>
        public string ItemValIR
        {
            get
            {
                return this.itemValIRField;
            }
            set
            {
                this.itemValIRField = value;
            }
        }

        /// <remarks/>
        public string ItemValAliqCOFINS
        {
            get
            {
                return this.itemValAliqCOFINSField;
            }
            set
            {
                this.itemValAliqCOFINSField = value;
            }
        }

        /// <remarks/>
        public string ItemValCOFINS
        {
            get
            {
                return this.itemValCOFINSField;
            }
            set
            {
                this.itemValCOFINSField = value;
            }
        }

        /// <remarks/>
        public string ItemValAliqCSLL
        {
            get
            {
                return this.itemValAliqCSLLField;
            }
            set
            {
                this.itemValAliqCSLLField = value;
            }
        }

        /// <remarks/>
        public string ItemValCSLL
        {
            get
            {
                return this.itemValCSLLField;
            }
            set
            {
                this.itemValCSLLField = value;
            }
        }

        /// <remarks/>
        public string ItemValAliqPIS
        {
            get
            {
                return this.itemValAliqPISField;
            }
            set
            {
                this.itemValAliqPISField = value;
            }
        }

        /// <remarks/>
        public string ItemValPIS
        {
            get
            {
                return this.itemValPISField;
            }
            set
            {
                this.itemValPISField = value;
            }
        }

        /// <remarks/>
        public string ItemRedBCRetido
        {
            get
            {
                return this.itemRedBCRetidoField;
            }
            set
            {
                this.itemRedBCRetidoField = value;
            }
        }

        /// <remarks/>
        public string ItemBCRetido
        {
            get
            {
                return this.itemBCRetidoField;
            }
            set
            {
                this.itemBCRetidoField = value;
            }
        }

        /// <remarks/>
        public string ItemPaisImpDevido
        {
            get
            {
                return this.itemPaisImpDevidoField;
            }
            set
            {
                this.itemPaisImpDevidoField = value;
            }
        }

        /// <remarks/>
        public string ItemValAliqISSRetido
        {
            get
            {
                return this.itemValAliqISSRetidoField;
            }
            set
            {
                this.itemValAliqISSRetidoField = value;
            }
        }

        /// <remarks/>
        public string ItemJustDed
        {
            get
            {
                return this.itemJustDedField;
            }
            set
            {
                this.itemJustDedField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSListaParcelas
    {

        private EnvioRPSListaParcelasParcela parcelaField;

        /// <remarks/>
        public EnvioRPSListaParcelasParcela Parcela
        {
            get
            {
                return this.parcelaField;
            }
            set
            {
                this.parcelaField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSListaParcelasParcela
    {

        private string prcSequencialField;

        private string prcValorField;

        private string prcDtaVencimentoField;

        private string prcNroFaturaField;

        private string prcTipVencField;

        private string prcDscTipVencField;

        /// <remarks/>
        public string PrcSequencial
        {
            get
            {
                return this.prcSequencialField;
            }
            set
            {
                this.prcSequencialField = value;
            }
        }

        /// <remarks/>
        public string PrcValor
        {
            get
            {
                return this.prcValorField;
            }
            set
            {
                this.prcValorField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public string PrcDtaVencimento
        {
            get
            {
                return this.prcDtaVencimentoField;
            }
            set
            {
                this.prcDtaVencimentoField = value;
            }
        }

        /// <remarks/>
        public string PrcNroFatura
        {
            get
            {
                return this.prcNroFaturaField;
            }
            set
            {
                this.prcNroFaturaField = value;
            }
        }

        /// <remarks/>
        public string PrcTipVenc
        {
            get
            {
                return this.prcTipVencField;
            }
            set
            {
                this.prcTipVencField = value;
            }
        }

        /// <remarks/>
        public string PrcDscTipVenc
        {
            get
            {
                return this.prcDscTipVencField;
            }
            set
            {
                this.prcDscTipVencField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSServico
    {

        private EnvioRPSServicoValores valoresField;

        private EnvioRPSServicoLocalPrestacao localPrestacaoField;

        private string iteListServicoField;

        private string cnaeField;

        private string fPagamentoField;

        private string tributMunicipioField;

        private string tributMunicDescField;

        private string discriminacaoField;

        private string cMunField;

        private string serQuantidadeField;

        private string serUnidadeField;

        private string serNumAlvaField;

        private string paiPreServicoField;

        private string cMunIncidenciaField;

        private string dVencimentoField;

        private string obsInsPagamentoField;

        private string obrigoMunicField;

        private string tributacaoISSField;

        private string codigoAtividadeEconomicaField;

        private string servicoViasPublicasField;

        private string numeroParcelasField;

        private string nroOrcamentoField;

        /// <remarks/>
        public EnvioRPSServicoValores Valores
        {
            get
            {
                return this.valoresField;
            }
            set
            {
                this.valoresField = value;
            }
        }

        /// <remarks/>
        public EnvioRPSServicoLocalPrestacao LocalPrestacao
        {
            get
            {
                return this.localPrestacaoField;
            }
            set
            {
                this.localPrestacaoField = value;
            }
        }

        /// <remarks/>
        public string IteListServico
        {
            get
            {
                return this.iteListServicoField;
            }
            set
            {
                this.iteListServicoField = value;
            }
        }

        /// <remarks/>
        public string Cnae
        {
            get
            {
                return this.cnaeField;
            }
            set
            {
                this.cnaeField = value;
            }
        }

        /// <remarks/>
        public string fPagamento
        {
            get
            {
                return this.fPagamentoField;
            }
            set
            {
                this.fPagamentoField = value;
            }
        }

        /// <remarks/>
        public string TributMunicipio
        {
            get
            {
                return this.tributMunicipioField;
            }
            set
            {
                this.tributMunicipioField = value;
            }
        }

        /// <remarks/>
        public string TributMunicDesc
        {
            get
            {
                return this.tributMunicDescField;
            }
            set
            {
                this.tributMunicDescField = value;
            }
        }

        /// <remarks/>
        public string Discriminacao
        {
            get
            {
                return this.discriminacaoField;
            }
            set
            {
                this.discriminacaoField = value;
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
        public string SerQuantidade
        {
            get
            {
                return this.serQuantidadeField;
            }
            set
            {
                this.serQuantidadeField = value;
            }
        }

        /// <remarks/>
        public string SerUnidade
        {
            get
            {
                return this.serUnidadeField;
            }
            set
            {
                this.serUnidadeField = value;
            }
        }

        /// <remarks/>
        public string SerNumAlva
        {
            get
            {
                return this.serNumAlvaField;
            }
            set
            {
                this.serNumAlvaField = value;
            }
        }

        /// <remarks/>
        public string PaiPreServico
        {
            get
            {
                return this.paiPreServicoField;
            }
            set
            {
                this.paiPreServicoField = value;
            }
        }

        /// <remarks/>
        public string cMunIncidencia
        {
            get
            {
                return this.cMunIncidenciaField;
            }
            set
            {
                this.cMunIncidenciaField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public string dVencimento
        {
            get
            {
                return this.dVencimentoField;
            }
            set
            {
                this.dVencimentoField = value;
            }
        }

        /// <remarks/>
        public string ObsInsPagamento
        {
            get
            {
                return this.obsInsPagamentoField;
            }
            set
            {
                this.obsInsPagamentoField = value;
            }
        }

        /// <remarks/>
        public string ObrigoMunic
        {
            get
            {
                return this.obrigoMunicField;
            }
            set
            {
                this.obrigoMunicField = value;
            }
        }

        /// <remarks/>
        public string TributacaoISS
        {
            get
            {
                return this.tributacaoISSField;
            }
            set
            {
                this.tributacaoISSField = value;
            }
        }

        /// <remarks/>
        public string CodigoAtividadeEconomica
        {
            get
            {
                return this.codigoAtividadeEconomicaField;
            }
            set
            {
                this.codigoAtividadeEconomicaField = value;
            }
        }

        /// <remarks/>
        public string ServicoViasPublicas
        {
            get
            {
                return this.servicoViasPublicasField;
            }
            set
            {
                this.servicoViasPublicasField = value;
            }
        }

        /// <remarks/>
        public string NumeroParcelas
        {
            get
            {
                return this.numeroParcelasField;
            }
            set
            {
                this.numeroParcelasField = value;
            }
        }

        /// <remarks/>
        public string NroOrcamento
        {
            get
            {
                return this.nroOrcamentoField;
            }
            set
            {
                this.nroOrcamentoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSServicoValores
    {

        private string valServicosField;

        private string valDeducoesField;

        private string valPISField;

        private string valCOFINSField;

        private string valINSSField;

        private string valIRField;

        private string valCSLLField;

        private string valBCPISField;

        private string valBCCOFINSField;

        private string valBCINSSField;

        private string valBCIRRFField;

        private string valBCCSLLField;

        private string iSSRetidoField;

        private string respRetencaoField;

        private string tributavelField;

        private string valISSField;

        private string valISSRetidoField;

        private string valOutrasRetencoesField;

        private string valBaseCalculoField;

        private string valAliqISSField;

        private string valAliqPISField;

        private string pISRetidoField;

        private string valAliqCOFINSField;

        private string cOFINSRetidoField;

        private string valAliqIRField;

        private string iRRetidoField;

        private string valAliqCSLLField;

        private string cSLLRetidoField;

        private string valAliqINSSField;

        private string iNSSRetidoField;

        private string valAliqCppField;

        private string cppRetidoField;

        private string valCppField;

        private string outrasRetencoesRetidoField;

        private string valAliqTotTributosField;

        private string valLiquidoField;

        private string valDescIncondField;

        private string valDescCondField;

        private string valAliqISSoMunicField;

        private string infValPISField;

        private string infValCOFINSField;

        private string valLiqFaturaField;

        private string valBCISSRetidoField;

        private string nroFaturaField;

        private string cargaTribValorField;

        private string cargaTribPercentualField;

        private string cargaTribFonteField;

        private string justDedField;

        /// <remarks/>
        public string ValServicos
        {
            get
            {
                return this.valServicosField;
            }
            set
            {
                this.valServicosField = value;
            }
        }

        /// <remarks/>
        public string ValDeducoes
        {
            get
            {
                return this.valDeducoesField;
            }
            set
            {
                this.valDeducoesField = value;
            }
        }

        /// <remarks/>
        public string ValPIS
        {
            get
            {
                return this.valPISField;
            }
            set
            {
                this.valPISField = value;
            }
        }

        /// <remarks/>
        public string ValCOFINS
        {
            get
            {
                return this.valCOFINSField;
            }
            set
            {
                this.valCOFINSField = value;
            }
        }

        /// <remarks/>
        public string ValINSS
        {
            get
            {
                return this.valINSSField;
            }
            set
            {
                this.valINSSField = value;
            }
        }

        /// <remarks/>
        public string ValIR
        {
            get
            {
                return this.valIRField;
            }
            set
            {
                this.valIRField = value;
            }
        }

        /// <remarks/>
        public string ValCSLL
        {
            get
            {
                return this.valCSLLField;
            }
            set
            {
                this.valCSLLField = value;
            }
        }

        /// <remarks/>
        public string ValBCPIS
        {
            get
            {
                return this.valBCPISField;
            }
            set
            {
                this.valBCPISField = value;
            }
        }

        /// <remarks/>
        public string ValBCCOFINS
        {
            get
            {
                return this.valBCCOFINSField;
            }
            set
            {
                this.valBCCOFINSField = value;
            }
        }

        /// <remarks/>
        public string ValBCINSS
        {
            get
            {
                return this.valBCINSSField;
            }
            set
            {
                this.valBCINSSField = value;
            }
        }

        /// <remarks/>
        public string ValBCIRRF
        {
            get
            {
                return this.valBCIRRFField;
            }
            set
            {
                this.valBCIRRFField = value;
            }
        }

        /// <remarks/>
        public string ValBCCSLL
        {
            get
            {
                return this.valBCCSLLField;
            }
            set
            {
                this.valBCCSLLField = value;
            }
        }

        /// <remarks/>
        public string ISSRetido
        {
            get
            {
                return this.iSSRetidoField;
            }
            set
            {
                this.iSSRetidoField = value;
            }
        }

        /// <remarks/>
        public string RespRetencao
        {
            get
            {
                return this.respRetencaoField;
            }
            set
            {
                this.respRetencaoField = value;
            }
        }

        /// <remarks/>
        public string Tributavel
        {
            get
            {
                return this.tributavelField;
            }
            set
            {
                this.tributavelField = value;
            }
        }

        /// <remarks/>
        public string ValISS
        {
            get
            {
                return this.valISSField;
            }
            set
            {
                this.valISSField = value;
            }
        }

        /// <remarks/>
        public string ValISSRetido
        {
            get
            {
                return this.valISSRetidoField;
            }
            set
            {
                this.valISSRetidoField = value;
            }
        }

        /// <remarks/>
        public string ValOutrasRetencoes
        {
            get
            {
                return this.valOutrasRetencoesField;
            }
            set
            {
                this.valOutrasRetencoesField = value;
            }
        }

        /// <remarks/>
        public string ValBaseCalculo
        {
            get
            {
                return this.valBaseCalculoField;
            }
            set
            {
                this.valBaseCalculoField = value;
            }
        }

        /// <remarks/>
        public string ValAliqISS
        {
            get
            {
                return this.valAliqISSField;
            }
            set
            {
                this.valAliqISSField = value;
            }
        }

        /// <remarks/>
        public string ValAliqPIS
        {
            get
            {
                return this.valAliqPISField;
            }
            set
            {
                this.valAliqPISField = value;
            }
        }

        /// <remarks/>
        public string PISRetido
        {
            get
            {
                return this.pISRetidoField;
            }
            set
            {
                this.pISRetidoField = value;
            }
        }

        /// <remarks/>
        public string ValAliqCOFINS
        {
            get
            {
                return this.valAliqCOFINSField;
            }
            set
            {
                this.valAliqCOFINSField = value;
            }
        }

        /// <remarks/>
        public string COFINSRetido
        {
            get
            {
                return this.cOFINSRetidoField;
            }
            set
            {
                this.cOFINSRetidoField = value;
            }
        }

        /// <remarks/>
        public string ValAliqIR
        {
            get
            {
                return this.valAliqIRField;
            }
            set
            {
                this.valAliqIRField = value;
            }
        }

        /// <remarks/>
        public string IRRetido
        {
            get
            {
                return this.iRRetidoField;
            }
            set
            {
                this.iRRetidoField = value;
            }
        }

        /// <remarks/>
        public string ValAliqCSLL
        {
            get
            {
                return this.valAliqCSLLField;
            }
            set
            {
                this.valAliqCSLLField = value;
            }
        }

        /// <remarks/>
        public string CSLLRetido
        {
            get
            {
                return this.cSLLRetidoField;
            }
            set
            {
                this.cSLLRetidoField = value;
            }
        }

        /// <remarks/>
        public string ValAliqINSS
        {
            get
            {
                return this.valAliqINSSField;
            }
            set
            {
                this.valAliqINSSField = value;
            }
        }

        /// <remarks/>
        public string INSSRetido
        {
            get
            {
                return this.iNSSRetidoField;
            }
            set
            {
                this.iNSSRetidoField = value;
            }
        }

        /// <remarks/>
        public string ValAliqCpp
        {
            get
            {
                return this.valAliqCppField;
            }
            set
            {
                this.valAliqCppField = value;
            }
        }

        /// <remarks/>
        public string CppRetido
        {
            get
            {
                return this.cppRetidoField;
            }
            set
            {
                this.cppRetidoField = value;
            }
        }

        /// <remarks/>
        public string ValCpp
        {
            get
            {
                return this.valCppField;
            }
            set
            {
                this.valCppField = value;
            }
        }

        /// <remarks/>
        public string OutrasRetencoesRetido
        {
            get
            {
                return this.outrasRetencoesRetidoField;
            }
            set
            {
                this.outrasRetencoesRetidoField = value;
            }
        }

        /// <remarks/>
        public string ValAliqTotTributos
        {
            get
            {
                return this.valAliqTotTributosField;
            }
            set
            {
                this.valAliqTotTributosField = value;
            }
        }

        /// <remarks/>
        public string ValLiquido
        {
            get
            {
                return this.valLiquidoField;
            }
            set
            {
                this.valLiquidoField = value;
            }
        }

        /// <remarks/>
        public string ValDescIncond
        {
            get
            {
                return this.valDescIncondField;
            }
            set
            {
                this.valDescIncondField = value;
            }
        }

        /// <remarks/>
        public string ValDescCond
        {
            get
            {
                return this.valDescCondField;
            }
            set
            {
                this.valDescCondField = value;
            }
        }

        /// <remarks/>
        public string ValAliqISSoMunic
        {
            get
            {
                return this.valAliqISSoMunicField;
            }
            set
            {
                this.valAliqISSoMunicField = value;
            }
        }

        /// <remarks/>
        public string InfValPIS
        {
            get
            {
                return this.infValPISField;
            }
            set
            {
                this.infValPISField = value;
            }
        }

        /// <remarks/>
        public string InfValCOFINS
        {
            get
            {
                return this.infValCOFINSField;
            }
            set
            {
                this.infValCOFINSField = value;
            }
        }

        /// <remarks/>
        public string ValLiqFatura
        {
            get
            {
                return this.valLiqFaturaField;
            }
            set
            {
                this.valLiqFaturaField = value;
            }
        }

        /// <remarks/>
        public string ValBCISSRetido
        {
            get
            {
                return this.valBCISSRetidoField;
            }
            set
            {
                this.valBCISSRetidoField = value;
            }
        }

        /// <remarks/>
        public string NroFatura
        {
            get
            {
                return this.nroFaturaField;
            }
            set
            {
                this.nroFaturaField = value;
            }
        }

        /// <remarks/>
        public string CargaTribValor
        {
            get
            {
                return this.cargaTribValorField;
            }
            set
            {
                this.cargaTribValorField = value;
            }
        }

        /// <remarks/>
        public string CargaTribPercentual
        {
            get
            {
                return this.cargaTribPercentualField;
            }
            set
            {
                this.cargaTribPercentualField = value;
            }
        }

        /// <remarks/>
        public string CargaTribFonte
        {
            get
            {
                return this.cargaTribFonteField;
            }
            set
            {
                this.cargaTribFonteField = value;
            }
        }

        /// <remarks/>
        public string JustDed
        {
            get
            {
                return this.justDedField;
            }
            set
            {
                this.justDedField = value;
            }
        }

        public string ValTotal { get; set; }
        public string ValAliqISSRetido { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSServicoLocalPrestacao
    {

        private string serEndTpLgrField;

        private string serEndLgrField;

        private string serEndNumeroField;

        private string serEndComplementoField;

        private string serEndBairroField;

        private string serEndxMunField;

        private string serEndcMunField;

        private string serEndCepField;

        private string serEndSiglaUFField;

        /// <remarks/>
        public string SerEndTpLgr
        {
            get
            {
                return this.serEndTpLgrField;
            }
            set
            {
                this.serEndTpLgrField = value;
            }
        }

        /// <remarks/>
        public string SerEndLgr
        {
            get
            {
                return this.serEndLgrField;
            }
            set
            {
                this.serEndLgrField = value;
            }
        }

        /// <remarks/>
        public string SerEndNumero
        {
            get
            {
                return this.serEndNumeroField;
            }
            set
            {
                this.serEndNumeroField = value;
            }
        }

        /// <remarks/>
        public string SerEndComplemento
        {
            get
            {
                return this.serEndComplementoField;
            }
            set
            {
                this.serEndComplementoField = value;
            }
        }

        /// <remarks/>
        public string SerEndBairro
        {
            get
            {
                return this.serEndBairroField;
            }
            set
            {
                this.serEndBairroField = value;
            }
        }

        /// <remarks/>
        public string SerEndxMun
        {
            get
            {
                return this.serEndxMunField;
            }
            set
            {
                this.serEndxMunField = value;
            }
        }

        /// <remarks/>
        public string SerEndcMun
        {
            get
            {
                return this.serEndcMunField;
            }
            set
            {
                this.serEndcMunField = value;
            }
        }

        /// <remarks/>
        public string SerEndCep
        {
            get
            {
                return this.serEndCepField;
            }
            set
            {
                this.serEndCepField = value;
            }
        }

        /// <remarks/>
        public string SerEndSiglaUF
        {
            get
            {
                return this.serEndSiglaUFField;
            }
            set
            {
                this.serEndSiglaUFField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSTomador
    {

        private string tomaCNPJField;

        private string tomaCPFField;

        private string tomaIMField;

        private string tomaRazaoSocialField;

        private string tomatpLgrField;

        private string tomaEnderecoField;

        private string tomaNumeroField;

        private string tomaComplementoField;

        private string tomaBairroField;

        private string tomacMunField;

        private string tomaxMunField;

        private string tomaUFField;

        private string tomaPaisField;

        private string tomaCEPField;

        private string tomaTelefoneField;

        private string tomaTipoTelefoneField;

        private string tomaEmailField;

        private string tomaSiteField;

        private string tomaIEField;

        private string tomaIMEField;

        private string tomaSituacaoEspecialField;

        private string docTomadorEstrangeiroField;

        /// <remarks/>
        public string TomaCNPJ
        {
            get
            {
                return this.tomaCNPJField;
            }
            set
            {
                this.tomaCNPJField = value;
            }
        }

        /// <remarks/>
        public string TomaCPF
        {
            get
            {
                return this.tomaCPFField;
            }
            set
            {
                this.tomaCPFField = value;
            }
        }

        /// <remarks/>
        public string TomaIM
        {
            get
            {
                return this.tomaIMField;
            }
            set
            {
                this.tomaIMField = value;
            }
        }

        /// <remarks/>
        public string TomaRazaoSocial
        {
            get
            {
                return this.tomaRazaoSocialField;
            }
            set
            {
                this.tomaRazaoSocialField = value;
            }
        }

        /// <remarks/>
        public string TomatpLgr
        {
            get
            {
                return this.tomatpLgrField;
            }
            set
            {
                this.tomatpLgrField = value;
            }
        }

        /// <remarks/>
        public string TomaEndereco
        {
            get
            {
                return this.tomaEnderecoField;
            }
            set
            {
                this.tomaEnderecoField = value;
            }
        }

        /// <remarks/>
        public string TomaNumero
        {
            get
            {
                return this.tomaNumeroField;
            }
            set
            {
                this.tomaNumeroField = value;
            }
        }

        /// <remarks/>
        public string TomaComplemento
        {
            get
            {
                return this.tomaComplementoField;
            }
            set
            {
                this.tomaComplementoField = value;
            }
        }

        /// <remarks/>
        public string TomaBairro
        {
            get
            {
                return this.tomaBairroField;
            }
            set
            {
                this.tomaBairroField = value;
            }
        }

        /// <remarks/>
        public string TomacMun
        {
            get
            {
                return this.tomacMunField;
            }
            set
            {
                this.tomacMunField = value;
            }
        }

        /// <remarks/>
        public string TomaxMun
        {
            get
            {
                return this.tomaxMunField;
            }
            set
            {
                this.tomaxMunField = value;
            }
        }

        /// <remarks/>
        public string TomaUF
        {
            get
            {
                return this.tomaUFField;
            }
            set
            {
                this.tomaUFField = value;
            }
        }

        /// <remarks/>
        public string TomaPais
        {
            get
            {
                return this.tomaPaisField;
            }
            set
            {
                this.tomaPaisField = value;
            }
        }

        /// <remarks/>
        public string TomaCEP
        {
            get
            {
                return this.tomaCEPField;
            }
            set
            {
                this.tomaCEPField = value;
            }
        }

        /// <remarks/>
        public string TomaTelefone
        {
            get
            {
                return this.tomaTelefoneField;
            }
            set
            {
                this.tomaTelefoneField = value;
            }
        }

        /// <remarks/>
        public string TomaTipoTelefone
        {
            get
            {
                return this.tomaTipoTelefoneField;
            }
            set
            {
                this.tomaTipoTelefoneField = value;
            }
        }

        /// <remarks/>
        public string TomaEmail
        {
            get
            {
                return this.tomaEmailField;
            }
            set
            {
                this.tomaEmailField = value;
            }
        }

        /// <remarks/>
        public string TomaSite
        {
            get
            {
                return this.tomaSiteField;
            }
            set
            {
                this.tomaSiteField = value;
            }
        }

        /// <remarks/>
        public string TomaIE
        {
            get
            {
                return this.tomaIEField;
            }
            set
            {
                this.tomaIEField = value;
            }
        }

        /// <remarks/>
        public string TomaIME
        {
            get
            {
                return this.tomaIMEField;
            }
            set
            {
                this.tomaIMEField = value;
            }
        }

        /// <remarks/>
        public string TomaSituacaoEspecial
        {
            get
            {
                return this.tomaSituacaoEspecialField;
            }
            set
            {
                this.tomaSituacaoEspecialField = value;
            }
        }

        /// <remarks/>
        public string DocTomadorEstrangeiro
        {
            get
            {
                return this.docTomadorEstrangeiroField;
            }
            set
            {
                this.docTomadorEstrangeiroField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSIntermServico
    {

        private string intermRazaoSocialField;

        private string intermCNPJField;

        private string intermCPFField;

        private string intermIMField;

        private string intermEmailField;

        private string intermEnderecoField;

        private string intermNumeroField;

        private string intermComplementoField;

        private string intermBairroField;

        private string intermCepField;

        private string intermCmunField;

        private string intermXmunField;

        private string intermFoneField;

        private string itermIEField;

        /// <remarks/>
        public string IntermRazaoSocial
        {
            get
            {
                return this.intermRazaoSocialField;
            }
            set
            {
                this.intermRazaoSocialField = value;
            }
        }

        /// <remarks/>
        public string IntermCNPJ
        {
            get
            {
                return this.intermCNPJField;
            }
            set
            {
                this.intermCNPJField = value;
            }
        }

        /// <remarks/>
        public string IntermCPF
        {
            get
            {
                return this.intermCPFField;
            }
            set
            {
                this.intermCPFField = value;
            }
        }

        /// <remarks/>
        public string IntermIM
        {
            get
            {
                return this.intermIMField;
            }
            set
            {
                this.intermIMField = value;
            }
        }

        /// <remarks/>
        public string IntermEmail
        {
            get
            {
                return this.intermEmailField;
            }
            set
            {
                this.intermEmailField = value;
            }
        }

        /// <remarks/>
        public string IntermEndereco
        {
            get
            {
                return this.intermEnderecoField;
            }
            set
            {
                this.intermEnderecoField = value;
            }
        }

        /// <remarks/>
        public string IntermNumero
        {
            get
            {
                return this.intermNumeroField;
            }
            set
            {
                this.intermNumeroField = value;
            }
        }

        /// <remarks/>
        public string IntermComplemento
        {
            get
            {
                return this.intermComplementoField;
            }
            set
            {
                this.intermComplementoField = value;
            }
        }

        /// <remarks/>
        public string IntermBairro
        {
            get
            {
                return this.intermBairroField;
            }
            set
            {
                this.intermBairroField = value;
            }
        }

        /// <remarks/>
        public string IntermCep
        {
            get
            {
                return this.intermCepField;
            }
            set
            {
                this.intermCepField = value;
            }
        }

        /// <remarks/>
        public string IntermCmun
        {
            get
            {
                return this.intermCmunField;
            }
            set
            {
                this.intermCmunField = value;
            }
        }

        /// <remarks/>
        public string IntermXmun
        {
            get
            {
                return this.intermXmunField;
            }
            set
            {
                this.intermXmunField = value;
            }
        }

        /// <remarks/>
        public string IntermFone
        {
            get
            {
                return this.intermFoneField;
            }
            set
            {
                this.intermFoneField = value;
            }
        }

        /// <remarks/>
        public string ItermIE
        {
            get
            {
                return this.itermIEField;
            }
            set
            {
                this.itermIEField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSConstCivil
    {

        private string codObraField;

        private string artField;

        private string obraLogField;

        private string obraComplField;

        private string obraNumeroField;

        private string obraBairroField;

        private string obraCEPField;

        private string obraMunField;

        private string obraUFField;

        private string obraPaisField;

        private string obraCEIField;

        private string obraMatriculaField;

        private string obraValRedBCField;

        private string obraTipoField;

        private string obraNumEncapsulamentoField;

        private EnvioRPSConstCivilListaMaterial listaMaterialField;

        /// <remarks/>
        public string CodObra
        {
            get
            {
                return this.codObraField;
            }
            set
            {
                this.codObraField = value;
            }
        }

        /// <remarks/>
        public string Art
        {
            get
            {
                return this.artField;
            }
            set
            {
                this.artField = value;
            }
        }

        /// <remarks/>
        public string ObraLog
        {
            get
            {
                return this.obraLogField;
            }
            set
            {
                this.obraLogField = value;
            }
        }

        /// <remarks/>
        public string ObraCompl
        {
            get
            {
                return this.obraComplField;
            }
            set
            {
                this.obraComplField = value;
            }
        }

        /// <remarks/>
        public string ObraNumero
        {
            get
            {
                return this.obraNumeroField;
            }
            set
            {
                this.obraNumeroField = value;
            }
        }

        /// <remarks/>
        public string ObraBairro
        {
            get
            {
                return this.obraBairroField;
            }
            set
            {
                this.obraBairroField = value;
            }
        }

        /// <remarks/>
        public string ObraCEP
        {
            get
            {
                return this.obraCEPField;
            }
            set
            {
                this.obraCEPField = value;
            }
        }

        /// <remarks/>
        public string ObraMun
        {
            get
            {
                return this.obraMunField;
            }
            set
            {
                this.obraMunField = value;
            }
        }

        /// <remarks/>
        public string ObraUF
        {
            get
            {
                return this.obraUFField;
            }
            set
            {
                this.obraUFField = value;
            }
        }

        /// <remarks/>
        public string ObraPais
        {
            get
            {
                return this.obraPaisField;
            }
            set
            {
                this.obraPaisField = value;
            }
        }

        /// <remarks/>
        public string ObraCEI
        {
            get
            {
                return this.obraCEIField;
            }
            set
            {
                this.obraCEIField = value;
            }
        }

        /// <remarks/>
        public string ObraMatricula
        {
            get
            {
                return this.obraMatriculaField;
            }
            set
            {
                this.obraMatriculaField = value;
            }
        }

        /// <remarks/>
        public string ObraValRedBC
        {
            get
            {
                return this.obraValRedBCField;
            }
            set
            {
                this.obraValRedBCField = value;
            }
        }

        /// <remarks/>
        public string ObraTipo
        {
            get
            {
                return this.obraTipoField;
            }
            set
            {
                this.obraTipoField = value;
            }
        }

        public string ObraNumEncapsulamento
        {
            get
            {
                return this.obraNumEncapsulamentoField;
            }
            set
            {
                this.obraNumEncapsulamentoField = value;
            }
        }
        /// <remarks/>
        public EnvioRPSConstCivilListaMaterial ListaMaterial
        {
            get
            {
                return this.listaMaterialField;
            }
            set
            {
                this.listaMaterialField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSConstCivilListaMaterial
    {

        private EnvioRPSConstCivilListaMaterialMaterial materialField;

        /// <remarks/>
        public EnvioRPSConstCivilListaMaterialMaterial Material
        {
            get
            {
                return this.materialField;
            }
            set
            {
                this.materialField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSConstCivilListaMaterialMaterial
    {

        private string matCodigoField;

        private string matDescricaoField;

        private string matUndMedCodigoField;

        private string matUndMedSiglaField;

        private string matQuantidadeField;

        private string matVlrTotalField;

        /// <remarks/>
        public string MatCodigo
        {
            get
            {
                return this.matCodigoField;
            }
            set
            {
                this.matCodigoField = value;
            }
        }

        /// <remarks/>
        public string MatDescricao
        {
            get
            {
                return this.matDescricaoField;
            }
            set
            {
                this.matDescricaoField = value;
            }
        }

        /// <remarks/>
        public string MatUndMedCodigo
        {
            get
            {
                return this.matUndMedCodigoField;
            }
            set
            {
                this.matUndMedCodigoField = value;
            }
        }

        /// <remarks/>
        public string MatUndMedSigla
        {
            get
            {
                return this.matUndMedSiglaField;
            }
            set
            {
                this.matUndMedSiglaField = value;
            }
        }

        /// <remarks/>
        public string MatQuantidade
        {
            get
            {
                return this.matQuantidadeField;
            }
            set
            {
                this.matQuantidadeField = value;
            }
        }

        /// <remarks/>
        public string MatVlrTotal
        {
            get
            {
                return this.matVlrTotalField;
            }
            set
            {
                this.matVlrTotalField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSListaDed
    {

        private EnvioRPSListaDedDed dedField;

        /// <remarks/>
        public EnvioRPSListaDedDed Ded
        {
            get
            {
                return this.dedField;
            }
            set
            {
                this.dedField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSListaDedDed
    {

        private string dedSeqField;

        private string dedValPerField;

        private string dedTipoField;

        private string dedCNPJRefField;

        private string dedCPFRefField;

        private string dednNFRefField;

        private string dedvlTotRefField;

        private string dedPerField;

        private string dedValorField;

        private string dedQtdeField;

        private string dedValUnitField;

        private string dedDescricaoField;

        /// <remarks/>
        public string DedSeq
        {
            get
            {
                return this.dedSeqField;
            }
            set
            {
                this.dedSeqField = value;
            }
        }

        /// <remarks/>
        public string DedValPer
        {
            get
            {
                return this.dedValPerField;
            }
            set
            {
                this.dedValPerField = value;
            }
        }

        /// <remarks/>
        public string DedTipo
        {
            get
            {
                return this.dedTipoField;
            }
            set
            {
                this.dedTipoField = value;
            }
        }

        /// <remarks/>
        public string DedCNPJRef
        {
            get
            {
                return this.dedCNPJRefField;
            }
            set
            {
                this.dedCNPJRefField = value;
            }
        }

        /// <remarks/>
        public string DedCPFRef
        {
            get
            {
                return this.dedCPFRefField;
            }
            set
            {
                this.dedCPFRefField = value;
            }
        }

        /// <remarks/>
        public string DednNFRef
        {
            get
            {
                return this.dednNFRefField;
            }
            set
            {
                this.dednNFRefField = value;
            }
        }

        /// <remarks/>
        public string DedvlTotRef
        {
            get
            {
                return this.dedvlTotRefField;
            }
            set
            {
                this.dedvlTotRefField = value;
            }
        }

        /// <remarks/>
        public string DedPer
        {
            get
            {
                return this.dedPerField;
            }
            set
            {
                this.dedPerField = value;
            }
        }

        /// <remarks/>
        public string DedValor
        {
            get
            {
                return this.dedValorField;
            }
            set
            {
                this.dedValorField = value;
            }
        }

        /// <remarks/>
        public string DedQtde
        {
            get
            {
                return this.dedQtdeField;
            }
            set
            {
                this.dedQtdeField = value;
            }
        }

        /// <remarks/>
        public string DedValUnit
        {
            get
            {
                return this.dedValUnitField;
            }
            set
            {
                this.dedValUnitField = value;
            }
        }

        /// <remarks/>
        public string DedDescricao
        {
            get
            {
                return this.dedDescricaoField;
            }
            set
            {
                this.dedDescricaoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EnvioRPSTransportadora
    {

        private string traNomeField;

        private string traCPFCNPJField;

        private string traIEField;

        private string traPlacaField;

        private string traEndField;

        private string traMunField;

        private string traUFField;

        private string traPaisField;

        private string traTipoFreteField;

        /// <remarks/>
        public string TraNome
        {
            get
            {
                return this.traNomeField;
            }
            set
            {
                this.traNomeField = value;
            }
        }

        /// <remarks/>
        public string TraCPFCNPJ
        {
            get
            {
                return this.traCPFCNPJField;
            }
            set
            {
                this.traCPFCNPJField = value;
            }
        }

        /// <remarks/>
        public string TraIE
        {
            get
            {
                return this.traIEField;
            }
            set
            {
                this.traIEField = value;
            }
        }

        /// <remarks/>
        public string TraPlaca
        {
            get
            {
                return this.traPlacaField;
            }
            set
            {
                this.traPlacaField = value;
            }
        }

        /// <remarks/>
        public string TraEnd
        {
            get
            {
                return this.traEndField;
            }
            set
            {
                this.traEndField = value;
            }
        }

        /// <remarks/>
        public string TraMun
        {
            get
            {
                return this.traMunField;
            }
            set
            {
                this.traMunField = value;
            }
        }

        /// <remarks/>
        public string TraUF
        {
            get
            {
                return this.traUFField;
            }
            set
            {
                this.traUFField = value;
            }
        }

        /// <remarks/>
        public string TraPais
        {
            get
            {
                return this.traPaisField;
            }
            set
            {
                this.traPaisField = value;
            }
        }

        /// <remarks/>
        public string TraTipoFrete
        {
            get
            {
                return this.traTipoFreteField;
            }
            set
            {
                this.traTipoFreteField = value;
            }
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Migrate.NFSe.DTO
{
    public class RetornoValidacao
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

            private string docNumeroField;

            private string docSerieField;

            private string docChaAcessoField;

            private string docProtocoloField;

            private string docSitCodigoField;

            private string docSitDescricaoField;

            private string docXMLField;

            private string docXMLLinkField;

            private string docPDFField;

            private string docPDFLinkField;

            private string dhRecbtoField;

            private string nTEmVigorField;

            private string versaoQRCodeField;

            private string imprimirUmaViaContingenciaField;

            private string docImpPrefeituraField;

            private string docStatusField;

            private string certHoraVencimentoField;

            private DocumentoResumo resumoField;

            private string eventosField;

            private DocumentoNFSe nFSeField;

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
            public string DocSitCodigo
            {
                get
                {
                    return this.docSitCodigoField;
                }
                set
                {
                    this.docSitCodigoField = value;
                }
            }

            /// <remarks/>
            public string DocSitDescricao
            {
                get
                {
                    return this.docSitDescricaoField;
                }
                set
                {
                    this.docSitDescricaoField = value;
                }
            }

            /// <remarks/>
            public string DocXML
            {
                get
                {
                    return this.docXMLField;
                }
                set
                {
                    this.docXMLField = value;
                }
            }

            /// <remarks/>
            public string DocXMLLink
            {
                get
                {
                    return this.docXMLLinkField;
                }
                set
                {
                    this.docXMLLinkField = value;
                }
            }

            /// <remarks/>
            public string DocPDF
            {
                get
                {
                    return this.docPDFField;
                }
                set
                {
                    this.docPDFField = value;
                }
            }

            /// <remarks/>
            public string DocPDFLink
            {
                get
                {
                    return this.docPDFLinkField;
                }
                set
                {
                    this.docPDFLinkField = value;
                }
            }

            /// <remarks/>
            public string dhRecbto
            {
                get
                {
                    return this.dhRecbtoField;
                }
                set
                {
                    this.dhRecbtoField = value;
                }
            }

            /// <remarks/>
            public string NTEmVigor
            {
                get
                {
                    return this.nTEmVigorField;
                }
                set
                {
                    this.nTEmVigorField = value;
                }
            }

            /// <remarks/>
            public string VersaoQRCode
            {
                get
                {
                    return this.versaoQRCodeField;
                }
                set
                {
                    this.versaoQRCodeField = value;
                }
            }

            /// <remarks/>
            public string ImprimirUmaViaContingencia
            {
                get
                {
                    return this.imprimirUmaViaContingenciaField;
                }
                set
                {
                    this.imprimirUmaViaContingenciaField = value;
                }
            }

            /// <remarks/>
            public string DocImpPrefeitura
            {
                get
                {
                    return this.docImpPrefeituraField;
                }
                set
                {
                    this.docImpPrefeituraField = value;
                }
            }

            /// <remarks/>
            public string DocStatus
            {
                get
                {
                    return this.docStatusField;
                }
                set
                {
                    this.docStatusField = value;
                }
            }

            /// <remarks/>
            public string CertHoraVencimento
            {
                get
                {
                    return this.certHoraVencimentoField;
                }
                set
                {
                    this.certHoraVencimentoField = value;
                }
            }

            /// <remarks/>
            public DocumentoResumo Resumo
            {
                get
                {
                    return this.resumoField;
                }
                set
                {
                    this.resumoField = value;
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
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class DocumentoResumo
        {

            private System.DateTime docDataEmissaoField;

            private string docFusoHorarioField;

            private string docDigestValueField;

            private string docNomeEmitenteField;

            private string docCNPJDestinatarioField;

            private string docNomeDestinatarioField;

            private string docVlrTotalField;

            /// <remarks/>
            public System.DateTime DocDataEmissao
            {
                get
                {
                    return this.docDataEmissaoField;
                }
                set
                {
                    this.docDataEmissaoField = value;
                }
            }

            /// <remarks/>
            public string DocFusoHorario
            {
                get
                {
                    return this.docFusoHorarioField;
                }
                set
                {
                    this.docFusoHorarioField = value;
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
            public string DocNomeEmitente
            {
                get
                {
                    return this.docNomeEmitenteField;
                }
                set
                {
                    this.docNomeEmitenteField = value;
                }
            }

            /// <remarks/>
            public string DocCNPJDestinatario
            {
                get
                {
                    return this.docCNPJDestinatarioField;
                }
                set
                {
                    this.docCNPJDestinatarioField = value;
                }
            }

            /// <remarks/>
            public string DocNomeDestinatario
            {
                get
                {
                    return this.docNomeDestinatarioField;
                }
                set
                {
                    this.docNomeDestinatarioField = value;
                }
            }

            /// <remarks/>
            public string DocVlrTotal
            {
                get
                {
                    return this.docVlrTotalField;
                }
                set
                {
                    this.docVlrTotalField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
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


    }
}

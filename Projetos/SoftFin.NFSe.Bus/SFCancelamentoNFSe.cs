using SoftFin.NFSe.Business.DTO;
using SoftFin.NFSe.DTO;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFSe.Business
{
    public class SFCancelamentoNFSe
    {
        public DTORetornoNFEs Execute(DTONotaFiscal dTONotaFiscal, System.Security.Cryptography.X509Certificates.X509Certificate2 cert, string caminhoXML)
        {
            try
            {
                var retorno = new DTORetornoNFEs();

                switch (dTONotaFiscal.municipio_desc)
                {
                    case "São Paulo":
                        retorno = new SoftFin.NFSe.SaoPaulo.Bussiness.CancelaNFe().Execute(dTONotaFiscal, cert);
                        break;
                    case "Guarulhos":
                        retorno = new SoftFin.NFSe.Guarulhos.Bussiness.CancelaNFe().Execute(dTONotaFiscal, cert, caminhoXML);
                        break;
                    default:
                        throw new Exception("Cidade não configurada.");
                }
                retorno.tipo = "Envio";
                return retorno;
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}

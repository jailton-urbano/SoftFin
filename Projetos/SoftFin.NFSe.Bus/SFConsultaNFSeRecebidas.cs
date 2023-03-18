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
    public class SFConsultaNFSeRecebidas
    {
        public DTORetornoNFEs Execute(DTONotaFiscal dTOConsultaNFSe, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            try
            {
                var retorno = new DTORetornoNFEs();
                switch (dTOConsultaNFSe.municipio_desc)
                {
                    case "São Paulo":
                        retorno = new SoftFin.NFSe.SaoPaulo.Bussiness.ConsultaNFeRecebidas().Execute(dTOConsultaNFSe, cert);
                        break;
                    case "Guarulhos":
                        retorno = new SoftFin.NFSe.Guarulhos.Bussiness.ConsultaNFeRecebidas().Execute(dTOConsultaNFSe, cert);
                        break;
                    default:
                        throw new Exception("Cidade não configurada.");
                }
                retorno.tipo = "ConsultaRecebida";
                return retorno;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}

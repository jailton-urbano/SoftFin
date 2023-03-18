using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.Business
{
    public class EnvioNFe
    {
        public DTORetornoNFe Execute(
                DTONfe DTONfe,
                System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
                bool simulaXML = false,
                string caminhoXSD = "",
                string urlServico = null,
                string urlServicoRet = null)
        {
            try
            {
                var retorno = new DTORetornoNFe();
                switch (DTONfe.InfNFe.Emi.EnderEmit.xMun)
                {
                    case "São Paulo":
                        retorno = new SoftFin.Sefaz.NFeAutorizacao().Execute(DTONfe, cert,simulaXML, caminhoXSD,urlServico,urlServicoRet);
                        break;
                    default:
                        throw new Exception("Cidade não configurada.");
                }
                //retorno.tipo = "Envio";
                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

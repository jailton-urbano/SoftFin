using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.Business
{
    public class ConsultaNFe
    {
        public DTORetornoNFe Execute(
            string chaveAcesso,

        System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string caminhoXSD, 
            string urlServico)
        
        {
            try
            {
                var retorno = new SoftFin.Sefaz.NFeConsultaProtocolo().Execute(chaveAcesso, cert, urlServico);
                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

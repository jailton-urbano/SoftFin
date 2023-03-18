using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.Business
{
    public class DownloadNFe
    {
        public DTORetornoNFe Execute(
            string chaveAcesso,
            string cnpjDestinatario,
        System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string caminhoXSD, string urlServico)
        
        {
            try
            {
                var retorno = new SoftFin.Sefaz.NFeDownload().Execute(chaveAcesso, cnpjDestinatario, cert, caminhoXSD, urlServico);
                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

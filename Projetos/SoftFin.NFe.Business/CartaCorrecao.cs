using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.Business
{
    public class CartaCorrecao
    {
        public DTORetornoNFe Execute(
        DTOLogEvento dTOEvento,
        System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string caminhoXSD, string urlServico)
        
        {
            try
            {
                var retorno = new DTORetornoNFe();
                switch (dTOEvento.cOrgao)
                {
                    case "35":
                        retorno = new SoftFin.Sefaz.NFeCartaCorrecao().Execute(dTOEvento, cert, caminhoXSD, urlServico);
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

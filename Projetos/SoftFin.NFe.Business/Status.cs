using SoftFin.NFe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFe.Business
{
    public class Status
    {
        public DTORetornoNFe Execute(
                string codMunicipio,
                System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
                string urlServico = null)
        {
            try
            {
                var retorno = new DTORetornoNFe();
                switch (codMunicipio)
                {
                    case "35":
                        retorno = new SoftFin.Sefaz.NFeStatus().Execute(codMunicipio, cert, urlServico);
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

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
    public class SFEnvioLoteRPS
    {
        public DTORetornoNFEs Execute(
            DTONotaFiscal dTOEnvioLoteRPS, 
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert,
            string arquivoxml)
        {
            try
            {
                var retorno = new DTORetornoNFEs();
                switch (dTOEnvioLoteRPS.municipio_desc)
                {
                    case "São Paulo":
                        retorno = new  SoftFin.NFSe.Business.EnvioLoteRPS.Cidade.SaoPaulo().Execute(dTOEnvioLoteRPS, cert);
                        break;
                    case "Guarulhos": case "Embu das Artes":
                        retorno = new SoftFin.NFSe.Business.EnvioLoteRPS.Cidade.Guarulhos().Execute(dTOEnvioLoteRPS, cert, arquivoxml);
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

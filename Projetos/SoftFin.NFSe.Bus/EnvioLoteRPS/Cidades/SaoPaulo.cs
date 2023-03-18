using SoftFin.NFSe.Business.DTO;
using SoftFin.NFSe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFSe.Business.EnvioLoteRPS.Cidade
{
    public class SaoPaulo
    {
        public DTORetornoNFEs Execute(DTONotaFiscal dTOEnvioLoteRPS, System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            var serviceEnvio = new SoftFin.NFSe.SaoPaulo.Bussiness.EnviarLoteRPS();
            var resultado = serviceEnvio.Executar(dTOEnvioLoteRPS, cert);
            return resultado;
        }

    }
}

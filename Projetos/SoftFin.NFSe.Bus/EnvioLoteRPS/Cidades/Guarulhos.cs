using SoftFin.NFSe.Business.DTO;
using SoftFin.NFSe.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.NFSe.Business.EnvioLoteRPS.Cidade
{
    public class Guarulhos
    {
        public DTORetornoNFEs Execute(DTONotaFiscal dTOEnvioLoteRPS, System.Security.Cryptography.X509Certificates.X509Certificate2 cert, string arquivoxml)
        {
            var serviceEnvio = new SoftFin.NFSe.Guarulhos.Bussiness.EnviarLoteRPS();
            var resultado = serviceEnvio.Executar(dTOEnvioLoteRPS, cert, arquivoxml);
            return resultado;
        }


    }
}

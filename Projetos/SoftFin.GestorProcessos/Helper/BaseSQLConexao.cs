using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Helper
{
    public class BaseSQLConexao
    {
        public string _stringConecao = "";
        public BaseSQLConexao()
        {
            _stringConecao = ConfigurationManager.AppSettings["ConexaoGP"];
        }
    }
}
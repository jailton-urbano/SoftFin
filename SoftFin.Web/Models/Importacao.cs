using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftFin.Web.Models
{
    public class ImportacaoArquivo 
    {
        public ImportacaoArquivo()
        {
            LinhasErros = new Dictionary<string, string>();
        }

        public string Descricao { get; set; }
        public int TotalLinhas { get; set; }
        public int TotalImportadas { get; set; }

        public int TotalErros { get; set; }

        public Dictionary<string, string> LinhasErros { get; set; }



        public string Situacao { get; set; }
    }
}

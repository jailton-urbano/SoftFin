using SoftFin.Web.Negocios;
using SoftFin.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SoftFin.API
{
    
    public class EmailTesteController: ApiController 
    {
        // GET api/<controller>
        public void Get(string email)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplate/");
            var arquivohmtl = Path.Combine(path, "Email01.html");
            string readText = File.ReadAllText(arquivohmtl);
            var emailobj = new Email();
            emailobj.EnviarMensagem(email, "Resumo Semanal", readText, true);
        }


    }
}
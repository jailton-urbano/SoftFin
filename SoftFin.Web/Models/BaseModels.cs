using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace SoftFin.Web.Models
{
    public class BaseModels
    {
        public virtual List<string> Validar(System.Web.Mvc.ModelStateDictionary ModelState = null)
        {
            var erros = new List<string>();
            if (ModelState != null)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                erros.AddRange(from a in allErrors select ((a.ErrorMessage != null) ? a.ErrorMessage : a.Exception.ToString()));
            }
            return erros;
        }


    }
}
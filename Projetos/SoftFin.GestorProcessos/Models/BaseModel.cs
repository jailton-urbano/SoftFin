using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftFin.GestorProcessos.Models
{
    public class BaseModel
    {
        public virtual List<string> Validar(System.Web.Mvc.ModelStateDictionary ModelState = null)
        {
            var erros = new List<string>();
            if (ModelState != null)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
                erros.AddRange(from a in allErrors select ((a.ErrorMessage != null) ? a.ErrorMessage : a.Exception.ToString()));

                for (int i = 0; i < erros.Count(); i++)
                {
                    erros[i] = (i + 1).ToString() + "-" + erros[i];
                }
            }
            return erros;
        }
    }
}
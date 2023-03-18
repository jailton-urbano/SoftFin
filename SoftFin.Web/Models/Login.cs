using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SoftFin.Web.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Campo código obrigatório")]
        public string codigo { get; set; }


        public string senha { get; set; }

        public string remember { get; set; }
    }
}

using SoftFin.Web.Classes;
using SoftFin.Web.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class RegistroPontoView
    {

        public int id { get; set; }

        [Display(Name = "Data Lançamento"), Required(ErrorMessage = "*")]
        public DateTime data { get; set; }

        [Display(Name = "Ponto1")]
        public string ponto1 { get; set; }
        [Display(Name = "Ponto2")]
        public string ponto2 { get; set; }
        [Display(Name = "Ponto3")]
        public string ponto3 { get; set; }
        [Display(Name = "Ponto4")]
        public string ponto4 { get; set; }
        [Display(Name = "Ponto5")]
        public string ponto5 { get; set; }
        [Display(Name = "Ponto6")]
        public string ponto6 { get; set; }
        [Display(Name = "Ponto7")]
        public string ponto7 { get; set; }
        [Display(Name = "Ponto8")]
        public string ponto8 { get; set; }

        [Display(Name = "Comentários"), MaxLength(400)]
        public string comentarios { get; set; }
    }



    public class RegistroPontoView2
    {

        [Display(Name = "Data Lançamento"), Required(ErrorMessage = "*")]
        public DateTime data { get; set; }

        [Display(Name = "Ponto")]
        public string ponto { get; set; }

        [Display(Name = "Comentários"), MaxLength(400)]
        public string comentarios { get; set; }
    }

}
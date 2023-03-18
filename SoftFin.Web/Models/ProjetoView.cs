using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using SoftFin.Web.Classes;
using SoftFin.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace SoftFin.Web.Models
{
    public class ProjetoUsuarioView
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public string selecionado { get; set; }

        [Display(Name = "Categoria")]
        public int? categoria_id { get; set; }

    }

    public class ProjetoView
    {

        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [JsonIgnore,ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [Display(Name = "Projeto Pai")]
        public int? projeto_id { get; set; }

        [Display(Name = "Projeto Pai"), ForeignKey("projeto_id")]
        public virtual Projeto ProjetoPai { get; set; }

        [Display(Name = "Codigo Projeto"), Required(ErrorMessage = "*")]
        public string codigoProjeto { get; set; }

        [Display(Name = "Nome Projeto"), Required(ErrorMessage = "*")]
        public string nomeProjeto { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime dataInicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime dataFinal { get; set; }
       
        [Display(Name = "Total Horas"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal totalHoras { get; set; }

        [Display(Name = "Contrato Item")]
        public int? ContratoItem_id { get; set; }

        [JsonIgnore,ForeignKey("ContratoItem_id")]
        public virtual ContratoItem ContratoItem { get; set; }

        public virtual List<ProjetoUsuarioView> ProjetoUsuarioViews { get; set; }
  
    }
}
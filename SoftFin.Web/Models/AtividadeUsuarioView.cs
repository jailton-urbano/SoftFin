using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class AtividadeUsuarioView
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }

        [Display(Name = "Descrição")]
        public string descricao { get; set; }
        [Display(Name = "Sequencia")]
        public int sequencia { get; set; }
        [Display(Name = "Predescessora")]
        public int? predescessora_id { get; set; }
        [Display(Name = "Sucessora")]
        public int? sucessora_id { get; set; }

        [Display(Name = "Projeto")]
        public int projeto_id { get; set; }

        public Projeto Projeto { get; set; }

        [Display(Name = "Data Inicial")]
        public DateTime? DataInicial { get; set; }

        [Display(Name = "Data Final")]
        public DateTime? DataFinal { get; set; }

        [Display(Name = "Quantidade Horas"), Required(ErrorMessage = "*")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        public decimal qtdHoras { get; set; }

        [Display(Name = "Atividade")]
        public int atividade_id { get; set; }

        [Display(Name = "Usuario")]
        public int usuario_id { get; set; }

        public virtual Usuario Usuario { get; set; }


        [Display(Name = "Recursos Atribuidos")]
        public List<object> ListaUsuarioVal { get; set; }

        [Display(Name = "Recursos Atribuidos")]
        public Dictionary<string, bool> ListaUsuario { get; set; }
    }




}
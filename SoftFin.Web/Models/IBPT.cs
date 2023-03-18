using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftFin.Web.Models
{
    public class IBPT: BaseModels
    {
        [Key]
        public int Id { get; set; }
        public string UF { get; set; }
        [MaxLength(9)]
        public string Codigo { get; set; }
        [MaxLength(1)]
        public string Ex { get; set; }
        [MaxLength(1)]
        public string Tipo { get; set; }
        
        [MaxLength(500)]
        public string Descricao { get; set; }   
            	
        public decimal Nacionalfederal { get; set; }                   	
                
        public decimal Importadosfederal { get; set; }                   	
        public decimal Estadual { get; set; }                   	
        public decimal Municipal { get; set; }                   	
        
        public DateTime Vigenciainicio { get; set; }      	                    	
                        	
        public DateTime Vigenciafim { get; set; }      	                    	

        [MaxLength(15)]
        public string Versao { get; set; }                       	

        [MaxLength(15)]
        public string Fonte { get; set; }   
        
       
    }
}
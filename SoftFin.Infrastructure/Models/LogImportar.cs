using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;
using Newtonsoft.Json;
using SoftFin.InfrastructureHelper;

namespace SoftFin.Infrastructure.Models
{
    public class LogImportar
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(15)]
        public string CodigoSistema { get; set; }
        public string Estabelecimento { get; set; }
        public DateTime Data { get; set; }
        public Guid Agrupador { get; set; }

        [MaxLength(400)]
        public string Usuario { get; set; }
        
        [MaxLength(15)]
        public string Tipo { get; set; }

        [MaxLength(50)]
        public string Ip { get; set; }

        
        [MaxLength(4000)]
        public string Descricao { get; set; }

        public string Json { get; set; }

        [MaxLength(200)]
        public string Registro { get; set; }
        public string Mensagem { get; set; }

        [MaxLength(50)]
        public string IPLogado { get; set; }

        public void Incluir()
        {
            Incluir(this);
        }
        public void Incluir(LogImportar obj)
        {
            var db = new DBINControle();
            db.Set<LogImportar>().Add(obj);
            db.SaveChanges();
        }

        public LogImportar ObterPorId(int id)
        {
            return ObterPorId(id, null);
        }
        public LogImportar ObterPorId(int id, DBINControle db)
        {
            if (db == null)
                db = new DBINControle();

            return db.LogImportacao.Where(x => x.Id == id).FirstOrDefault();
        }
        public List<LogImportar> ObterTodosPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var db = new DBINControle();
            return db.LogImportacao.Where(x => DbFunctions.TruncateTime(x.Data) >= dataInicial
                                        && DbFunctions.TruncateTime(x.Data) <= dataFinal)
                                        .ToList();
        }


    }
}
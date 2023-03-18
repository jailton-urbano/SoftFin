using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoftFin.Web.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SoftFin.Web.Negocios;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Diagnostics;
using SoftFin.Web.Regras;

namespace SoftFin.Web.Models
{
    public class LogMudanca
    {
        [Key]
        public int id { get; set; }
        public DateTime data { get; set; }
        public string metodo { get; set; }
        public string mensagem { get; set; }
        public string registroNovo { get; set; }
        public string registroAnterior { get; set; }

        [Display(Name = "Usuário"), Required(ErrorMessage = "*")]
        public int usuario_id { get; set; }
        [ForeignKey("usuario_id")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Estabelecimento"), Required(ErrorMessage = "*")]
        public int estabelecimento_id { get; set; }
        [ForeignKey("estabelecimento_id")]
        public virtual Estabelecimento Estabelecimento { get; set; }

        [MaxLength(4000)]
        public string cadeiaMetodos { get; set; }

        [MaxLength(500)]
        public string metodoChamador { get; set; }

        [MaxLength(80)]
        public string IP { get; set; }

        

        
                      

        public bool Incluir(
            object vRegistroNovo, 
            object vRegistroAnterior, 
            string mens, 
            DbControle banco, 
            ParamBase paramBase)
        {
            try
            {

                if (paramBase.estab_id == 0)
                    throw new Exception("Not a estalecimento value is 0 (Zero)");

                JsonSerializerSettings settings = new JsonSerializerSettings();
                
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                settings.CheckAdditionalContent = false;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                


                //Identifica Estabelecimento Logado
                registroNovo = JsonConvert.SerializeObject(vRegistroNovo, settings);
                StackTrace stackTrace = new StackTrace();

                metodo = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name +
                    "." + stackTrace.GetFrame(1).GetMethod().Name;

                for (int i = 0; i < stackTrace.FrameCount; i++)
                {
                    if (stackTrace.GetFrame(i).GetMethod().ReflectedType != null)
                    {
                        cadeiaMetodos += stackTrace.GetFrame(i).GetMethod().ReflectedType.Name +
                        "." + stackTrace.GetFrame(i).GetMethod().Name + "=>";

                        if (stackTrace.GetFrame(i).GetMethod().ReflectedType.Name.Contains("Controller"))
                        {
                            break;
                        }
                    }
                }

                for (int i = 0; i < stackTrace.FrameCount; i++)
                {
                    if (stackTrace.GetFrame(i).GetMethod().ReflectedType != null)
                    {


                        if (stackTrace.GetFrame(i).GetMethod().ReflectedType.Name.Contains("Controller"))
                        {
                            metodoChamador = stackTrace.GetFrame(i).GetMethod().ReflectedType.Name +
                            "." + stackTrace.GetFrame(i).GetMethod().Name ;
                            break;
                        }
                    }
                }

                var eventos = new Eventos();
                eventos.paramBase = paramBase;

                eventos.Change(mensagem, registroNovo, metodoChamador, cadeiaMetodos);

                return true;

            }
            catch (Exception ex)
            {
                Eventos _eventos = new Eventos();
                _eventos.paramBase = paramBase;
                _eventos.Error(ex);
                return true;
            }
        }




        public List<LogMudanca> ObterTodos(ParamBase paramBase)
        {
            int estab = paramBase.estab_id ;
            DbControle banco = new DbControle();
            var logs = banco.LogMudanca.Where(x => x.estabelecimento_id == estab);
            return logs.ToList();
        }
    }


}
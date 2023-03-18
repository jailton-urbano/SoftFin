using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Core;
using Newtonsoft.Json;
using SoftFin.Web.Negocios;
using System.Diagnostics;
using SoftFin.Web.Models;
using SoftFin.Web.CallApi.CallApi;

namespace SoftFin.Web.Regras
{
    public class Eventos 
    {
        public ParamBase paramBase { get; set; }
        public string Metodo { get; set; }
        public Guid Agrupador { get; set; }

        private Log _logger;


        public Eventos()
        {
            Agrupador = Guid.NewGuid();
        }

        private void setMetodo()
        {
            StackTrace stackTrace = new StackTrace();
            Metodo = stackTrace.GetFrame(2).GetMethod().ReflectedType.UnderlyingSystemType.FullName +
                    "." + stackTrace.GetFrame(2).GetMethod().Name;
        }

        public void Info(string message)
        {
            setMetodo();
            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "INFO",
                Descricao = message,
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
            });

        }

        public void Info(string message, IList lista)
        {

            setMetodo();
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var json = JsonConvert.SerializeObject(lista, settings);

            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "INFO",
                Descricao = message,
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id,
                Json = json

            });

        }

        public void Info(string message, object item)
        {

            setMetodo();
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            settings.CheckAdditionalContent = false;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var json = JsonConvert.SerializeObject(item, settings);
            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "INFO",
                Descricao = message,
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id,
                Json = json

            });
        }

        

        public void Change(string mensagem, string registroNovo, string metodoChamador, string cadeiaMetodos)
        { 
                new Log().PostEvento(new Infrastructure.DTO.DTOLogEvento
                {
                    Agrupador = Agrupador,
                    Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                    Tipo = "CHANGE",
                    Descricao = mensagem,
                    Ip = paramBase.usuario_ip,
                    Json = registroNovo,
                    Usuario = paramBase.usuario_name,
                    Rotina = metodoChamador,
                    CadeiaMetodo = cadeiaMetodos,
                    Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
                });

        }

        public void Warn(string message)
        {
            setMetodo();
            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "WARN",
                Descricao = message,
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
            });
        }

        public void Debug(string message)
        {
            setMetodo();
            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "DEBUG",
                Descricao = message,
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
            });
        }

        public void Error(string message)
        {
            setMetodo();
            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "ERROR",
                Descricao = message,
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
            });
        }

        public void Error(Exception x)
        {
            setMetodo();
            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "ERROR",
                Exception = x.ToString(),
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
            });
        }

        public void Error(string message, Exception x)
        {
            setMetodo();
            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "ERROR",
                Descricao = message,
                Exception = x.ToString(),
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
            });
        }

        public void Fatal(string message)
        {
            setMetodo();
            new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
            {
                Agrupador = Agrupador,
                Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                Tipo = "FATAL",
                Descricao = message,
                Ip = paramBase.usuario_ip,
                Usuario = paramBase.usuario_name,
                Rotina = Metodo,
                Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
            });
        }

        public void Fatal(Exception x)
        {

                setMetodo();
                new Log().PostEventoAsync(new Infrastructure.DTO.DTOLogEvento
                {
                    Agrupador = Agrupador,
                    Data = SoftFin.Utils.UtilSoftFin.DateTimeBrasilia(),
                    Tipo = "FATAL",
                    Exception = x.ToString(),
                    Ip = paramBase.usuario_ip,
                    Usuario = paramBase.usuario_name,
                    Rotina = Metodo,
                    Estabelecimento = paramBase.empresa_id + ":" + paramBase.estab_id
                });

        }



    }
}

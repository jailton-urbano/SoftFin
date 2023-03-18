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

namespace SoftFin.Web.Regras
{
    public class EventosAntigo : ILogger
    {
        private ILog _logger;

        public ParamBase paramBase { get; set; }

        private void IniciaLoger()
        {

            log4net.Config.XmlConfigurator.Configure();

            log4net.ThreadContext.Properties["usuario"] = paramBase.usuario_name;
            log4net.ThreadContext.Properties["ip"] = paramBase.usuario_ip;
            log4net.ThreadContext.Properties["json"] = string.Empty;

            StackTrace stackTrace = new StackTrace();
            var metodo = stackTrace.GetFrame(1).GetMethod().ReflectedType.UnderlyingSystemType.FullName +
                    "." + stackTrace.GetFrame(1).GetMethod().Name;
            _logger = LogManager.GetLogger(metodo);
        }




        public void Info(string message)
        {
            IniciaLoger();

            _logger.Info(message);
        }

        public void Info(string message, IList lista)
        {
            IniciaLoger();

            var json = JsonConvert.SerializeObject(lista);
            log4net.ThreadContext.Properties["json"] = json;
            _logger.Info(message);

            log4net.ThreadContext.Properties["json"] = string.Empty;
        }

        public void Info(string message, object item)
        {
            IniciaLoger();

            var json = JsonConvert.SerializeObject(item);
            log4net.ThreadContext.Properties["json"] = json;
            _logger.Info(message);
            log4net.ThreadContext.Properties["json"] = string.Empty;
        }

        public void Warn(string message)
        {
            IniciaLoger();
            _logger.Warn(message);
        }

        public void Debug(string message)
        {
            IniciaLoger();
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            IniciaLoger();
            _logger.Error(message);
        }

        public void Error(Exception x)
        {
            IniciaLoger();
            _logger.Error(x.ToString());
        }

        public void Error(string message, Exception x)
        {
            IniciaLoger();
            _logger.Error(message, x);
        }

        public void Fatal(string message)
        {
            IniciaLoger();
            _logger.Fatal(message);
        }

        public void Fatal(Exception x)
        {
            IniciaLoger();
            Fatal(x.Message);
        }

        public bool IsEnabledFor(Level level)
        {
            return IsEnabledFor(level);
        }

        public void Log(LoggingEvent logEvent)
        {
            IniciaLoger();
            Log(logEvent);
        }

        public void Log(Type callerStackBoundaryDeclaringType, Level level, object message, Exception exception)
        {
            IniciaLoger();
            Log(callerStackBoundaryDeclaringType, level, message, exception);
        }


        public string Name
        {
            get
            {
                return Name;
            }
        }

        public log4net.Repository.ILoggerRepository Repository
        {
            get
            {
                return Repository;
            }
        }
    }
}

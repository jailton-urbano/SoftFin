using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Utils
{
    public class Email
    {
        MailMessage _mail = new MailMessage();
        SmtpClient _client = new SmtpClient();

        public string EnviarMensagem(string para, string titulo, string mensagem, bool corpoHtml = false)
        {
            _mail.Subject = titulo;
            _mail.Body = mensagem;
            if (para.IndexOf(';') == -1)
                _mail.To.Add(new MailAddress(para));
            else
            {
                var paraIndex = para.Split(';');
                foreach (var item in paraIndex)
                {
                    _mail.To.Add(new MailAddress(item));
                }
            }
            
            _mail.IsBodyHtml = corpoHtml;
            return Enviar();
        }




        private string Enviar()
        {
            try
            {
                
                _mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"].ToString());
                
                _client.UseDefaultCredentials = false;
                _client.Credentials =
                    new System.Net.NetworkCredential(
                        ConfigurationManager.AppSettings["EmailNetworkUser"].ToString(),
                        ConfigurationManager.AppSettings["EmailNetworkPass"].ToString());
                _client.EnableSsl = true;
                if (ConfigurationManager.AppSettings["EmailPort"].ToString() != "")
                    _client.Port = int.Parse(ConfigurationManager.AppSettings["EmailPort"].ToString());

                _client.Host = ConfigurationManager.AppSettings["EmailHost"].ToString();
//#if !DEBUG
                _client.Send(_mail);
//#endif
                    return "ok";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}

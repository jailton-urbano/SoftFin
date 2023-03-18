using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SoftFin.MyLIMS.Integrar
{
    class RestConection
    {

        public HttpClient Conecta(bool Origem)
        {
            var cta = new HttpClient();

            TimeSpan span = new TimeSpan(0, 0, 1, 30, 0);
            cta.Timeout = span;

            if (Origem)
            {
                //var apiUrlServer = ConfigurationManager.AppSettings["apiUrlServer"];
                var key = ConfigurationManager.AppSettings["tokenApiMyLims"].ToString();

                //cta.BaseAddress = new Uri(apiUrlServer);
                cta.DefaultRequestHeaders.Accept.Clear();
                cta.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                cta.DefaultRequestHeaders.Clear();
                cta.DefaultRequestHeaders.Add("x-access-key", key);
                //cta.DefaultRequestHeaders.Add("channelToken", keyAdianat);

                //if (clientToken != null)
                //cta.DefaultRequestHeaders.Add("x-access-key", key);


                //var byteArray = Encoding.ASCII.GetBytes(keyVindi + ":");
                //var header = new AuthenticationHeaderValue(
                //           "Basic", Convert.ToBase64String(byteArray));
                //cta.DefaultRequestHeaders.Authorization = header;
            }
            else
            {
                var key = ConfigurationManager.AppSettings["tokenApiSoftFin"].ToString();
                cta.DefaultRequestHeaders.Accept.Clear();
                cta.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                cta.DefaultRequestHeaders.Clear();
                cta.DefaultRequestHeaders.Add("x-access-key", key);
            }

            return cta;
        }

    }
}

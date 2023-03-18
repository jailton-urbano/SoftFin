using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SoftFin.MyLIMS.Integracao
{
    class RestConectionLog
    {

        public HttpClient Conecta()
        {
            var cta = new HttpClient();

            TimeSpan span = new TimeSpan(0, 0, 5, 0, 0);
            cta.Timeout = span;


            var key = ConfigurationManager.AppSettings["tokenApiSoftFinLog"].ToString();
            cta.DefaultRequestHeaders.Accept.Clear();
            cta.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cta.DefaultRequestHeaders.Clear();
            cta.DefaultRequestHeaders.Add("x-access-key", key);

            var byteArray = Encoding.ASCII.GetBytes(key);
            var header = new AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(byteArray));
            cta.DefaultRequestHeaders.Authorization = header;
            

            return cta;
        }

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Vindi
{
    public class BaseVindi
    {
        public async Task<U> Get<U>(string url)
        {
            try
            {
                using (var client = new RestConection().Conecta())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }
                    var model = await response.Content.ReadAsAsync<U>();

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public U GetSync<U>(string url)
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                Task<string> jsonString = null;

                using (var client = new RestConection().Conecta())
                {
                    var task = client.GetAsync(url)
                    .ContinueWith(
                      (taskwithresponse) =>
                      {
                          response = taskwithresponse.Result;
                          jsonString = response.Content.ReadAsStringAsync();
                          jsonString.Wait();

                      }
                      );

                    task.Wait();

                    var model = JsonConvert.DeserializeObject<U>(jsonString.Result); ;

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<U> Post<U>(string url, string objSerializado)
        {
            using (var client = new RestConection().Conecta())
            {
                HttpContent content = new StringContent(objSerializado);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }
                var model = await response.Content.ReadAsAsync<U>();

                return model;
            }
        }

        public U PostSync<U>(string url, string objSerializado)
        {
            try
            {
                var response = new HttpResponseMessage();
                Task<string> jsonString = null;
                HttpContent content = new StringContent(objSerializado);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (var client = new RestConection().Conecta())
                {
                    var task = client.PostAsync(url, content)
                    .ContinueWith(
                      (taskwithresponse) =>
                      {
                          response = taskwithresponse.Result;
                          jsonString = response.Content.ReadAsStringAsync();
                          jsonString.Wait();

                      }
                      );

                    task.Wait();

                    var model = JsonConvert.DeserializeObject<U>(jsonString.Result); ;

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<U> Delete<U>(string url)
        {
            using (var client = new RestConection().Conecta())
            {
                HttpResponseMessage response = await client.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }
                var model = await response.Content.ReadAsAsync<U>();

                return model;
            }
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SoftFin.MyLIMS.Integrar
{
    public class BaseMyLIMS
    {
        public async Task<U> Get<U>(string url, bool origem)
        {
            try
            {
                using (var client = new RestConection().Conecta(origem))
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

        public async Task<Stream> GetFile(string url, bool origem)
        {
            try
            {
                using (var client = new RestConection().Conecta(origem))
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception();
                    }
                    var model = await response.Content.ReadAsStreamAsync();

                    return model;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public U GetSync<U>(string url, bool origem)
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                Task<string> jsonString = null;

                using (var client = new RestConection().Conecta(origem))
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
        public async Task<U> Post<U>(string url, string objSerializado, bool origem)
        {
            using (var client = new RestConection().Conecta(origem))
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
        public async Task<Boolean> PostFile(string url, FileInfo fileInfo, bool origem)
        {
            using (var client = new RestConection().Conecta(origem))
            {
                var requestContent = new MultipartFormDataContent();
                var fileContent = new StreamContent(fileInfo.OpenRead());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    Name = "\"file\"",
                    FileName = "\"" + fileInfo.Name + "\""
                };
                fileContent.Headers.ContentType =
                    MediaTypeHeaderValue.Parse(MimeMapping.GetMimeMapping(fileInfo.Name));

                requestContent.Add(fileContent);

                //var folderContent = new StringContent(token);
                //requestContent.Add(folderContent, "\"clientToken\"");

                var result = await client.PostAsync(url, requestContent);
                return result.IsSuccessStatusCode;
            }
        }
        public U PostSync<U>(string url, string objSerializado, bool origem)
        {
            try
            {
                var response = new HttpResponseMessage();
                Task<string> jsonString = null;
                HttpContent content = new StringContent(objSerializado);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (var client = new RestConection().Conecta(origem))
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
        public async Task<U> Delete<U>(string url, bool origem)
        {
            using (var client = new RestConection().Conecta(origem))
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

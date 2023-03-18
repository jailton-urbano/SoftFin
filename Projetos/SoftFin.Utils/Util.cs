using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.Utils
{
    public static class UtilSoftFin
    {

        public static IEnumerable<Exception> FlattenHierarchy(this Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var innerException = ex;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static X509Certificate2 BuscaCert(int estab, string senha, string localCertificadoTMP, string cnpj)
        {
            try
            {
                AzureStorage.DownloadFile(localCertificadoTMP, "Certificados/" + estab + "/cert.pfx",
                        ConfigurationManager.AppSettings["StorageAtendimento"].ToString());
                X509Certificate2Collection collection = new X509Certificate2Collection();
               
                collection.Import(localCertificadoTMP, senha, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                cnpj = cnpj.Replace("-", "").Replace("/", "").Replace(".", "");
                foreach (var item in collection)
                {
                    if (item.Subject.Contains(cnpj))
                        return item;
                }


                return collection[collection.Count - 1];



            }
            catch (Exception ex)
            {
                throw new Exception("Certificado não configurado corretamente: " + ex.Message + " [" + localCertificadoTMP + "-" + senha + "]");
            }
        }

        public static String Limpastrings(String valor)
        {
            if (valor == null)
                return "";
            return valor.Replace("-", "").Replace("/", "").Replace(".", "").Replace("\\", "").Replace(" ", "").Replace("(", "").Replace(")", "");
        }

        public static class Seguranca
        {
            public static void validaNulo(object obj)
            {
                if (obj == null)
                    throw new Exception("Acesso Negado.");

            }


        }

        public static T Cast<T>(Object myobj)
        {
            Type objectType = myobj.GetType();
            Type target = typeof(T);
            var x = Activator.CreateInstance(target, false);
            var z = from source in objectType.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;
            var d = from source in target.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;
            List<MemberInfo> members = d.Where(memberInfo => d.Select(c => c.Name)
               .ToList().Contains(memberInfo.Name)).ToList();
            PropertyInfo propertyInfo;
            object value;
            foreach (var memberInfo in members)
            {
                propertyInfo = typeof(T).GetProperty(memberInfo.Name);
                try
                {
                    value = myobj.GetType().GetProperty(memberInfo.Name).GetValue(myobj, null);
                    propertyInfo.SetValue(x, value, null);

                }
                catch { }

            }
            return (T)x;
        }

        public static string ExtraiString(Dictionary<string, string> parameters, string key)
        {
            try
            {
                if (parameters.ContainsKey(key))
                    return parameters[key];
                else
                    return null;
            }
            catch
            {
                return null;
            }

        }


        public static List<T> Organiza<T>(List<T> objs, JqGridRequest request)
        {
            var quebra = request.SortingName.Split('$');


            switch (quebra.Length)
            {
                case 3:
                    //TODO
                    return objs;
                case 2:
                    return new GenericSorter<T>().Sort(objs.AsQueryable(), quebra[1], request.SortingOrder).ToList();
                default:
                    throw new Exception("Ordenação não implementada");

            }
        }

        public static DateTime DateTimeBrasilia()
        {
            // Listando os fusos horários existentes (apenas para observar os valores na collection)
            ReadOnlyCollection<TimeZoneInfo> collection = TimeZoneInfo.GetSystemTimeZones();

            // Mesmo estando o servidor configurado para qualquer fuso horário, o código abaixo obtém o horário de Brasília
            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo kstZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); // Brasilia/BRA
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, kstZone);
        }


        public static DateTime DateBrasilia()
        {
            var data = DateTimeBrasilia();


            return new DateTime(data.Year, data.Month, data.Day, 0, 0, 0);
        }
        public static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

        public static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }


        public class GenericSorter<T>
        {
            public IEnumerable<T> Sort(IEnumerable<T> source, string sortBy, JqGridSortingOrders sortDirection)
            {
                var param = Expression.Parameter(typeof(T), "item");

                var sortExpression = Expression.Lambda<Func<T, object>>
                    (Expression.Convert(Expression.Property(param, sortBy), typeof(object)), param);

                switch (sortDirection)
                {
                    case JqGridSortingOrders.Asc:
                        return source.AsQueryable<T>().OrderBy<T, object>(sortExpression);
                    default:
                        return source.AsQueryable<T>().OrderByDescending<T, object>(sortExpression);

                }
            }




        }

        public static DateTime TiraHora(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        }


        public static T GetObjectFromCacheFunction<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = (T)cache[cacheItemName];
            if (cachedObject == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes);
                cachedObject = objectSettingFunction();
                cache.Set(cacheItemName, cachedObject, policy);
            }
            return cachedObject;
        }

        public static class CacheSF
        {
            public static void Reset()
            {
                List<string> keys = new List<string>();
                // retrieve application Cache enumerator
                var cache = MemoryCache.Default;

                foreach (var item in cache)
                {
                    keys.Add(item.Key);
                }
                foreach (var item in keys)
                {
                    cache.Remove(item);
                }
            }

            public static void AddItem(string key, object value)
            {
                var cache = MemoryCache.Default;
                var padlock = new object();
                lock (padlock)
                {
                    var res = cache[key];

                    if (res != null)
                    {
                        cache.Remove(key);
                    }
                    cache.Add(key, value, DateTimeOffset.MaxValue);
                }
            }

            public static void RemoveItem(string key)
            {
                var cache = MemoryCache.Default;
                var padlock = new object();
                lock (padlock)
                {
                    cache.Remove(key);
                }
            }

            public static T GetItem<T>(string key, bool remove = false)
            {
                var cache = MemoryCache.Default;
                var padlock = new object();
                lock (padlock)
                {
                    var res = cache[key];

                    if (res != null)
                    {
                        if (remove == true)
                            cache.Remove(key);
                    }


                    return (T)res;
                }
            }


            

        }

        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }


    }

}

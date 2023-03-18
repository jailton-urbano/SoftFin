using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace SoftFin.Utils
{
    public static class Crypto
    {
        private static TripleDES CreateDES(string key)
        {
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(key));
            des.IV = new byte[des.BlockSize / 8];
            return des;
        }

        private static byte[] In(string plainText, string key)
        {
            ICryptoTransform ct = CreateDES(key).CreateEncryptor();
            byte[] input = Encoding.UTF8.GetBytes(plainText);
            return ct.TransformFinalBlock(input, 0, input.Length);
        }

        private static string Out(string cypherText, string key)
        {
            if (string.IsNullOrEmpty(cypherText)) return cypherText;
            byte[] b = Convert.FromBase64String(cypherText);
            ICryptoTransform ct = CreateDES(key).CreateDecryptor();
            byte[] output = ct.TransformFinalBlock(b, 0, b.Length);
            return Encoding.UTF8.GetString(output);
        }
                
        public static string Encryption(this String plainText, string key)
        {
            return Convert.ToBase64String(In(plainText, key));
        }

        public static string Decryption(this String cypherText, string key)
        {
            return Out(cypherText, key);
        }

        public static string Encryption(this String plainText)
        {

            String tripleKey = ConfigurationManager.AppSettings["CryptoKey"].ToString();
            if (!String.IsNullOrWhiteSpace(tripleKey))
                return Encryption(plainText, tripleKey);
            else
                throw new System.Exception("AppSettings não encontrado \"TripleKey\".");
        }

        public static string Decryption(this String cypherText)
        {

            String tripleKey = ConfigurationManager.AppSettings["CryptoKey"].ToString();
            if (!String.IsNullOrWhiteSpace(tripleKey))
                return Decryption(cypherText, tripleKey);
            else
                throw new System.Exception("AppSettings não encontrado \"TripleKey\".");
        }
    }
}

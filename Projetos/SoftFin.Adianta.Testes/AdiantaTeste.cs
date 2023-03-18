using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftFin.Adianta.Testes
{
    [TestClass]
    public class AdiantaTeste
    {
        [TestMethod]
        public void TestMethod1()
        {
            var cnpj = "23857646000195";
            var adianta = new SoftFin.Adianta.CallApi.Negocio();
            adianta.token(cnpj);
        }
    }
}

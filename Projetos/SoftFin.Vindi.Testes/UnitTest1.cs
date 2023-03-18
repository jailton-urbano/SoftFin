using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftFin.Vindi.callApi;

namespace SoftFin.Vindi.Testes
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var obj = new Fatura();
            var resultado = obj.getList("889740");

            Assert.IsTrue(resultado != null);
        }
    }
}

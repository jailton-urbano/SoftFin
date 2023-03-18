using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftFin.MyLIMS.CallApi;
namespace SoftFin.Vindi.Testes
{
    [TestClass]
    public class TesteIntegracao
    {
        [TestMethod]
        public void TestMethod1()
        {
            var obj = new IntegracaoController();
            var resultado = obj.getList("889740");

            Assert.IsTrue(resultado != null);
        }
    }
}

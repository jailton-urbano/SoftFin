using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftFin.MyLIMS.Integracao.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGetWorks()
        {
            var wks = new CallApi.Origem().GetWorks();
        }
        

        [TestMethod]
        public void TestMakeToken()
        {
            var wks = new CallApi.Destino().GetContratoPorCodigo("01");
        }


        [TestMethod]
        public void TestGetContrato()
        {
            var wks = new CallApi.Destino().GetContratoPorCodigo("01");
        }
        

    }
}

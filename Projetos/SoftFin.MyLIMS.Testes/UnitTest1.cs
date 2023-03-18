using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftFin.MyLIMS.Testes
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCheckConnection()
        {
            try
            {
                var retorno = new SoftFin.MyLIMS.CallApi.Origem().CheckConnection();

                Assert.IsTrue(true, retorno);
            }
            catch(Exception ex)
            {
                Assert.IsFalse(true, ex.Message);
            }

        }

        //[TestMethod]
        //public void TestServiceArea()
        //{
        //    try
        //    {
        //        var retorno = new SoftFin.MyLIMS.CallApi.Negocio().ServiceArea();

        //        Assert.IsTrue(true, retorno.Count.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsFalse(true, ex.Message);
        //    }

        //}
        [TestMethod]
        public void TestServiceWork()
        {
            try
            {
                var retorno = new SoftFin.MyLIMS.CallApi.Origem().Works();

                Assert.IsTrue(true, retorno.Count.ToString());
            }
            catch (Exception ex)
            {
                Assert.IsFalse(true, ex.Message);
            }

        }


        [TestMethod]
        public void TestServiceWorkClasses()
        {
            try
            {
                var retorno = new SoftFin.MyLIMS.CallApi.Origem().WorkClasses();

                Assert.IsTrue(true, retorno.Count.ToString());
            }
            catch (Exception ex)
            {
                Assert.IsFalse(true, ex.Message);
            }

        }
    }
}

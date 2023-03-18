using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftFin.GestorProcessos.API.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftFin.GestorProcessos.Tests.Controllers
{
    [TestClass]
    public class ProcessoExecucaoControllerText
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            ProcessoExecucaoController controller = new ProcessoExecucaoController();

            // Act
            //var result = controller.CriarNovaExecucaoProcesso(
            //    new Comum.Param.ParamProcesso { CodigoEmpresa = "SoftFin",
            //        CodigoUsuario = "gp@softfin.com.br",
            //        CodigoProcesso = "BDF4535A-3FF1-435D-84D6-E8B9BCD2BE72"
            //    });

            // Assert
            
            //Assert.IsTrue(result.status == "OK");

        }
    }
}

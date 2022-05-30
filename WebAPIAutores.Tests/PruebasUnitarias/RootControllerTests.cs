using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebAPIAutores.Tests.Mocks;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_Obtenemos4Links()
        {
            // Preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Ejecucion
            var resultado = await rootController.Get();

            //Verificacion
            Assert.AreEqual(4, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2Links()
        {
            // Preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Ejecucion
            var resultado = await rootController.Get();

            //Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
        }
    }
}

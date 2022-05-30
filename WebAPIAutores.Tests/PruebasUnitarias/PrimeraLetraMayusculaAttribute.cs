using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            // Preparacion
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "angelo";
            var valContext = new ValidationContext(new { Nombre = valor });

            // Ejecucion
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            // Verificacion
            Assert.AreEqual("La primera letra debe ser mayúscula", resultado.ErrorMessage);
        }
    }
}
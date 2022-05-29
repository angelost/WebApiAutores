namespace WebApiAutores.DTOs
{
    public class CollecionDeRecursos<T>: Recurso where T: Recurso
    {
        public List<T> Valores { get; set; }
    }
}

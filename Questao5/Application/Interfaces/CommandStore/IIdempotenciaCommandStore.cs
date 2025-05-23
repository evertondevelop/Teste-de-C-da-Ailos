namespace Questao5.Application.Interfaces.CommandStore
{
    public interface IIdempotenciaCommandStore
    {
        Task<bool> SalvaIdempotencia(string idIdempotencia, string requestSerialized, string resultadoSerialized);
    }
}

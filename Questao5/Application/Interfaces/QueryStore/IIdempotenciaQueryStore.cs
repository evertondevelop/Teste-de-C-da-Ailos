using Questao5.Domain.Entities;

namespace Questao5.Application.Interfaces.QueryStore
{
    public interface IIdempotenciaQueryStore
    {
        Task<Idempotencia> BuscarIdempotenciaPorID(string idempotencia);
    }
}

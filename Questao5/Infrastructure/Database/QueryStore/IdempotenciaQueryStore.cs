using Dapper;
using Questao5.Application.Interfaces.QueryStore;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public class IdempotenciaQueryStore : IIdempotenciaQueryStore
    {
        private readonly IDbConnection _conn;

        public IdempotenciaQueryStore(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<Idempotencia> BuscarIdempotenciaPorID(string idempotencia)
        {
            return await _conn.QueryFirstOrDefaultAsync<Idempotencia>(
                @"SELECT chave_idempotencia as Id, requisicao, resultado FROM Idempotencia WHERE chave_idempotencia = @Idempotencia",
                new { Idempotencia = idempotencia }
            );
        }
    }
}

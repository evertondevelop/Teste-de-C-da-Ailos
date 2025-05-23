using Dapper;
using Questao5.Application.Interfaces.CommandStore;
using System.Data;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class IdempotenciaCommandStore : IIdempotenciaCommandStore
    {
        private readonly IDbConnection _conn;

        public IdempotenciaCommandStore(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<bool> SalvaIdempotencia(string idIdempotencia, string requestSerialized, string resultadoSerialized)
        {
            var affectedRows = await _conn.ExecuteAsync(
                @"INSERT INTO Idempotencia (chave_idempotencia, requisicao, resultado) 
                  VALUES (@Idempotencia, @Requisicao, @Resultado);",
                new
                {
                    Idempotencia = idIdempotencia,
                    Requisicao = requestSerialized,
                    Resultado = resultadoSerialized
                }
            );

            return affectedRows > 0;
        }
    }
}

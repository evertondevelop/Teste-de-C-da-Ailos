using Dapper;
using Questao5.Application.Interfaces.CommandStore;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Infrastructure.Database.CommandStore
{
    public class MovimentoCommandStore : IMovimentoCommandStore
    {
        private readonly IDbConnection _conn;

        public MovimentoCommandStore(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<bool> SalvaMovimento(Movimento movimento)
        {
            var affectedRows = await _conn.ExecuteAsync(
                @"INSERT INTO Movimento (IdMovimento, IdContaCorrente, TipoMovimento, Valor, DataMovimento) 
                  VALUES (@IdMovimento, @IdContaCorrente, @TipoMovimento, @Valor, @DataMovimento);",
                movimento
            );

            return affectedRows > 0;
        }
    }
}

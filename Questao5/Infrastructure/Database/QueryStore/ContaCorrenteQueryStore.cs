using Dapper;
using Questao5.Application.Interfaces.QueryStore;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Infrastructure.Database.QueryStore
{
    public class ContaCorrenteQueryStore : IContaCorrenteQueryStore
    {
        private readonly IDbConnection _conn;

        public ContaCorrenteQueryStore(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<ContaCorrente> BuscarContaCorrentePorID(string idContaCorrente)
        {
            return await _conn.QuerySingleOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM ContaCorrente WHERE idContaCorrente = @Id",
                new { Id = idContaCorrente }
            );
        }

        public async Task<decimal> BuscarSaldoContaCorrente(string idContaCorrente)
        {
            return await _conn.ExecuteScalarAsync<decimal>(
                @"SELECT 
                        SUM(CASE WHEN TipoMovimento = 'C' THEN Valor ELSE 0 END) -
                        SUM(CASE WHEN TipoMovimento = 'D' THEN Valor ELSE 0 END) AS Saldo
                     FROM Movimento
                     WHERE idContaCorrente = @Id;
                     ",
                new { Id = idContaCorrente });
        }
    }
}

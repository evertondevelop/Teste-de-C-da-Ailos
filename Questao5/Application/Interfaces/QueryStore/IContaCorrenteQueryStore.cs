using Questao5.Domain.Entities;

namespace Questao5.Application.Interfaces.QueryStore
{
    public interface IContaCorrenteQueryStore
    {
        Task<ContaCorrente> BuscarContaCorrentePorID(string idContaCorrente);
        Task<decimal> BuscarSaldoContaCorrente(string idContaCorrente);
    }
}

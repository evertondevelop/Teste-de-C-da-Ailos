using Questao5.Domain.Entities;

namespace Questao5.Application.Interfaces.CommandStore
{
    public interface IMovimentoCommandStore
    {
        Task<bool> SalvaMovimento(Movimento movimento);
    }
}

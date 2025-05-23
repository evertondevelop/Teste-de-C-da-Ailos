using Questao5.Domain.Entities;

namespace Questao5.Application.Interfaces
{
    public interface IInserirMovimentoService
    {
        Task<bool> Execute(Movimento movimento, string identificadorRequisicao, string requestSerialized, string responseSerialized);
    }
}

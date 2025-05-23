using Questao5.Application.Interfaces;
using Questao5.Application.Interfaces.CommandStore;
using Questao5.Domain.Entities;
using System.Data;

namespace Questao5.Application.Services
{
    public class InserirMovimentoService : IInserirMovimentoService
    {
        private readonly IDbConnection _conn;
        private readonly IMovimentoCommandStore _movimentoCommandStore;
        private readonly IIdempotenciaCommandStore _idempotenciaCommandStore;

        public InserirMovimentoService(IDbConnection conn, IMovimentoCommandStore movimentoCommandStore, IIdempotenciaCommandStore idempotenciaCommandStore)
        {
            _conn = conn;
            _movimentoCommandStore = movimentoCommandStore;
            _idempotenciaCommandStore = idempotenciaCommandStore;
        }

        public async Task<bool> Execute(Movimento movimento, string identificadorRequisicao, string requestSerialized, string responseSerialized)
        {
            if (_conn.State != ConnectionState.Open)
            {
                _conn.Open();
            }

            using var transaction = _conn.BeginTransaction();

            try
            {
                await Task.WhenAll(
                    _movimentoCommandStore.SalvaMovimento(movimento),
                    _idempotenciaCommandStore.SalvaIdempotencia(identificadorRequisicao, requestSerialized, responseSerialized)
                );

                transaction.Commit();

                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
            }

            return false;
        }
    }
}

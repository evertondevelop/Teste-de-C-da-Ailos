using MediatR;
using Questao5.Application.Common;
using Questao5.Application.Commons.Enumerators;
using Questao5.Application.Extensions;
using Questao5.Application.Interfaces.QueryStore;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Handlers
{
    public class SaldoContaCorrenteQueryHandler : IRequestHandler<SaldoContaCorrenteQueryRequest, Resultado<SaldoContaCorrenteQueryResponse>>
    {
        private readonly IContaCorrenteQueryStore _contaCorrenteQueryStore;

        public SaldoContaCorrenteQueryHandler(IContaCorrenteQueryStore contaCorrenteQueryStore)
        {
            _contaCorrenteQueryStore = contaCorrenteQueryStore;
        }

        public async Task<Resultado<SaldoContaCorrenteQueryResponse>> Handle(SaldoContaCorrenteQueryRequest request, CancellationToken cancellationToken)
        {
            var validator = (new SaldoContaCorrenteQueryRequestValidator()).Validate(request);

            if (!validator.IsValid)
            {
                return Resultado<SaldoContaCorrenteQueryResponse>.Erro(validator.Errors.ToResultado());
            }

            var conta = await _contaCorrenteQueryStore.BuscarContaCorrentePorID(request.ContaCorrenteId);

            if (conta == null)
            {
                return Resultado<SaldoContaCorrenteQueryResponse>.Erro(ETipoErro.INVALID_ACCOUNT, "A conta não foi encontrada");
            }

            if (!conta.Ativo)
            {
                return Resultado<SaldoContaCorrenteQueryResponse>.Erro(ETipoErro.INACTIVE_ACCOUNT, "Conta inativa");
            }

            var saldo = await _contaCorrenteQueryStore.BuscarSaldoContaCorrente(request.ContaCorrenteId);

            var response = new SaldoContaCorrenteQueryResponse
            {
                NumeroConta = conta.Numero,
                Nome = conta.Nome,
                DataConsulta = DateTime.Now,
                Saldo = saldo
            };

            return Resultado<SaldoContaCorrenteQueryResponse>.Ok(response);
        }
    }
}

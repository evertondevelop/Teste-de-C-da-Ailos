using MediatR;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Common;
using Questao5.Application.Commons.Enumerators;
using Questao5.Application.Extensions;
using Questao5.Application.Interfaces;
using Questao5.Application.Interfaces.QueryStore;
using Questao5.Domain.Entities;
using System.Text.Json;

namespace Questao5.Application.Handlers
{
    public class MovimentarContaCorrenteHandler : IRequestHandler<MovimentarContaCorrenteRequest, Resultado<MovimentarContaCorrenteResponse>>
    {
        private readonly IIdempotenciaQueryStore _idempotenciaQueryStore;
        private readonly IContaCorrenteQueryStore _contaCorrenteStore;
        private readonly IInserirMovimentoService _inserirMovimentoService;

        public MovimentarContaCorrenteHandler(
            IIdempotenciaQueryStore idempotenciaQueryStore,
            IContaCorrenteQueryStore contaCorrenteStore,
            IInserirMovimentoService inserirMovimentoService
        )
        {
            _idempotenciaQueryStore = idempotenciaQueryStore;
            _contaCorrenteStore = contaCorrenteStore;
            _inserirMovimentoService = inserirMovimentoService;
        }

        public async Task<Resultado<MovimentarContaCorrenteResponse>> Handle(MovimentarContaCorrenteRequest request, CancellationToken cancellationToken)
        {
            var validator = (new MovimentarContaCorrenteRequestValidator()).Validate(request);

            if (!validator.IsValid)
            {
                return Resultado<MovimentarContaCorrenteResponse>.Erro(validator.Errors.ToResultado());
            }

            var idempotencia = await _idempotenciaQueryStore.BuscarIdempotenciaPorID(request.IdentificadorRequisicao);

            if (idempotencia != null)
            {
                var responseIdempotencia = JsonSerializer.Deserialize<string>(idempotencia.Resultado);
                return Resultado<MovimentarContaCorrenteResponse>.Ok(new MovimentarContaCorrenteResponse { MovimentoId = responseIdempotencia! });
            }

            var conta = await _contaCorrenteStore.BuscarContaCorrentePorID(request.ContaCorrenteId);

            if (conta == null)
            {
                return Resultado<MovimentarContaCorrenteResponse>.Erro(ETipoErro.INVALID_ACCOUNT, "A conta não foi encontrada");
            }

            if (!conta.Ativo)
            {
                return Resultado<MovimentarContaCorrenteResponse>.Erro(ETipoErro.INACTIVE_ACCOUNT, "Conta inativa");
            }

            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = conta.IdContaCorrente,
                TipoMovimento = request.TipoMovimento.ToString(),
                Valor = request.Valor,
                DataMovimento = DateTime.Now
            };

            var success = await _inserirMovimentoService.Execute(
                    movimento,
                    request.IdentificadorRequisicao,
                    JsonSerializer.Serialize(request),
                    JsonSerializer.Serialize(movimento.IdMovimento)
            );

            if (!success)
            {
                return Resultado<MovimentarContaCorrenteResponse>.Erro(ETipoErro.ERROR_INSERTING_MOVEMENT, "Ocorreu um erro ao movimentar a conta, contate o suporte.");
            }

            var response = new MovimentarContaCorrenteResponse
            {
                MovimentoId = movimento.IdMovimento
            };

            return Resultado<MovimentarContaCorrenteResponse>.Ok(response);
        }
    }
}

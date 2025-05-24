using FluentValidation;
using MediatR;
using Questao5.Application.Common;
using Questao5.Application.Commons.Enumerators;
using Questao5.Application.Queries.Responses;

namespace Questao5.Application.Queries.Requests
{
    public record SaldoContaCorrenteQueryRequest : IRequest<Resultado<SaldoContaCorrenteQueryResponse>>
    {
        /// <summary>
        /// Id da conta corrente
        /// </summary>
        /// <example>FA99D033-7067-ED11-96C6-7C5DFA4A16C9</example>
        public string ContaCorrenteId { get; set; } = string.Empty;
    }

    public class SaldoContaCorrenteQueryRequestValidator : AbstractValidator<SaldoContaCorrenteQueryRequest>
    {
        public SaldoContaCorrenteQueryRequestValidator()
        {
            RuleFor(x => x.ContaCorrenteId)
                .NotEmpty()
                .WithErrorCode(ETipoErro.VALIDATION_ERROR.ToString())
                .WithMessage("Conta Corrente Id é obrigatório.");
        }
    }
}

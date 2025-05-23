using FluentValidation;
using MediatR;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Common;
using Questao5.Application.Commons.Enumerators;
using System.ComponentModel;

namespace Questao5.Application.Commands.Requests
{
    public record MovimentarContaCorrenteRequest : IRequest<Resultado<MovimentarContaCorrenteResponse>>
    {
        [DefaultValue("111379f4-e79b-4139-af29-bdd9a425909e")]
        public string IdentificadorRequisicao { get; set; } = string.Empty;

        [DefaultValue("FA99D033-7067-ED11-96C6-7C5DFA4A16C9")]
        public string ContaCorrenteId { get; set; } = string.Empty;

        [DefaultValue(100)]
        public decimal Valor { get; set; }

        [DefaultValue("C")]
        public string TipoMovimento { get; set; } = string.Empty;
    }

    public class MovimentarContaCorrenteRequestValidator : AbstractValidator<MovimentarContaCorrenteRequest>
    {
        public MovimentarContaCorrenteRequestValidator()
        {
            RuleFor(x => x.IdentificadorRequisicao)
                .NotEmpty()
                .WithErrorCode(ETipoErro.VALIDATION_ERROR.ToString())
                .WithMessage("Identificador de requisição é obrigatório.");

            RuleFor(x => x.ContaCorrenteId)
                .NotEmpty()
                .WithErrorCode(ETipoErro.VALIDATION_ERROR.ToString())
                .WithMessage("Conta Corrente Id é obrigatório.");

            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithErrorCode(ETipoErro.INVALID_VALUE.ToString())
                .WithMessage("Valor deve ser maior que zero.");

            RuleFor(x => x.TipoMovimento)
                .NotEmpty()
                .WithErrorCode(ETipoErro.VALIDATION_ERROR.ToString())
                .WithMessage("Tipo de movimento é obrigatório.")
                .Must(tm => tm == "C" || tm == "D")
                .WithErrorCode(ETipoErro.INVALID_TYPE.ToString())
                .WithMessage("Tipo de movimento deve ser 'C' ou 'D'.");
        }
    }
}

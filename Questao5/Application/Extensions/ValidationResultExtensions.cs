using FluentValidation.Results;
using Questao5.Application.Common;

namespace Questao5.Application.Extensions
{
    public static class ValidationResultExtensions
    {
        public static IEnumerable<DetalheErro> ToResultado(this List<ValidationFailure> validationErrors)
        {
            return validationErrors
                .Select(e => new DetalheErro(e.ErrorMessage, e.ErrorCode))
                .ToList();
        }
    }
}

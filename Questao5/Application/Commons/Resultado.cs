using Questao5.Application.Commons.Enumerators;

namespace Questao5.Application.Common
{
    public class Resultado<T>
    {
        public bool Sucesso { get; init; }
        public T? Value { get; init; }
        public List<DetalheErro> Errors { get; init; } = new();

        private Resultado(bool sucesso, T? value, List<DetalheErro>? errors = null)
        {
            Sucesso = sucesso;
            Value = value;
            Errors = errors ?? new();
        }

        public static Resultado<T> Ok(T value) => new(true, value);

        public static Resultado<T> Erro(ETipoErro tipo, params string[] messages) =>
            new(false, default, messages.Select(msg => new DetalheErro(msg, tipo.ToString())).ToList());

        public static Resultado<T> Erro(IEnumerable<DetalheErro> errors) =>
            new(false, default, errors.ToList());
    }
}

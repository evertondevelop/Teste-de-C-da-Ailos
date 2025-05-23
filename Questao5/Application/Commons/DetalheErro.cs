using Questao5.Application.Commons.Enumerators;

namespace Questao5.Application.Common
{
    public class DetalheErro
    {
        public string Mensagem { get; set; } = string.Empty;
        public string Tipo { get; set; }

        public DetalheErro(string mensagem, string tipo)
        {
            Mensagem = mensagem;
            Tipo = tipo;
        }
    }
}

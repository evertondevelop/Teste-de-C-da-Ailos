namespace Questao5.Application.Queries.Responses
{
    public record SaldoContaCorrenteQueryResponse
    {
        public int NumeroConta { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataConsulta { get; set; }
        public decimal Saldo { get; set; }
    }
}

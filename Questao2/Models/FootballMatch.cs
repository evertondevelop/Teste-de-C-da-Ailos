namespace Questao2.Models
{
    public record FootballMatch
    {
        public int Page { get; set; }
        public int Per_page { get; set; }
        public int Total { get; set; }
        public int Total_pages { get; set; }
        public CompetetionModel[]? Data { get; set; }
    }
}

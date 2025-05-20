namespace Questao2.Models
{
    public record CompetetionModel
    {
        public string Competition { get; set; } = String.Empty;
        public int Year { get; set; }
        public string Round { get; set; } = String.Empty;
        public string Team1 { get; set; } = String.Empty;
        public string Team2 { get; set; } = String.Empty;
        public int Team1goals { get; set; }
        public int Team2goals { get; set; }
    }
}

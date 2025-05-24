using Newtonsoft.Json;
using Questao2.Models;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team "+ teamName +" scored "+ totalGoals.ToString() + " goals in "+ year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> getTotalScoredGoals(string team, int year)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://jsonmock.hackerrank.com/api/football_matches");

        int totalGoalsTeam1 = await fetchTotalScoredPerTeamType(client, team, "team1", year, f => f.Team1goals);
        int totalGoalsTeam2 = await fetchTotalScoredPerTeamType(client, team, "team2", year, f => f.Team2goals);

        int totalGoals = totalGoalsTeam1 + totalGoalsTeam2;
        return totalGoals;
    }

    private static async Task<int> fetchTotalScoredPerTeamType(HttpClient client, string team, string teamType, int year, Func<CompetetionModel, int> teamGoalsSelector)
    {
        List<CompetetionModel> allFootbalMatch = new();
        int actualPage = 1;

        while(true)
        {
            var response = await client.GetAsync($"?year={year}&{teamType}={Uri.EscapeDataString(team)}&page={actualPage}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<FootballMatch>(json);

            if (data.Data.Length == 0) break;

            allFootbalMatch.AddRange(data.Data);
            actualPage++;
        }

        return allFootbalMatch.Sum(teamGoalsSelector);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILeaderboardService
{
    Task RegisterNickname();
    Task SubmitScore(int score);
    Task<List<LeaderboardEntry>> GetTopScores(int count);
    Task<int> GetPlayerRank();
    Task<LeaderboardEntry> GetPlayerBestScore();
}

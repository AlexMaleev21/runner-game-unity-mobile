using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public interface ILeaderboardService
{
    Task SubmitScore(int score);
    Task<List<LeaderboardEntry>> GetTopScores(int count);
    Task<int> GetPlayerRank(string userId);
    Task<LeaderboardEntry> GetPlayerBestScore(string userId);
}

using Cysharp.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseLeaderboardService : ILeaderboardService
{
    public const string LeaderboardPath = "bestScores";
    private DatabaseReference _databaseRef;
    private readonly IAuthService _authService;
    private bool _isInitialized = false;
    private Task _initTask;

    public FirebaseLeaderboardService(IAuthService authService)
    {
        _authService = authService;
        //Task.Run(() => InitializeAsync());
    }

    private async Task InitializeAsync()
    {
        Debug.Log(_isInitialized);
        if (_isInitialized) return;
        if (_initTask != null) await _initTask;
        _initTask = InitializeInternalAsync();
        await _initTask;
    }

    private async Task InitializeInternalAsync()
    {
        var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == Firebase.DependencyStatus.Available)
        {
            _databaseRef = FirebaseDatabase.DefaultInstance.RootReference.Child(LeaderboardPath);
            _isInitialized = true;
        }
    }

    public async Task SubmitScore(int score)
    {
        await InitializeAsync();
        string userId = _authService.UserId;
        string userName = _authService.UserName;

        var bestSnapshot = await _databaseRef.Parent.Child(LeaderboardPath).Child(userId).GetValueAsync();
        var bestEntry = JsonUtility.FromJson<LeaderboardEntry>(bestSnapshot.GetRawJsonValue());
        
        if (bestEntry != null && bestEntry.score >= score ) return;

        var entry = new LeaderboardEntry(userId, userName, score);
        string json = JsonUtility.ToJson(entry);
        await _databaseRef.Parent.Child(LeaderboardPath).Child(userId).SetRawJsonValueAsync(json);
    }

    public async Task<List<LeaderboardEntry>> GetTopScores(int count)
    {
        await InitializeAsync();
        var result = new List<LeaderboardEntry>();
        var snapshot = await _databaseRef.Parent.Child(LeaderboardPath).OrderByChild("score").LimitToLast(count).GetValueAsync();
        foreach (var child in snapshot.Children)
        {
            var json = child.GetRawJsonValue();
            var entry = JsonUtility.FromJson<LeaderboardEntry>(json);
            if (entry != null)
                result.Insert(0, entry);
        }
        return result;
    }

    public async Task<int> GetPlayerRank(string userId)
    {
        await InitializeAsync();
        var snapshot = await _databaseRef.OrderByChild("score").GetValueAsync();

        int rank = 1;
        int? lastScore = null;
        foreach (var child in snapshot.Children)
        {
            var json = child.GetRawJsonValue();
            var entry = JsonUtility.FromJson<LeaderboardEntry>(json);
            if (entry.userId == userId)
            {
                return rank;
            }
            if (entry.score < lastScore)
            {
                rank++;
                lastScore = entry.score;
            }
        }
        return -1;
    }

    public async Task<LeaderboardEntry> GetPlayerBestScore(string userId)
    {
        await InitializeAsync();
        var snapshot = await _databaseRef.OrderByChild("userId").EqualTo(userId).GetValueAsync();

        int bestScore = 0;
        LeaderboardEntry bestEntry = null;
        foreach (var child in snapshot.Children)
        {
            var json = child.GetRawJsonValue();
            var entry = JsonUtility.FromJson<LeaderboardEntry>(json);
            if (entry.score > bestScore)
            {
                bestScore = entry.score;
                bestEntry = entry;
            }
        }
        return bestEntry;
    }
}

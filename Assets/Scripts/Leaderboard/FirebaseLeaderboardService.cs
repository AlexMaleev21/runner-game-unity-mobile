using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseLeaderboardService : ILeaderboardService
{
    public const string LeaderboardPath = "bestScores";
    public const string NicknamesPath = "nicknames";
    private DatabaseReference _databaseRef;
    private readonly PlayerProfileService _playerProfileService;
    private bool _isInitialized = false;
    private Task _initTask;

    public FirebaseLeaderboardService(PlayerProfileService playerProfileService)
    {
        _playerProfileService = playerProfileService;
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        if (_initTask != null)
        {
            await _initTask;
            return;
        }

        _initTask = InitializeInternalAsync();
        await _initTask;

        if (!_isInitialized)
            _initTask = null;
    }

    private async Task InitializeInternalAsync()
    {
        var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == Firebase.DependencyStatus.Available)
        {
            _databaseRef = FirebaseDatabase.DefaultInstance.RootReference.Child(LeaderboardPath);
            _isInitialized = true;
            return;
        }

        Debug.LogError($"Firebase dependencies are not available: {dependencyStatus}");
    }

    public async Task RegisterNickname()
    {
        if (!await TryInitializeAsync() || !_playerProfileService.HasNickname)
            return;

        try
        {
            await _databaseRef.Parent
                .Child(NicknamesPath)
                .Child(_playerProfileService.NicknameKey)
                .SetValueAsync(_playerProfileService.Nickname);
        }
        catch (Exception exception)
        {
            LogFirebaseError("register nickname", exception);
        }
    }

    public async Task SubmitScore(int score)
    {
        if (!await TryInitializeAsync() || !_playerProfileService.HasNickname)
            return;

        try
        {
            string userId = _playerProfileService.NicknameKey;
            string userName = _playerProfileService.Nickname;

            var bestSnapshot = await _databaseRef.Parent.Child(LeaderboardPath).Child(userId).GetValueAsync();
            var bestEntry = bestSnapshot.Exists
                ? JsonUtility.FromJson<LeaderboardEntry>(bestSnapshot.GetRawJsonValue())
                : null;

            if (bestEntry != null && bestEntry.score >= score)
                return;

            var entry = new LeaderboardEntry(userId, userName, score);
            string json = JsonUtility.ToJson(entry);
            await _databaseRef.Parent.Child(LeaderboardPath).Child(userId).SetRawJsonValueAsync(json);
        }
        catch (Exception exception)
        {
            LogFirebaseError("submit score", exception);
        }
    }

    public async Task<List<LeaderboardEntry>> GetTopScores(int count)
    {
        var result = new List<LeaderboardEntry>();
        if (!await TryInitializeAsync())
            return result;

        try
        {
            var snapshot = await _databaseRef.Parent.Child(LeaderboardPath).OrderByChild("score").LimitToLast(count).GetValueAsync();
            foreach (var child in snapshot.Children)
            {
                var json = child.GetRawJsonValue();
                var entry = JsonUtility.FromJson<LeaderboardEntry>(json);
                if (entry != null)
                    result.Insert(0, entry);
            }
        }
        catch (Exception exception)
        {
            LogFirebaseError("load top scores", exception);
        }

        return result;
    }

    public async Task<int> GetPlayerRank()
    {
        if (!await TryInitializeAsync() || !_playerProfileService.HasNickname)
            return -1;

        var topScores = await GetTopScores(1000);

        for (int i = 0; i < topScores.Count; i++)
        {
            if (topScores[i].userId == _playerProfileService.NicknameKey)
                return i + 1;
        }

        return -1;
    }

    public async Task<LeaderboardEntry> GetPlayerBestScore()
    {
        if (!await TryInitializeAsync() || !_playerProfileService.HasNickname)
            return null;

        try
        {
            var snapshot = await _databaseRef.Child(_playerProfileService.NicknameKey).GetValueAsync();
            return snapshot.Exists
                ? JsonUtility.FromJson<LeaderboardEntry>(snapshot.GetRawJsonValue())
                : null;
        }
        catch (Exception exception)
        {
            LogFirebaseError("load player best score", exception);
            return null;
        }
    }

    private async Task<bool> TryInitializeAsync()
    {
        try
        {
            await InitializeAsync();
            return _isInitialized;
        }
        catch (Exception exception)
        {
            LogFirebaseError("initialize Firebase", exception);
            return false;
        }
    }

    private void LogFirebaseError(string operation, Exception exception)
    {
        Debug.LogWarning($"Failed to {operation}. Check Firebase Realtime Database rules for unauthenticated leaderboard access. {exception.Message}");
    }
}

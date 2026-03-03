using System;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string userId;
    public string username;
    public int score;

    public LeaderboardEntry() { }
    public LeaderboardEntry(
        string userId,
        string username,
        int score)
    {
        this.userId = userId;
        this.username = username;
        this.score = score;
    }
}

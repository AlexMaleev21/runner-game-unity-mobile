using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void SetData(int rank, string name, int score)
    {
        _rankText.text = rank.ToString();
        _nameText.text = name;
        _scoreText.text = score.ToString();
    }
}

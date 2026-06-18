using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LeaderboardWindow : BaseWindow
{
    [SerializeField] private RectTransform _contentParent;
    [SerializeField] private LeaderboardItem _itemPrefab;
    [SerializeField] private LeaderboardItem _playerItem;
    [SerializeField] private Button _closeButton;

    private ILeaderboardService _leaderboardService;
    private int _leaderboardItemsCount = 5;
    private List<LeaderboardItem> _leaderboardItems = new List<LeaderboardItem>();

    [Inject]
    public void Construct(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(Hide);
        Hide();
    }

    public override async void Show()
    {
        base.Show();
        await UpdateLeaderboard();
    }

    public async Task UpdateLeaderboard()
    {
        var topScores = await _leaderboardService.GetTopScores(_leaderboardItemsCount);

        while (_leaderboardItems.Count < topScores.Count)
        {
            var item = Instantiate(_itemPrefab, _contentParent);
            _leaderboardItems.Add(item);
        }

        for (int i = 0; i < topScores.Count; i++)
        {
            _leaderboardItems[i].SetData(i + 1, topScores[i].username, topScores[i].score);
            _leaderboardItems[i].gameObject.SetActive(true);
        }

        for (int i = topScores.Count; i < _leaderboardItems.Count; i++)
        {
            _leaderboardItems[i].gameObject.SetActive(false);
        }

        var best = await _leaderboardService.GetPlayerBestScore();
        if (best != null)
        {
            int rank = await _leaderboardService.GetPlayerRank();
            _playerItem.SetData(rank, best.username, best.score);
            _playerItem.gameObject.SetActive(true);
        }
        else
        {
            _playerItem.gameObject.SetActive(false);
        }
    }
}

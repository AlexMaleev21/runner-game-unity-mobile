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
    private IAuthService _authService;
    private bool _isInitialized = false;
    private int _leaderboardItemsCount = 5;
    private List<LeaderboardItem> _leaderboardItems = new List<LeaderboardItem>();

    [Inject]
    public void Construct(ILeaderboardService leaderboardService, IAuthService authService)
    {
        _leaderboardService = leaderboardService;
        _authService = authService;
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(Hide);
        _isInitialized = false;
        Hide();
    }

    public override async void Show()
    {
        base.Show();
        await UpdateLeaderboard();
    }

    public async Task UpdateLeaderboard()
    {
        Debug.Log("1");
        var topScores = await _leaderboardService.GetTopScores(_leaderboardItemsCount);
        Debug.Log(topScores.ToString());
        if (!_isInitialized)
        {
            for(int i = 0; i < topScores.Count; i++)
            {
                var item = Instantiate(_itemPrefab, _contentParent);
                _leaderboardItems.Add(item);
            }
            _isInitialized = true;
        }
        //foreach (Transform child in _contentParent)
        //{
        //    Destroy(child.gameObject);
        //}

        for (int i = 0; i < topScores.Count; i++)
        {
            _leaderboardItems[i].SetData(i + 1, topScores[i].username, topScores[i].score);
            _leaderboardItems[i].gameObject.SetActive(true);
        }

        if (_authService.IsAuthenticated)
        {
            var best = await _leaderboardService.GetPlayerBestScore(_authService.UserId);
            int rank = await _leaderboardService.GetPlayerRank(_authService.UserId);
            _playerItem.SetData(rank, best.username, best.score);
            Debug.Log("2");
        }
    }
}
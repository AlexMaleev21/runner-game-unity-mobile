using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AdsManager : IInitializable
{
    public event Action OnRewardedAdCompleted;

    private readonly AdmobAdsService _adsService;
    private readonly PlayerController _player;
    private readonly ObstacleManipulator _obstacleMover;
    private readonly ObstacleSpawner _spawner;
    private readonly SpeedManager _speedManager;

    public AdsManager(
        AdmobAdsService adsService,
        PlayerController player,
        ObstacleManipulator obstacleMover,
        ObstacleSpawner spawner,
        SpeedManager speedManager)
    {
        _adsService = adsService;
        _player = player;
        _obstacleMover = obstacleMover;
        _spawner = spawner;
        _speedManager = speedManager;
    }

    public void Initialize()
    {
        _adsService.Initialize();
    }

    public bool IsAdReady => _adsService.IsRewardedAdReady;

    public void ShowRewardedAd()
    {
        if (!IsAdReady)
        {
            return;
        }

        _adsService.ShowRewardedAd(
            onRewarded: OnAdRewarded,
            onFailed: () =>
            {
                Debug.LogError("Ad display gone wrong");
            }
        );
    }
    private void OnAdRewarded()
    {
        OnRewardedAdCompleted?.Invoke();
    }
}
using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AdsManager : IInitializable
{
    public event Action OnGameContinue;

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

    private async void OnAdRewarded()
    {
        _obstacleMover.RemoveClosestObstacle(_player.transform.position.z);

        _spawner.PauseSpawn(1f);

        _player.SetEnabled(false);
        await Task.Delay(2500);

        OnGameContinue.Invoke();

        _obstacleMover.ResumeGame();
        _speedManager.Resume();
        _player.SetEnabled(true);
        _player.StateMachine.ChangeState(PlayerStateType.Running);

    }
}
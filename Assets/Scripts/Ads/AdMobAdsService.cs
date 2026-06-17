using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdmobAdsService
{
    private RewardedAd _rewardedAd;
    private const string _rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    private Action _onRewardedCallback;
    private Action _onFailedCallback;

    public bool IsRewardedAdReady => _rewardedAd != null && _rewardedAd.CanShowAd();

    public AdmobAdsService()
    {
        
    }

    public void Initialize()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
        });
    }

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        var adRequest = new AdRequest();
        RewardedAd.Load(_rewardedAdUnitId, adRequest, (ad, error) =>
        {
            if (ad == null || error != null)
            {
                return;
            }

            _rewardedAd = ad;

            _rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                LoadRewardedAd();
            };
            _rewardedAd.OnAdFullScreenContentFailed += (error) =>
            {
                _onFailedCallback?.Invoke();
                LoadRewardedAd();
            };
        });
    }

    public void ShowRewardedAd(Action onRewarded, Action onFailed)
    {
        _onRewardedCallback = onRewarded;
        _onFailedCallback = onFailed;

        if (!IsRewardedAdReady)
        {
            onFailed?.Invoke();
            LoadRewardedAd();
            return;
        }

        _rewardedAd.Show((reward) =>
        {
            _onRewardedCallback?.Invoke();
        });
    }
}
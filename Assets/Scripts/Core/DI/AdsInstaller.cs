using UnityEngine;
using Zenject;

public class AdsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AdmobAdsService>().AsSingle();
    }
}

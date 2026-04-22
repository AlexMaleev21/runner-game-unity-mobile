using UnityEngine;
using Zenject;

public class CoinInstaller : MonoInstaller
{
    [SerializeField] private CoinSpawnConfig _coinSpawnConfig;

    public override void InstallBindings()
    {
        Container.BindInstance(_coinSpawnConfig);
        Container.Bind<CoinFactory>().AsSingle();
        Container.Bind<CoinPool>().AsSingle();
        Container.BindInterfacesAndSelfTo<CoinMover>().AsSingle();
        Container.BindInterfacesAndSelfTo<CoinSpawner>().AsSingle();
        Container.Bind<CoinManager>().AsSingle();
    }
}

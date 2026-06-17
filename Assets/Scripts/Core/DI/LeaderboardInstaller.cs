using UnityEngine;
using Zenject;

public class LeaderboardInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ILeaderboardService>().To<FirebaseLeaderboardService>().AsSingle();
    }
}

using UnityEngine;
using Zenject;

public class ManagersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SpeedManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle();
        Container.Bind<PlayerProfileService>().AsSingle();
        Container.BindInterfacesAndSelfTo<AdsManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<MenuManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameplayManager>().AsSingle();
        Container.BindInterfacesTo<GameStateController>().AsSingle();
        Container.Bind<NicknamePromptWindow>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}

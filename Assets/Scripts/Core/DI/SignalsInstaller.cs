using Zenject;
using Zenject.SpaceFighter;

public class SignalsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<PlayerDiedSignal>();
        Container.DeclareSignal<ScoreUpdatedSignal>();
        Container.DeclareSignal<AuthSuccessSignal>();
        Container.DeclareSignal<GameResumedSignal>();
    }
}

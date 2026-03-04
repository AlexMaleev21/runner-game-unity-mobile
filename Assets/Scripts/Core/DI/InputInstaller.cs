using UnityEngine;
using Zenject;

public class InputInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
#if UNITY_EDITOR
        Container.Bind<IInputStrategy>().To<EditorInputStrategy>().AsSingle();
#else
        Container.Bind<IInputStrategy>().To<MobileInputStrategy>().AsSingle();
#endif

        Container.BindInterfacesTo<InputHandler>().AsSingle();
    }
}

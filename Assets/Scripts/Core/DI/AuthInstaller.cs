using UnityEngine;
using Zenject;

public class AuthInstaller : MonoInstaller
{

    public override void InstallBindings()
    {
        Container.Bind<IAuthService>().To<FirebaseAuthService>().AsSingle();
    }
}

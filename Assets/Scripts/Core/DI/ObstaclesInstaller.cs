using UnityEngine;
using Zenject;

public class ObstaclesInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IObstacleFactory>().To<ObstacleFactory>().AsSingle();
        Container.Bind<ObstaclePool>().AsSingle();
        Container.Bind<ObstacleManipulator>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<ObstacleSpawner>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<BackgroundMover>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}
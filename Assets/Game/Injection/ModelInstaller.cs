
using System.Diagnostics;
using Zenject;

public class ModelInstaller : Installer<ModelInstaller>
{
    public override void InstallBindings()
    {
        UnityEngine.Debug.Log("Installing!");
        Container.Bind<IGameManager>().To<GameManager>().FromNew().AsSingle();
    }
}
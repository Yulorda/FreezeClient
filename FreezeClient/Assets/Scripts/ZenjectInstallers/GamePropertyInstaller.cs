using UnityEngine;
using Zenject;

public class GamePropertyInstaller : MonoInstaller
{
    [SerializeField]
    private GameProperty gameProperty;

    public override void InstallBindings()
    {
        Container.Bind<GameProperty>().FromInstance(Instantiate(gameProperty)).AsSingle();
    }
}
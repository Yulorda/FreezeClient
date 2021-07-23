using UnityEngine;
using Zenject;

public class GamePropertyInstaller : MonoInstaller
{
    [SerializeField]
    private GridSize gameProperty;

    public override void InstallBindings()
    {
        Container.Bind<GridSize>().FromInstance(Instantiate(gameProperty)).AsSingle();
    }
}
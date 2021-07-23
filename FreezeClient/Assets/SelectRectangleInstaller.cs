using UnityEngine;
using Zenject;

public class SelectRectangleInstaller : MonoInstaller
{
    SelectionRectangleModel rectangleModel;

    public override void InstallBindings()
    {
        rectangleModel = new SelectionRectangleModel(Camera.main);
        Container.Bind<SelectionRectangleModel>().FromInstance(rectangleModel);
    }

    private void OnDestroy()
    {
        rectangleModel.Dispose();
    }
}

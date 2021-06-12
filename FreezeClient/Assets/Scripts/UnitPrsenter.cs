using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class UnitPrsenter : MonoBehaviour, IDisposable
{
    private MeshRenderer meshRenderer;

    [Inject]
    private GameProperty gameProperty;

    public List<IDisposable> disposables = new List<IDisposable>();

    public void InjectModel(Unit unit)
    {
        if (!meshRenderer)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        disposables.Add(unit.State.Subscribe(x =>
        {
            if (x == UnitState.Run)
                Run();
            else
                Stay();
        }));

        disposables.Add(unit.Position.Subscribe(x =>
            transform.position = new Vector3(x.x * gameProperty.distance.x, transform.position.y, x.y * gameProperty.distance.y)));

        transform.position = unit.Position.Value;
        gameObject.SetActive(true);
    }

    public void Dispose()
    {
        disposables.ForEach(x => x.Dispose());
        disposables.Clear();
        DestroyImmediate(this.gameObject);
    }

    [ContextMenu(nameof(Run))]
    private void Run()
    {
        meshRenderer.material.SetColor("_Color", Color.green);
    }

    [ContextMenu(nameof(Stay))]
    private void Stay()
    {
        meshRenderer.material.SetColor("_Color", Color.white);
    }
}
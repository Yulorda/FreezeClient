using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class UnitPrsenter : MonoBehaviour, IDisposable
{
    public List<IDisposable> disposables = new List<IDisposable>();

    private MeshRenderer meshRenderer;

    [Inject]
    private GameProperty gameProperty;

    public void InjectModel(Unit unit)
    {
        if (!meshRenderer)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        disposables.Add(unit.State.Subscribe(x => ChangeColor()));
        disposables.Add(unit.Selected.Subscribe(x => ChangeColor()));

        disposables.Add(unit.Position.Subscribe(position => transform.position = (unit.Position.Value * gameProperty.distance).XZ()));

        transform.position = (unit.Position.Value * gameProperty.distance).XZ();

        gameObject.SetActive(true);

        void ChangeColor()
        {
            if (!unit.Selected.Value)
            {
                if (unit.State.Value == UnitState.Run)
                {
                    meshRenderer.material.SetColor("_Color", Color.green);
                }
                else
                {
                    meshRenderer.material.SetColor("_Color", Color.white);
                }
            }
            else
            {
                meshRenderer.material.SetColor("_Color", Color.red);
            }
        }
    }

    public void Dispose()
    {
        disposables.ForEach(x => x.Dispose());
        disposables.Clear();
        DestroyImmediate(this.gameObject);
    }
}
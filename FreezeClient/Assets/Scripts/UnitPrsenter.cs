using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UnitPrsenter : MonoBehaviour, IDisposable
{
    [SerializeField]
    private Animation stayAnimation;
    [SerializeField]
    private Animation runAnimation;

    public List<IDisposable> disposables = new List<IDisposable>();

    public void InjectModel(Unit unit)
    {
        disposables.Add(unit.State.Subscribe(x =>
        {
            if (x == UnitState.Run)
                Run();
            else
                Stay();
        }));
        disposables.Add(unit.Position.Subscribe(x => transform.position = x));
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
        stayAnimation.Stop();
        runAnimation.Play();
    }

    [ContextMenu(nameof(Stay))]
    private void Stay()
    {
        runAnimation.Stop();
        stayAnimation.Play();
    }
}
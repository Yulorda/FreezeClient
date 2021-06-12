using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public enum UnitState
{
    Stay,
    Run
}

public class Unit : IDisposable
{
    public ReactiveProperty<UnitState> State { get; private set;} = new ReactiveProperty<UnitState>(UnitState.Stay);
    public ReactiveProperty<Vector2> Position { get; private set; } = new ReactiveProperty<Vector2>();
    public List<IDisposable> disposables = new List<IDisposable>();

    public Unit()
    {

    }

    public Unit(Vector2 position, UnitState state)
    {
        MoveTo(position, state);
    }

    public void MoveTo(Vector2 position, UnitState state)
    {
        this.State.Value = state;
        this.Position.Value = position;
    }

    public void Dispose()
    {
        disposables.ForEach(x => x.Dispose());
        disposables.Clear();
    }
}
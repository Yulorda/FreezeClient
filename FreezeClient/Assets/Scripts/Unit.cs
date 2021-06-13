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
    public readonly int id;
    public ReactiveProperty<UnitState> State { get; private set; } = new ReactiveProperty<UnitState>(UnitState.Stay);
    public ReactiveProperty<Vector2> Position { get; private set; } = new ReactiveProperty<Vector2>();
    public ReactiveProperty<bool> Selected { get; private set; } = new ReactiveProperty<bool>();

    public List<IDisposable> disposables = new List<IDisposable>();

    public Unit(int id)
    {
        this.id = id;
    }

    public Unit(int id, Vector2 position, UnitState state) : this(id)
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
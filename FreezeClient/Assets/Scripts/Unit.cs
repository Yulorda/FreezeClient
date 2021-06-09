using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Unit : IDisposable
{
    public enum MovingState
    {
        Stay,
        Run
    }

    public int Id { get; private set; }
    public ReactiveProperty<MovingState> State { get; private set;} = new ReactiveProperty<MovingState>(MovingState.Stay);
    public ReactiveProperty<Vector3> Position { get; private set; } = new ReactiveProperty<Vector3>();
    public List<IDisposable> disposables = new List<IDisposable>();

    public Unit()
    {

    }

    public Unit(int id)
    {
        Id = id; 
    }

    public void MoveTo(Vector3 position, MovingState state)
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
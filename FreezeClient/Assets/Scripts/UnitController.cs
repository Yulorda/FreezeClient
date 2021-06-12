using Packages;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using Zenject;

public class UnitController : MonoBehaviour
{
    public Transform content;
    public UnitPrsenter prefab;

    private GameProperty gameProperty;
    private NetworkClient networkClient;

    private Dictionary<int, Unit> units = new Dictionary<int, Unit>();

    [Inject]
    public void Inject(GameProperty gameProperty, NetworkClient networkClient)
    {
        this.gameProperty = gameProperty;
        this.networkClient = networkClient;
    }

    private void Start()
    {
        networkClient.AddListener<UnitMoveState>(OnPackageReceive);
        networkClient.AddListener<UnitLifeState>(OnPackageReceive);
    }

    private void OnDestroy()
    {
        networkClient.RemoveListener<UnitMoveState>(OnPackageReceive);
        networkClient.RemoveListener<UnitLifeState>(OnPackageReceive);
    }

    private void OnPackageReceive(UnitMoveState unitPackage)
    {
        if (units.TryGetValue(unitPackage.id, out var value))
        {
            value.MoveTo(unitPackage.position, unitPackage.state);
        }
    }

    private void OnPackageReceive(UnitLifeState unitPackage)
    {
        Unit unit;
        switch (unitPackage.lifeStatus)
        {
            case UnitLifeStatus.Create:
                unit = new Unit(unitPackage.unitState.position, unitPackage.unitState.state);
                units.Add(unitPackage.id, unit);
                CreatePresnterForUnit(unit);
                break;

            case UnitLifeStatus.Delete:
                if (units.TryGetValue(unitPackage.id, out unit))
                {
                    unit.Dispose();
                    units.Remove(unitPackage.id);
                }
                break;
        }
    }

    private void CreatePresnterForUnit(Unit unit)
    {
        var presenter = Instantiate(prefab, content);
        presenter.InjectModel(unit);
        unit.disposables.Add(presenter);
    }
}

namespace Packages
{
    [Serializable]
    public class UnitMoveState : ISerializable
    {
        public int id;

        [NonSerialized]
        public Vector2 position;

        public UnitState state;

        public UnitMoveState()
        {
        }

        public UnitMoveState(SerializationInfo info, StreamingContext context)
        {
            position.x = (float)info.GetValue("x", typeof(float));
            position.y = (float)info.GetValue("y", typeof(float));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("x", position.x);
            info.AddValue("y", position.y);
        }
    }

    [Serializable]
    public enum UnitLifeStatus
    {
        Create,
        Delete
    }

    [Serializable]
    public class UnitLifeState
    {
        public int id;
        public UnitLifeStatus lifeStatus;
        public UnitMoveState unitState;
    }
}
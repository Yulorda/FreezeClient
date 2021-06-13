using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UniRx;
using UnityEngine;
using Zenject;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    [SerializeField]
    private UnitPrsenter prefab;

    [SerializeField]
    private SelectionRectangleModel selectionRectangleModel;

    private Dictionary<int, Unit> units = new Dictionary<int, Unit>();
    private NetworkClient networkClient;
    private List<IDisposable> disposables = new List<IDisposable>();

    [Inject]
    public void Inject(NetworkClient networkClient, GameProperty gameProperty)
    {
        this.networkClient = networkClient;

        selectionRectangleModel.onRectangleSizeChange.Subscribe(x =>
        {
            OnSelection();
        });

        selectionRectangleModel.state.Subscribe(x =>
        {
            OnSelection();
        });

        void OnSelection()
        {
            var selectedMode = selectionRectangleModel.state.Value == SelectionState.Drag;
            if (selectedMode)
            {
                foreach (var unit in units.Values)
                {
                    var unitPos = unit.Position.Value * gameProperty.distance;
                    unit.Selected.Value = selectionRectangleModel.IsRectangleContains(unitPos.XZ());
                }
            }
        }
    }

    public List<Unit> GetSelectedUnits()
    {
        return units.Values.Where(x => x.Selected.Value).ToList();
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
        disposables.ForEach(x => x.Dispose());
        disposables.Clear();
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
                unit = new Unit(unitPackage.id, unitPackage.unitState.position, unitPackage.unitState.state);
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
        position.x = info.GetInt32("x");
        position.y = info.GetInt32("y");
        state = (UnitState)info.GetValue("state", typeof(UnitState));
        id = info.GetInt32("id");
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("x", (int)position.x);
        info.AddValue("y", (int)position.y);
        info.AddValue("state", state);
        info.AddValue("id", id);
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
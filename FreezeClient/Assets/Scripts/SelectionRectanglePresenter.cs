using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SelectionRectanglePresenter : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    private List<IDisposable> disposables = new List<IDisposable>();

    private void Awake()
    {
        Inject(GetComponentInParent<SelectionRectangleModel>());
    }

    private void OnDestroy()
    {
        disposables.ForEach(x => x.Dispose());
        disposables.Clear();
    }

    public void Inject(SelectionRectangleModel model)
    {
        disposables.Add(model.onRectangleSizeChange.Subscribe(x =>
        {
            OnPositionChange(model.RectanglePosition);
            OnSizeChange(model.SizeDelta);
        }));

        disposables.Add(model.state.Subscribe(OnStateChange));
    }

    private void OnStateChange(SelectionState selectionState)
    {
        rectTransform.gameObject.SetActive(selectionState == SelectionState.Drag);
    }

    private void OnPositionChange(Vector3 value)
    {
        rectTransform.position = value;
    }

    private void OnSizeChange(Vector3 value)
    {
        rectTransform.sizeDelta = value;
    }
}
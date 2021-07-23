using UniRx;
using UnityEngine;
using Zenject;

public class SelectionRectanglePresenter : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    [Inject]
    public void Init(SelectionRectangleModel model)
    {
        model.onRectangleSizeChange.Subscribe(x =>
        {
            OnPositionChange(model.RectanglePosition);
            OnSizeChange(model.SizeDelta);
        }).AddTo(this.gameObject);

        model.state.Subscribe(OnStateChange).AddTo(this.gameObject);
    }

    private void OnStateChange(SelectionState selectionState)
    {
        rectTransform.gameObject.SetActive(selectionState == SelectionState.Draging);
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
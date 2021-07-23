using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum SelectionState
{
    None,
    StartDragging,
    Draging,
    EndDrag,
}

public class SelectionRectangleModel : IDisposable
{
    public ReactiveCommand onRectangleSizeChange = new ReactiveCommand();
    public ReactiveProperty<SelectionState> state = new ReactiveProperty<SelectionState>();

    public Vector2 RectanglePosition { get; private set; } = new Vector3();
    public Vector2 SizeDelta { get; private set; } = new Vector3();

    private Vector2 rectangleStartPosition;
    private Vector3 TL, TR, BL, BR;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private InputData inputData;
    private IDisposable dragging;
    private Camera camera;

    private List<IDisposable> disposables = new List<IDisposable>();

    public bool IsRectangleContains(Vector3 unitPos)
    {
        bool isWithinPolygon = false;
        if (IsTriangleContains(unitPos, TL, BL, TR))
        {
            return true;
        }
        if (IsTriangleContains(unitPos, TR, BL, BR))
        {
            return true;
        }
        return isWithinPolygon;
    }

    public SelectionRectangleModel(Camera camera)
    {
        this.camera = camera;

        inputData = new InputData();
        inputData.Enable();

        disposables.Add(Observable.FromEvent<InputAction.CallbackContext>(
             e => inputData.UI.Hold.started += e,
             e => inputData.UI.Hold.started -= e
             )
             .Subscribe(x => OnStarted(x)));

        disposables.Add(Observable.FromEvent<InputAction.CallbackContext>(
                e => inputData.UI.Hold.performed += e,
                e => inputData.UI.Hold.performed -= e
                )
                .Subscribe(x => OnPressed(x)));

        disposables.Add(Observable.FromEvent<InputAction.CallbackContext>(
             e => inputData.UI.Hold.canceled += e,
             e => inputData.UI.Hold.canceled -= e
             )
             .Subscribe(x => OnEnd(x)));
    }

    private void OnStarted(InputAction.CallbackContext obj)
    {
        state.Value = SelectionState.None;
        var pointPosition = inputData.UI.Point.ReadValue<Vector2>();

        //TODO вынести в отдельный класс
        bool ignoreThisMovementBecauseOfRect = !PositionInCameraRect(pointPosition.x, pointPosition.y);
        bool ignoreThisMovementBecauseOfUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        bool thisMovementShouldBeIgnored = ignoreThisMovementBecauseOfRect || ignoreThisMovementBecauseOfUI;
        if (thisMovementShouldBeIgnored)
            return;

        rectangleStartPosition = pointPosition;
        state.Value = SelectionState.StartDragging;
    }

    private void OnPressed(InputAction.CallbackContext obj)
    {
        if (state.Value != SelectionState.StartDragging)
        {
            return;
        }

        state.Value = SelectionState.Draging;
        dragging = Observable.EveryEndOfFrame().Subscribe(x => ChangeRectangleView());
    }

    private void OnEnd(InputAction.CallbackContext obj)
    {
        if (state.Value == SelectionState.Draging)
            state.Value = SelectionState.EndDrag;

        if (dragging != null)
            dragging.Dispose();
    }

    private bool IsTriangleContains(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        bool result = false;

        float denominator = ((p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z));

        float a = ((p2.z - p3.z) * (p.x - p3.x) + (p3.x - p2.x) * (p.z - p3.z)) / denominator;
        float b = ((p3.z - p1.z) * (p.x - p3.x) + (p1.x - p3.x) * (p.z - p3.z)) / denominator;
        float c = 1 - a - b;

        if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
        {
            result = true;
        }

        return result;
    }

    private void ChangeRectangleView()
    {
        var rectangleEndPos = inputData.UI.Point.ReadValue<Vector2>();
        Vector2 middle = (rectangleStartPosition + rectangleEndPos) / 2f;
        RectanglePosition = middle;

        float sizeX = Mathf.Abs(rectangleStartPosition.x - rectangleEndPos.x);
        float sizeY = Mathf.Abs(rectangleStartPosition.y - rectangleEndPos.y);

        SizeDelta = new Vector2(sizeX, sizeY);
        float halfSizeX = sizeX * 0.5f;
        float halfSizeY = sizeY * 0.5f;

        Vector3 TL_screenSpace = new Vector3(middle.x - halfSizeX, middle.y + halfSizeY, 0f);
        Vector3 TR_screenSpace = new Vector3(middle.x + halfSizeX, middle.y + halfSizeY, 0f);
        Vector3 BL_screenSpace = new Vector3(middle.x - halfSizeX, middle.y - halfSizeY, 0f);
        Vector3 BR_screenSpace = new Vector3(middle.x + halfSizeX, middle.y - halfSizeY, 0f);

        Ray rayTL = Camera.main.ScreenPointToRay(TL_screenSpace);
        Ray rayTR = Camera.main.ScreenPointToRay(TR_screenSpace);
        Ray rayBL = Camera.main.ScreenPointToRay(BL_screenSpace);
        Ray rayBR = Camera.main.ScreenPointToRay(BR_screenSpace);

        float distanceToPlane = 0f;

        if (groundPlane.Raycast(rayTL, out distanceToPlane))
        {
            TL = rayTL.GetPoint(distanceToPlane);
        }
        if (groundPlane.Raycast(rayTR, out distanceToPlane))
        {
            TR = rayTR.GetPoint(distanceToPlane);
        }
        if (groundPlane.Raycast(rayBL, out distanceToPlane))
        {
            BL = rayBL.GetPoint(distanceToPlane);
        }
        if (groundPlane.Raycast(rayBR, out distanceToPlane))
        {
            BR = rayBR.GetPoint(distanceToPlane);
        }

        onRectangleSizeChange.Execute();
    }

    private bool PositionInCameraRect(float x, float y)
    {
        return x >= Screen.width * camera.rect.xMin && y >= Screen.height * camera.rect.yMin && x <= Screen.width * camera.rect.xMax && y <= Screen.height * camera.rect.yMax;
    }

    public void Dispose()
    {
        disposables.ForEach(x => x.Dispose());
        disposables.Clear();
    }
}
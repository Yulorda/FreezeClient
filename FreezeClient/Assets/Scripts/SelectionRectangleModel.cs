using UniRx;
using UnityEngine;

public enum SelectionState
{
    None,
    Click,
    Drag,
    Up
}

public class SelectionRectangleModel : MonoBehaviour
{
    [HideInInspector]
    public ReactiveProperty<SelectionState> state = new ReactiveProperty<SelectionState>();

    public ReactiveCommand onRectangleSizeChange = new ReactiveCommand();

    public Vector3 RectanglePosition { get; private set; } = new Vector3();
    public Vector3 SizeDelta { get; private set; } = new Vector3();

    private float delay = 0.15f;
    private float lastClickTime = 0f;
    private Vector3 rectangleStartPosition;
    private Vector3 TL, TR, BL, BR;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    private void LateUpdate()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastClickTime = Time.time;
            rectangleStartPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - lastClickTime <= delay)
            {
                state.Value = SelectionState.Click;
            }
            else
            {
                state.Value = SelectionState.Up;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Time.time - lastClickTime > delay)
            {
                ChangeRectangleView();
                state.Value = SelectionState.Drag;
            }
        }
        else
        {
            state.Value = SelectionState.None;
        }
    }

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
        Vector3 rectangleEndPos = Input.mousePosition;
        Vector3 middle = (rectangleStartPosition + rectangleEndPos) / 2f;
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
}
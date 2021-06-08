using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private Vector2 distance;

    private List<GameObject> grid = new List<GameObject>();

    public void CreateBoard(int rank)
    {
        foreach (var element in grid)
        {
            DestroyImmediate(element);
        }
        grid.Clear();

        float deltaZ = 0f;
        for (int i = 0; i < rank; i++)
        {
            float deltaX = 0f;
            for (int j = 0; j < rank; j++)
            {
                var temp = Instantiate(prefab, new Vector3(deltaX,0, deltaZ), new Quaternion(), transform);
                deltaX += distance.x;
            }
            deltaZ += distance.y;
        }
    }

    [ContextMenu(nameof(CreateBoard5))]
    private void CreateBoard5()
    {
        CreateBoard(5);
    }
}
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [Inject]
    private NetworkClient networkClient;
    [Inject]
    private GridSize gameProperty;

    private List<GameObject> grid = new List<GameObject>();
    
    private void Awake()
    {
        networkClient.AddListener<BoardProperty>(OnPackageReceive).AddTo(this.gameObject);
    }

    private void OnPackageReceive(BoardProperty boardProperty)
    {
        CreateBoard(boardProperty.rank, gameProperty.gridSize);
    }

    public void CreateBoard(int rank, Vector2 distance)
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
                var temp = Instantiate(prefab, new Vector3(deltaX, 0, deltaZ), new Quaternion(), transform);
                deltaX += distance.x;
            }
            deltaZ += distance.y;
        }
    }
}

[Serializable]
public class BoardProperty
{
    public int rank;
}
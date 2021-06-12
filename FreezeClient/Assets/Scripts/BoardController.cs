using Packages;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BoardController : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private List<GameObject> grid = new List<GameObject>();

    private NetworkClient networkClient;
    private GameProperty gameProperty;

    [Inject]
    public void Inject(NetworkClient networkClient, GameProperty gameProperty)
    {
        this.networkClient = networkClient;
        this.gameProperty = gameProperty;
    }

    private void Start()
    {
        networkClient.AddListener<BoardProperty>(OnPackageReceive);
    }

    private void OnDestroy()
    {
        networkClient.RemoveListener<BoardProperty>(OnPackageReceive);
    }

    private void OnPackageReceive(BoardProperty boardProperty)
    {
        CreateBoard(boardProperty.rank, gameProperty.distance);
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

    [ContextMenu(nameof(CreateBoard5))]
    private void CreateBoard5()
    {
        CreateBoard(5, new Vector2(10, 10));
    }
}

namespace Packages
{
    public class BoardProperty
    {
        public int rank;
    }
}
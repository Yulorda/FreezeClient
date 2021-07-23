using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PZDC
{
    public class MoveTaskCreator : MonoBehaviour
    {
        [SerializeField]
        private UnitController unitController;

        [SerializeField]
        private LayerMask layerMask;

        private NetworkClient networkClient;
        private GridSize gameProperty;

        [Inject]
        public void Inject(NetworkClient networkClient, GridSize gameProperty)
        {
            this.networkClient = networkClient;
            this.gameProperty = gameProperty;
        }

        private void Update()
        {
            return;
            if (Input.GetMouseButtonUp(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 1000, layerMask))
                {
                    var position = (hit.collider.transform.position).XZ() / gameProperty.gridSize;
                    networkClient.Send(new MoveTask()
                    {
                        unitMoveStates = unitController.GetSelectedUnits().Select(x => new UnitMoveState() { id = x.id, position = position, state = UnitState.Run }).ToList()
                    });
                }
            }
        }
    }

    [Serializable]
    public class MoveTask
    {
        public List<UnitMoveState> unitMoveStates = new List<UnitMoveState>();
    }
}
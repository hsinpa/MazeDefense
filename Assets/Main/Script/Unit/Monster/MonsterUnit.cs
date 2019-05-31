using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;

namespace TD.Unit {
    public class MonsterUnit : MonoBehaviour, BaseUnit
    {
        public GameObject unitObject { get { return gameObject; } }

        MapGrid _mapGrid;

        Vector3 moveDelta;

        public void SetUp(MapGrid mapGrid)
        {
            _mapGrid = mapGrid;
        }

        public void ReadyToAction(System.Action<BaseUnit> OnDestroyCallback)
        {

        }

        public void OnUpdate()
        {
            if (_mapGrid == null) return;

            var unitPosition = transform.position;
            var currentTile = _mapGrid.GetTileNodeByWorldPos(transform.position);

            if (currentTile.TilemapMember != null)
            {

                moveDelta.Set((currentTile.FlowFieldDirection.x), (currentTile.FlowFieldDirection.y), 0);
                moveDelta *= Time.deltaTime;


                transform.position += moveDelta;
            }
        }

        public void Destroy()
        {

        }

    }
}
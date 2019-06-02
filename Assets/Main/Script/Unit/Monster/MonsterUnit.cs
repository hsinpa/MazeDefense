using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;

namespace TD.Unit {
    public class MonsterUnit : MonoBehaviour, UnitInterface
    {
        public GameObject unitObject { get { return gameObject; } }

        private MapGrid _mapGrid;

        private Vector3 moveDelta;

        private System.Action<UnitInterface> OnDestroyCallback;

        private TileNode currentTile;

        public void SetUp(MapGrid mapGrid)
        {
            _mapGrid = mapGrid;
        }

        public void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback)
        {
            this.OnDestroyCallback = OnDestroyCallback;
        }

        public void OnUpdate()
        {
            if (_mapGrid == null) return;

            Vector3 unitPosition = transform.position;

            TileNode standTile = _mapGrid.GetTileNodeByWorldPos(transform.position);

            if (standTile.TilemapMember != null && standTile.GridIndex != currentTile.GridIndex)
            {
                if (currentTile.TilemapMember != null)
                    _mapGrid.EditUnitState(currentTile.GridIndex, this, false);

                _mapGrid.EditUnitState(standTile.GridIndex, this, true);
                currentTile = standTile;
            }

            moveDelta.Set((currentTile.FlowFieldDirection.x), (currentTile.FlowFieldDirection.y), 0);
            moveDelta *= Time.deltaTime;

            transform.position += moveDelta;
        }

        public void Destroy()
        {
            if (this.OnDestroyCallback != null)
                OnDestroyCallback(this);

            if (_mapGrid != null)
            {
                _mapGrid.EditUnitState(currentTile.GridIndex, this, false);
            }

            this.OnDestroyCallback = null;

            this.currentTile = default(TileNode);
        }

    }
}
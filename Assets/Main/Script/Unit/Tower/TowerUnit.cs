using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using TD.Map;

namespace TD.Unit {
    public class TowerUnit : MonoBehaviour, UnitInterface
    {
        STPTower _stpTower;
        MapGrid _mapGrid;
        System.Action<UnitInterface> OnDestroyCallback;

        [SerializeField]
        private Transform stationObject;

        [SerializeField]
        private Transform gunBodyObject;

        [SerializeField]
        private SpriteRenderer RangeIndicator;

        public GameObject unitObject { get => this.gameObject; }

        TileNode currentTileNode;

        public void SetUp(STPTower stpTower, MapGrid mapGrid) {
            _stpTower = stpTower;
            _mapGrid = mapGrid;
        }

        public void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback)
        {
            this.OnDestroyCallback = OnDestroyCallback;

            UpdateUnitState();

            if (_mapGrid != null)
                _mapGrid.OnMapReform += UpdateUnitState;
        }

        public void OnUpdate()
        {
            if (this.OnDestroyCallback != null && _mapGrid != null && currentTileNode.TilemapMember != null && _stpTower != null) {
 

            }
        }

        private void UpdateUnitState()
        {

            TileNode standTile = _mapGrid.GetTileNodeByWorldPos(unitObject.transform.position);

            if (standTile.TilemapMember != null && standTile.GridIndex != currentTileNode.GridIndex)
            {
                if (currentTileNode.TilemapMember != null)
                    _mapGrid.EditUnitState(currentTileNode.GridIndex, this, false);

                currentTileNode = standTile;

                _mapGrid.EditUnitState(standTile.GridIndex, this, true);
                _mapGrid.RefreshMonsterFlowFieldMap();
            }
        }

        public void Destroy()
        {
            if (this.OnDestroyCallback != null)
                OnDestroyCallback(this);

            if (_mapGrid != null) {
                _mapGrid.EditUnitState(currentTileNode.GridIndex, this, false);
                _mapGrid.OnMapReform -= UpdateUnitState;
                _mapGrid.RefreshMonsterFlowFieldMap();
            }

            this.OnDestroyCallback = null;
            this._stpTower = null;

            currentTileNode = default(TileNode);
        }


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using Pooling;

namespace TD.Unit {
    public class MonsterUnit : MonoBehaviour, UnitInterface
    {
        public GameObject unitObject { get { return gameObject; } }

        private MapGrid _mapGrid;

        private Vector3 moveDelta;

        private System.Action<UnitInterface> OnDestroyCallback;

        private TileNode currentTile;

        private STPMonster _stpMonster;

        private float hp;

        public bool isActive { get { return OnDestroyCallback != null; } }

        public void SetUp(STPMonster stpMonster, MapGrid mapGrid)
        {
            _stpMonster = stpMonster;
            _mapGrid = mapGrid;

            hp = _stpMonster.hp;
        }

        public void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback)
        {
            this.OnDestroyCallback = OnDestroyCallback;
        }

        public void OnUpdate()
        {
            if (_mapGrid == null || OnDestroyCallback == null) return;

            Vector3 unitPosition = transform.position;

            TileNode standTile = _mapGrid.GetTileNodeByWorldPos(transform.position);

            if (standTile.TilemapMember != null && standTile.GridIndex != currentTile.GridIndex )
            {
                if (currentTile.TilemapMember != null)
                    _mapGrid.EditUnitState(currentTile.GridIndex, this, false);

                _mapGrid.EditUnitState(standTile.GridIndex, this, true);
            }

            moveDelta.Set((currentTile.FlowFieldDirection.x), (currentTile.FlowFieldDirection.y), 0);
            moveDelta *= Time.deltaTime;

            transform.position += moveDelta;

            currentTile = standTile;
        }

        public void OnAttack(float damage)
        {
            hp -= damage;

            if (hp <= 0)
                Destroy();
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
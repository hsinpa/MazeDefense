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

        private MapBlockManager _mapBlockManager;

        private Vector3 moveDelta;

        private System.Action<UnitInterface> OnDestroyCallback;

        public TileNode currentTile { get { return _currentTile; } }

        private TileNode _currentTile;

        private BlockComponent currentBlockComp;

        private STPMonster _stpMonster;

        public bool isActive { get { return OnDestroyCallback != null; } }

        public float hp {
            get { return _hp; } 
        }
        float _hp;

        private enum StupidState
        {
            Idle, PathFirst, TowerFirst
        }

        private StupidState currentStatus;

        public void SetUp(STPMonster stpMonster, MapGrid mapGrid, MapBlockManager mapBlockManager)
        {
            _stpMonster = stpMonster;
            _mapGrid = mapGrid;
            _mapBlockManager = mapBlockManager;
            currentStatus = StupidState.PathFirst;

            _hp = _stpMonster.hp;
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

            if (standTile.TilemapMember != null && standTile.GridIndex != _currentTile.GridIndex )
            {
                if (_currentTile.TilemapMember != null)
                    _mapGrid.EditUnitState(_currentTile.GridIndex, this, false);

                _mapGrid.EditUnitState(standTile.GridIndex, this, true);
            }

            BlockComponent blockComponent = _mapBlockManager.GetMapComponentByPos(transform.position);
            if (blockComponent != null && currentBlockComp != blockComponent)
            {
                if (currentBlockComp == null || (!currentBlockComp.isMoving && !blockComponent.isMoving))
                {
                    currentBlockComp = blockComponent;

                    this.transform.SetParent(currentBlockComp.unitHolder);
                }
            }
            

            ChooseTactics(currentBlockComp);
            AgentMove();

            _currentTile = standTile;
        }

        private void AgentMove() {
            if (currentStatus == StupidState.PathFirst) {
                moveDelta.Set((_currentTile.FlowFieldDirection.x), (_currentTile.FlowFieldDirection.y), 0);
                moveDelta *= Time.deltaTime;

                transform.position += moveDelta;

            }
        }

        private void ChooseTactics(BlockComponent blockComponent) {
            if (blockComponent != null) {
                //Debug.Log(blockComponent.name);
                currentStatus = (blockComponent.isMoving) ? StupidState.Idle : StupidState.PathFirst;
            }
        }

        public void OnAttack(float damage)
        {
            _hp -= damage;

            if (_hp <= 0)
                Destroy();
        }

        public void Destroy()
        {
            if (this.OnDestroyCallback != null)
                OnDestroyCallback(this);

            if (_mapGrid != null)
            {
                _mapGrid.EditUnitState(_currentTile.GridIndex, this, false);
            }

            this.OnDestroyCallback = null;

            this._currentTile = default(TileNode);
        }

    }
}
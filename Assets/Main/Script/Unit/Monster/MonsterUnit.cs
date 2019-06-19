using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Database;
using Pooling;
using TD.AI;

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

        private MonsterStats _monsterStats;

        public bool isActive { get { return OnDestroyCallback != null; } }

        public float hp {
            get { return _hp; } 
        }
        float _hp;

        public enum ActiveState
        {
            Idle, Action, Attack
        }
        public ActiveState currentState;

        [SerializeField]
        private Animator _animator;

        private BaseStrategy _strategy;

        public void SetUp( MonsterStats monsterStats, BaseStrategy strategy, MapGrid mapGrid, MapBlockManager mapBlockManager)
        {
            _monsterStats = monsterStats;
            _strategy = strategy;
            _mapGrid = mapGrid;
            _mapBlockManager = mapBlockManager;
            currentState = ActiveState.Action;
            
            _hp = _monsterStats.hp;

            SetAnimator(monsterStats.animator);
        }

        public void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback)
        {
            this.OnDestroyCallback = OnDestroyCallback;
        }

        public void OnUpdate()
        {
            if (_mapGrid == null || OnDestroyCallback == null) return;

            Vector3 unitPosition = transform.position;

            //Update map information
            TileNode standTile = _mapGrid.GetTileNodeByWorldPos(unitPosition);

            if (standTile.TilemapMember != null && standTile.GridIndex != _currentTile.GridIndex )
            {
                if (_currentTile.TilemapMember != null)
                    _mapGrid.EditUnitState(_currentTile.GridIndex, this, false);

                _mapGrid.EditUnitState(standTile.GridIndex, this, true);
            }

            //Add self under mapObject 
            BlockComponent blockComponent = _mapBlockManager.GetMapComponentByPos(unitPosition);
            if (blockComponent != null && currentBlockComp != blockComponent)
            {
                if (currentBlockComp == null || (!currentBlockComp.isMoving && !blockComponent.isMoving))
                {
                    currentBlockComp = blockComponent;

                    this.transform.SetParent(currentBlockComp.unitHolder);
                }
            }
            
            UpdateActiveState(currentBlockComp);

            if (currentState == ActiveState.Action)
            {
                if (_strategy == null)
                    AgentMove();
                else {
                    //_strategy shouldn't put any where but here, all units share the same strategy object
                    _strategy.SetUp(this, _monsterStats, _mapGrid);
                    _strategy.Execute(_currentTile);
                }
            }

            _currentTile = standTile;
        }

        private void AgentMove() {
            moveDelta.Set((_currentTile.GetFlowFieldPath(VariableFlag.Strategy.CastleFirst).x), 
                            _currentTile.GetFlowFieldPath(VariableFlag.Strategy.CastleFirst).y, 0);

            moveDelta *= Time.deltaTime * _monsterStats.spd * 0.3f;

            transform.position += moveDelta;
        }

        private void UpdateActiveState(BlockComponent blockComponent) {
            if (blockComponent != null) {
                //Debug.Log(blockComponent.name);
                currentState = (blockComponent.isMoving) ? ActiveState.Idle : ActiveState.Action;
            }
        }

        private void SetAnimator(RuntimeAnimatorController targetAnimator) {
            if (targetAnimator != null && _animator != null) {

                _animator.runtimeAnimatorController = targetAnimator ;

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
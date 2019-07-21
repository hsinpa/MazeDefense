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

        #region Private Parameter
        private MapGrid _mapGrid;

        private MapBlockManager _mapBlockManager;

        private GameDamageManager _gameDamageManager;

        private Vector3 moveDelta;

        private System.Action<UnitInterface> OnDestroyCallback;

        private TileNode _currentTile;

        private BlockComponent currentBlockComp;

        private MonsterStats _monsterStats;

        [SerializeField]
        private Animator _animator;

        private BaseStrategy _strategy;
        private VariableFlag.Strategy _currentStrategy = VariableFlag.Strategy.None;
        private int _uniqueUnitID;
        #endregion

        #region Public Parameter
        public GameObject unitObject { get { return gameObject; } }

        public TileNode currentTile { get { return _currentTile; } }

        public UnitStats unitStats { get { return _monsterStats; } }

        public float hp
        {
            get { return _hp; }
        }
        float _hp;

        public enum ActiveState
        {
            Idle, Action
        }
        public ActiveState currentState;
        public bool isActive { get { return OnDestroyCallback != null; } }

        #endregion

        public void SetUp(MonsterStats monsterStats, BaseStrategy strategy, MapGrid mapGrid, MapBlockManager mapBlockManager,
                        GameDamageManager gameDamageManager)
        {
            _monsterStats = monsterStats;
            _strategy = strategy;
            _mapGrid = mapGrid;
            _mapBlockManager = mapBlockManager;
            _gameDamageManager = gameDamageManager;

            currentState = ActiveState.Action;

            _hp = _monsterStats.hp;
            _uniqueUnitID = gameObject.GetInstanceID();

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
            TileNode standTile = FindValidStandPosition(unitPosition);

            if (standTile.TilemapMember != null && standTile.GridIndex != _currentTile.GridIndex)
            {
                if (_currentTile.TilemapMember != null)
                    _mapGrid.EditUnitState(_currentTile.GridIndex.x, _currentTile.GridIndex.y, this, false);

                _mapGrid.EditUnitState(standTile.GridIndex.x, standTile.GridIndex.y, this, true);
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
                //Fallback function
                if (_strategy == null)
                    AgentMove();
                else
                {
                    //_strategy shouldn't put any where but here, all units share the same strategy object
                    _strategy.SetUp(this, _uniqueUnitID, _monsterStats, _mapGrid, _gameDamageManager);

                    _currentStrategy = _strategy.Think(_currentTile, _currentStrategy);

                    _strategy.Execute(_currentTile, _currentStrategy);
                }
            }

            _currentTile = standTile;
        }

        private void AgentMove() {
            moveDelta.Set((_currentTile.GetFlowFieldPath(VariableFlag.Strategy.CastleFirst).x),
                            _currentTile.GetFlowFieldPath(VariableFlag.Strategy.CastleFirst).y, 0);

            moveDelta *= Time.deltaTime * _monsterStats.moveSpeed * 0.3f;
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

                _animator.runtimeAnimatorController = targetAnimator;

            }
        }

        private TileNode FindValidStandPosition(Vector3 unitPosition) {
            TileNode standTile = _mapGrid.GetTileNodeByWorldPos(unitPosition);

            if (standTile.TilemapMember != null && !standTile.IsWalkable)
            {
                Vector3[] searchDirection = new Vector3[] { Vector3.up, Vector3.down, Vector3.right, Vector3.left };
                int searchLength = searchDirection.Length;

                for (int i = 0; i < searchLength; i++)
                {
                    TileNode searchTile = _mapGrid.GetTileNodeByWorldPos(unitPosition + searchDirection[i]);
                    if (searchTile.IsWalkable)
                    {
                        return searchTile;
                    }
                }
            }
            return standTile;
        }

        public void OnAttack(GameDamageManager.DMGRegistry dmgCard)
        {
            try
            {
                _hp -= dmgCard.unitStats.atk;

                if (_hp <= 0)
                {

                    if (dmgCard.fromUnit != null)
                    {

                        TowerUnit towerUnit = (TowerUnit)dmgCard.fromUnit;
                        if (towerUnit != null && towerUnit.buildPlayer != null) {
                            towerUnit.buildPlayer.EarnPrize(_monsterStats.prize);
                        }
                    }

                    Destroy();
                }
            }
            catch {
                Debug.Log("OnAttack error occur");
            }
        }

        public void Destroy()
        {
            if (this.OnDestroyCallback != null)
                OnDestroyCallback(this);

            if (_mapGrid != null)
            {
                _mapGrid.EditUnitState(_currentTile.GridIndex.x, _currentTile.GridIndex.y, this, false);
            }

            this.OnDestroyCallback = null;

            this._currentTile = default(TileNode);
        }

    }
}
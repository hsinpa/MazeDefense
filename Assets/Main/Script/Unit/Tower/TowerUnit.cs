using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using System.Linq;
using TD.Map;

namespace TD.Unit {
    public class TowerUnit : MonoBehaviour, UnitInterface
    {
        STPTower _stpTower;
        MapGrid _mapGrid;
        System.Action<UnitInterface> OnDestroyCallback;

        public bool isActive { get { return OnDestroyCallback != null; } }


        [SerializeField]
        private Transform stationObject;

        [SerializeField]
        private Transform gunBodyObject;

        [SerializeField]
        private SpriteRenderer RangeIndicator;

        public GameObject unitObject { get => this.gameObject; }

        TileNode currentTileNode;
        MonsterUnit currentTarget;

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

                this.currentTarget = UpdateTargetState();
                Attack(this.currentTarget);
            }
        }

        private void Attack(MonsterUnit target) {
            if (target != null) {
                Debug.Log("Attack this one " + target.name +", " + target.transform.position);
            }
        }

        private MonsterUnit UpdateTargetState() {
            //If target is already set, and is within range
            if (currentTarget != null && currentTarget.isActive &&
                Vector2.Distance(currentTarget.transform.position, transform.position) <= _stpTower.range)
            {
                return this.currentTarget;
            }
            else {

                var monsters = _mapGrid.FindUnitsFromRange<MonsterUnit>(currentTileNode, _stpTower.range);

                if (monsters.Count <= 0)
                    return null;

                return FindTheClosestUnit(monsters);
            }
        }

        private MonsterUnit FindTheClosestUnit(List<MonsterUnit> monsterUnits) {
            Vector2 selfPos = transform.position;

            return monsterUnits.Aggregate((i1, i2) => Vector2.Distance(i1.transform.position, selfPos) <= Vector2.Distance(i2.transform.position, selfPos) ? i1 : i2);
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

            currentTarget = null;
            currentTileNode = default(TileNode);
        }


    }
}

﻿using System.Collections;
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

        System.Action<UnitInterface, GameDamageManager.DMGRegistry> OnFireProjectile;

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
        const float minRotationDiff = 35;
        bool fireReady = false;
        float recordFrequency;

        public void SetUp(STPTower stpTower, MapGrid mapGrid, System.Action<UnitInterface, GameDamageManager.DMGRegistry> OnFireProjectile) {
            _stpTower = stpTower;
            _mapGrid = mapGrid;
            this.OnFireProjectile = OnFireProjectile;
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
            float angle = 0;

            if (target != null && gunBodyObject != null) {

                Vector3 distance = (target.transform.position - transform.position);
                Vector3 direction = distance.normalized;

                angle = Utility.MathUtility.NormalizeAngle(Utility.MathUtility.VectorToAngle(direction) - 90);

                if (fireReady)
                {
                    gunBodyObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                    Fire(target);
                }
                else {
                    float currentAngle = gunBodyObject.transform.eulerAngles.z;
                    if (currentAngle > 270 && angle >= 0 && angle < 180) {
                        angle += 360;
                    }

                    float lerpAngle = Utility.MathUtility.NormalizeAngle( Mathf.Lerp(currentAngle, angle, 0.1f) );
                    gunBodyObject.transform.rotation = Quaternion.Euler(0, 0, lerpAngle);

                    float rotationDiff = Mathf.Abs(angle - currentAngle);
                    if (rotationDiff < minRotationDiff)
                        fireReady = true;
                }

            }
        }

        private void Fire(MonsterUnit target) {
            float time = Time.time;

            if (recordFrequency < time) {
                GameObject bulletObj = Pooling.PoolManager.instance.ReuseObject(_stpTower.stpBullet._id);

                if (bulletObj != null && OnFireProjectile != null) {
                    BulletUnit bulletUnit = bulletObj.GetComponent<BulletUnit>();
                    bulletUnit.transform.position = transform.position;
                    bulletUnit.SetUp(_stpTower.stpBullet, target);

                    float dist = Vector3.Distance(target.transform.position, transform.position);
                    float reachTime = dist / (_stpTower.stpBullet.moveSpeed * Time.deltaTime * 60);

                    OnFireProjectile(bulletUnit, GetDMGRegisterCard(target, time, reachTime));
                    recordFrequency = time + _stpTower.frequency;
                }
            }
        }

        private GameDamageManager.DMGRegistry GetDMGRegisterCard(MonsterUnit target, float fireTime, float timeToDst) {
            GameDamageManager.DMGRegistry damageRequest = new GameDamageManager.DMGRegistry();
            damageRequest.fireTime = fireTime;
            damageRequest.timeToDest = timeToDst;
            damageRequest.towerInfo = _stpTower;

            damageRequest.targetNum = 1;
            damageRequest.targets = new MonsterUnit[] { target };

            return damageRequest;
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

                fireReady = false;

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

            this.OnFireProjectile = null;
            this.OnDestroyCallback = null;
            this._stpTower = null;

            this.recordFrequency = 0;

            currentTarget = null;
            currentTileNode = default(TileNode);
        }


    }
}

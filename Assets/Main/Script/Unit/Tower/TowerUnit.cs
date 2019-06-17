using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using System.Linq;
using TD.Map;
using TD.Database;

namespace TD.Unit {
    public class TowerUnit : MonoBehaviour, UnitInterface
    {
        STPTower _stpTower;
        TowerStats _towerStats;
        
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

        public TileNode currentTile { get { return _currentTile; } }
        private TileNode _currentTile;

        MonsterUnit currentTarget;
        const float minRotationDiff = 35;
        bool fireReady = false;
        float recordFrequency;

        public void SetUp(TowerStats towerStats, STPTower stpTower, Sprite towerSprite, MapGrid mapGrid, 
            System.Action<UnitInterface, GameDamageManager.DMGRegistry> OnFireProjectile) {
            _towerStats = towerStats;
            _stpTower = stpTower;
            _mapGrid = mapGrid;

            SpriteRenderer renderer = gunBodyObject.GetComponent<SpriteRenderer>();
            renderer.sprite = towerSprite;

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
            if (this.OnDestroyCallback != null && _mapGrid != null && _currentTile.TilemapMember != null && _stpTower != null) {

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
            damageRequest.towerStats = _towerStats;

            damageRequest.target = target;

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

                var monsters = _mapGrid.FindUnitsFromRange<MonsterUnit>(_currentTile, _stpTower.range);

                fireReady = false;

                if (monsters.Count <= 0)
                    return null;

                return FindTheClosestUnit(monsters);
            }
        }

        public void OnAttack(float dmg)
        {
            throw new System.NotImplementedException();
        }

        private MonsterUnit FindTheClosestUnit(List<MonsterUnit> monsterUnits) {
            Vector2 selfPos = transform.position;

            return monsterUnits.Aggregate((i1, i2) => Vector2.Distance(i1.transform.position, selfPos) <= Vector2.Distance(i2.transform.position, selfPos) ? i1 : i2);
        }

        private void UpdateUnitState()
        {

            TileNode standTile = _mapGrid.GetTileNodeByWorldPos(unitObject.transform.position);

            if (standTile.TilemapMember != null && standTile.GridIndex != _currentTile.GridIndex)
            {
                if (_currentTile.TilemapMember != null)
                    _mapGrid.EditUnitState(_currentTile.GridIndex, this, false);

                _currentTile = standTile;

                _mapGrid.EditUnitState(standTile.GridIndex, this, true);
                _mapGrid.RefreshMonsterFlowFieldMap();
            }
        }

        public void Destroy()
        {
            if (this.OnDestroyCallback != null)
                OnDestroyCallback(this);

            if (_mapGrid != null) {
                _mapGrid.EditUnitState(_currentTile.GridIndex, this, false);
                _mapGrid.OnMapReform -= UpdateUnitState;
                _mapGrid.RefreshMonsterFlowFieldMap();
            }

            this.OnFireProjectile = null;
            this.OnDestroyCallback = null;
            this._stpTower = null;

            this.recordFrequency = 0;

            currentTarget = null;
            _currentTile = default(TileNode);
        }

    }
}

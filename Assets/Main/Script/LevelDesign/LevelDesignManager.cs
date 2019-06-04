using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Unit;
using Pooling;

namespace TD.AI {
    public class LevelDesignManager : MonoBehaviour
    {
        private GameUnitManager _gameUnitManager;
        private MapHolder _mapHolder;
        private MapGrid _mapGrid;

        private MapComponent _entranceComponent;
        private List<STPMonster> _monsterUnits;
        private int monsterLength;

        [SerializeField]
        float spawnSpeedAtWaitingStage = 1;

        [SerializeField]
        float spawnStageLastSecond = 20;

        [SerializeField]
        float spawnStageFrequncy = 0.3f;

        private float recordTime;

        public void SetUp(GameUnitManager gameUnitManager, MapHolder mapHolder, MapGrid mapGrid, List<STPMonster> monsterUnits)
        {
            _gameUnitManager = gameUnitManager;
            _mapHolder = mapHolder;
            _mapGrid = mapGrid;
            _monsterUnits = monsterUnits;
            monsterLength = _monsterUnits.Count;
        }

        public void CallEveryoneReady() {
            if (_mapHolder == null) return;

            var tComponent = FindEntryComponent(_mapHolder);
            this.recordTime = 0;

            if (tComponent != null) {
                _entranceComponent = tComponent;

            }
        }

        private MapComponent FindEntryComponent(MapHolder mapHolder) {
            return mapHolder.mapComponents.Find(x => x.map_type == MapComponent.Type.Entrance);
        }

        private void Update() {

            if (recordTime > spawnStageLastSecond || _entranceComponent == null)
                return;

            if (recordTime < Time.time) {
                Spawn();
                recordTime = Time.time + spawnStageFrequncy;
            }
        }

        private void Spawn() {
            //Pick monster type
            STPMonster randomMonster = _monsterUnits[Random.Range(0, monsterLength)];

            //Pick start position
            int randomX = Random.Range(0, _entranceComponent.fullSize.x);
            TileNode randomTileNode = _entranceComponent.tilemapReader.nodes[randomX, _entranceComponent.fullSize.y-1];

            GameObject monsterObject = PoolManager.instance.ReuseObject(randomMonster._id);
            if (monsterObject != null) {
                MonsterUnit unit = monsterObject.GetComponent<MonsterUnit>();
                unit.transform.position = randomTileNode.WorldSpace;
                unit.SetUp(_mapGrid);
                _gameUnitManager.AddUnit(unit);
            }
        }

    }
}

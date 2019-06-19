using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Unit;
using Pooling;
using TD.Database;

namespace TD.AI {
    public class LevelDesignManager : MonoBehaviour
    {
        private GameUnitManager _gameUnitManager;
        private MapBlockManager _mapHolder;
        private MapGrid _mapGrid;
        private GameStrategyMapper _strategyMapper;

        private BlockComponent _entranceComponent;
        private List<MonsterStats> _monsterUnits;
        private int monsterLength;

        public static float Time, DeltaTime;

        [SerializeField]
        float spawnSpeedAtWaitingStage = 1;

        [SerializeField]
        float spawnStageLastSecond = 20;

        [SerializeField]
        float spawnStageFrequncy = 0.3f;

        private float recordTime;

        public void SetUp(GameUnitManager gameUnitManager, MapBlockManager mapHolder, MapGrid mapGrid, List<MonsterStats> monsterUnits)
        {
            _gameUnitManager = gameUnitManager;
            _mapHolder = mapHolder;
            _mapGrid = mapGrid;
            _monsterUnits = monsterUnits;
            _strategyMapper = new GameStrategyMapper();
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

        private BlockComponent FindEntryComponent(MapBlockManager mapHolder) {
            return mapHolder.mapComponents.Find(x => x.map_type == BlockComponent.Type.Entrance);
        }

        private void Update() {

            Time = UnityEngine.Time.time;
            DeltaTime = UnityEngine.Time.deltaTime;

            if (recordTime > spawnStageLastSecond || _entranceComponent == null)
                return;

            if (recordTime < Time) {
                Spawn();
                recordTime = Time + spawnStageFrequncy;
            }
        }

        private void Spawn() {
            int spawnNumPerTime = Random.Range(1, 4);

            for (int s = 0; s < spawnNumPerTime; s++) {
                //Pick monster type
                MonsterStats randomMonster = _monsterUnits[Random.Range(0, monsterLength)];

                //Pick start position
                int randomX = Random.Range(0, _entranceComponent.fullSize.x);
                TileNode randomTileNode = _entranceComponent.tilemapReader.nodes[randomX, _entranceComponent.fullSize.y - 1];

                GameObject monsterObject = PoolManager.instance.ReuseObject(VariableFlag.Pooling.MonsterID);
                if (monsterObject != null)
                {
                    BaseStrategy strategy = _strategyMapper.GetStrategy(randomMonster.strategy);

                    MonsterUnit unit = monsterObject.GetComponent<MonsterUnit>();
                    unit.transform.position = randomTileNode.WorldSpace;
                    unit.SetUp(randomMonster, strategy, _mapGrid, _mapHolder);
                    _gameUnitManager.AddUnit(unit);
                }
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Unit;
using Pooling;
using TD.Database;
using System.Threading;
using Hsinpa.Utility;

namespace TD.AI {
    public class LevelDesignManager : MonoBehaviour
    {
        private LevelDirector _levelDirector;

        private GameUnitManager _gameUnitManager;
        private MapBlockManager _mapHolder;
        private MapGrid _mapGrid;
        private GameStrategyMapper _strategyMapper;

        private BlockComponent _entranceComponent;
        private List<MonsterStats> _monsterUnits;
        private List<PlayerModel> _players;

        public PlayerModel selfPlayer {
            get { return _player; }
        }
        private PlayerModel _player;

        private int monsterLength;

        private Queue<MonsterStats> _spawnQueue;

        public static float Time, DeltaTime;

        [SerializeField]
        float spawnSpeedAtWaitingStage = 1;

        [SerializeField]
        float spawnStageLastSecond = 20;

        [SerializeField]
        float spawnStageFrequncy = 0.3f;

        private float recordTime;

        public void Init(GameUnitManager gameUnitManager, MapBlockManager mapHolder, MapGrid mapGrid, List<MonsterStats> allMonsterUnits)
        {
            _levelDirector = new LevelDirector(gameUnitManager, allMonsterUnits, PoolManager.instance.GetObjectLength(VariableFlag.Pooling.MonsterID) );
            _spawnQueue = new Queue<MonsterStats>();
            _gameUnitManager = gameUnitManager;
            _mapHolder = mapHolder;
            _mapGrid = mapGrid;
            _monsterUnits = allMonsterUnits;

            _strategyMapper = new GameStrategyMapper();
            monsterLength = _monsterUnits.Count;

            _levelDirector.OnCallReinforcement += CallReinforcement;
        }

        public void SetLevel(List<PlayerModel> players, PlayerModel selfPlayer) {
            this._players = players;
            this._player = selfPlayer;

            CallEveryoneReady();
        }

        private void CallEveryoneReady() {
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

            Time = TimeSystem.time;
            DeltaTime = UnityEngine.Time.deltaTime;

            if (_entranceComponent != null)
                _levelDirector.OnUpdate();

            //if (recordTime > spawnStageLastSecond || _entranceComponent == null)
            //    return;

            if (recordTime < Time)
            {
                SpawnQueueMonster();
                recordTime = Time + spawnStageFrequncy;
            }
        }

        private void CallReinforcement(LevelDirector.UnitStructure unitStructList) {

            Thread t = new Thread(new ThreadStart(() => {

                lock (_spawnQueue) {
                    int arrayLength = unitStructList.unitArray.Length;
                    for (int i = 0; i < arrayLength; i++) {
                        for (int k = 0; k < unitStructList.unitArray[i].spawnNum; k++)
                        {
                            _spawnQueue.Enqueue(unitStructList.unitArray[i].monsterStats);
                        }
                    }
                }
            }));

            t.Start();
        }

        private void SpawnQueueMonster()
        {
            int queueLength = _spawnQueue.Count;
            int spawnNumPerTime = Random.Range(1, 4);



            for (int s = 0; s < spawnNumPerTime; s++)
            {
                if (s < queueLength) {
                    Spawn(_spawnQueue.Dequeue());
                }
            }
        }

        private void Spawn(MonsterStats monster) {
            //int spawnNumPerTime = Random.Range(1, 4);

            //for (int s = 0; s < spawnNumPerTime; s++) {
            //    //Pick monster type
            //    MonsterStats randomMonster = _monsterUnits[Random.Range(0, monsterLength)];

                //Pick start position
                int randomX = Random.Range(0, _entranceComponent.fullSize.x);
                TileNode randomTileNode = _entranceComponent.tilemapReader.nodes[randomX, _entranceComponent.fullSize.y - 1];

                GameObject monsterObject = PoolManager.instance.ReuseObject(VariableFlag.Pooling.MonsterID);
                if (monsterObject != null)
                {
                    BaseStrategy strategy = _strategyMapper.GetStrategy(monster.strategy);

                    MonsterUnit unit = monsterObject.GetComponent<MonsterUnit>();
                    unit.transform.position = randomTileNode.WorldSpace;
                    unit.SetUp(monster, strategy, _mapGrid, _mapHolder, _gameUnitManager.gameDamageManager);
                    _gameUnitManager.AddUnit(unit);
                }
            //}
        }

        public void Reset()
        {
            _spawnQueue.Clear();
            if (_levelDirector != null)
                _levelDirector.OnCallReinforcement -= CallReinforcement;

        }

    }
}

using Hsinpa.Utility;
using System.Collections;
using System.Collections.Generic;
using TD.AI;
using TD.Database;
using TD.Map;
using TD.Unit;
using TD.Static;
using UnityEngine;
using Pooling;

namespace TD.Storyboard {
    public class StoryBoardManager : MonoBehaviour
    {
        private GameUnitManager _gameUnitManager;
        private MapBlockManager _mapHolder;
        private MapGrid _mapGrid;
        private GameStrategyMapper _strategyMapper;

        private BlockComponent _entranceComponent;
        private List<MonsterStats> _monsterUnits;
        private List<PlayerModel> _players;

        private StoryBoardFlow _storyBoardFlow;
        private Queue<MonsterStats> _spawnQueue;

        private Dictionary<string, System.Action<GeneralObjectStruct.RawStoryBoardStruct>> storyActionTable;

        [SerializeField]
        float spawnStageFrequncy = 0.3f;

        //Wave related
        private float waveStepTime;
        private int waveStartTime;
        private float[] waveUnitPTable;
        private GeneralObjectStruct.WaveStruct _currentWaveStruct;

        public void Init(GameUnitManager gameUnitManager, MapBlockManager mapHolder, MapGrid mapGrid, List<MonsterStats> allMonsterUnits)
        {
            _spawnQueue = new Queue<MonsterStats>();
            _gameUnitManager = gameUnitManager;
            _mapHolder = mapHolder;
            _mapGrid = mapGrid;
            _monsterUnits = allMonsterUnits;

            _strategyMapper = new GameStrategyMapper();
            _storyBoardFlow = new StoryBoardFlow(allMonsterUnits);

            _storyBoardFlow.ParseStoryboardData();

            this.storyActionTable = new Dictionary<string, System.Action<GeneralObjectStruct.RawStoryBoardStruct>>() {
                {VariableFlag.StoryBoard.StartType, RunStartStruct },
                {VariableFlag.StoryBoard.WaveType, RunWaveStruct },
                {VariableFlag.StoryBoard.CinemaType, RunCinemaStruct },
                {VariableFlag.StoryBoard.EndType, RunEndStruct },
            };
        }

        #region Storyboard Logic Flow
        public async void StartStoryBoard(string level_id) {
            await Utility.UtilityMethod.WaitUntil(() => _storyBoardFlow.IsCompleteLoad, 100);

            var storyStruct =_storyBoardFlow.StartStoryBoard(level_id);
            RunStoryBoardStruct(storyStruct);
        }

        private void RunStoryBoardStruct(GeneralObjectStruct.RawStoryBoardStruct storyBoardStruct) { 
            if (this.storyActionTable.TryGetValue(storyBoardStruct.Type, out var action)) {
                action(storyBoardStruct);
            }
        }

        private void RunStartStruct(GeneralObjectStruct.RawStoryBoardStruct storyBoardStruct) {             
            var next = _storyBoardFlow.Process();

            RunStoryBoardStruct(next);
        }

        private void RunWaveStruct(GeneralObjectStruct.RawStoryBoardStruct storyBoardStruct)
        {
            this._currentWaveStruct = _storyBoardFlow.ParseWave(storyBoardStruct);
            this.waveUnitPTable = new float[this._currentWaveStruct.length];
            this.waveStartTime = (int)TimeSystem.time;
        }

        private void RunCinemaStruct(GeneralObjectStruct.RawStoryBoardStruct storyBoardStruct)
        {

        }

        private void RunEndStruct(GeneralObjectStruct.RawStoryBoardStruct storyBoardStruct)
        {
            Debug.Log("End Level " + storyBoardStruct.ID);
        }
        #endregion

        #region Monster Unit Spawn

        private int SelectMonsterIndex(GeneralObjectStruct.WaveStruct wave, ref float[] non_alloc_table) {
            int w_lens = wave.length;
            float totalPercentage = 0;

            for (int i = 0; i < w_lens; i++) {
                float spawnPercentage = (wave.spawn_count[i] - wave.spawn_record[i]) / (float)wave.spawn_count[i];
                non_alloc_table[i] = spawnPercentage;
                totalPercentage += spawnPercentage;
            }

            if (totalPercentage == 0) return -1;

            //Normalize
            for (int i = 0; i < w_lens; i++)
            {
                non_alloc_table[i] /= totalPercentage;
            }

            return Utility.UtilityMethod.PercentageTurntable(non_alloc_table);
        }

        private void SpawnQueueMonster(GeneralObjectStruct.WaveStruct wave, int selectIndex)
        {
            if (selectIndex < 0) return;

            int maxSpawn = 4;
            int remainingSpawn = (wave.spawn_count[selectIndex] - wave.spawn_record[selectIndex]);
            if (remainingSpawn < maxSpawn)
                maxSpawn = remainingSpawn;

            int spawnNumPerTime = Random.Range(1, maxSpawn);

            wave.spawn_record[selectIndex] += spawnNumPerTime;

            //for (int s = 0; s < spawnNumPerTime; s++)
            //{
            //    Spawn(wave.monsters[selectIndex]);
            //}
        }

        private void Spawn(MonsterStats monster)
        {
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
        }

        #endregion

        private void Update()
        {
            if (waveStepTime < TimeSystem.time && _storyBoardFlow.CurrentStroyStruct.Type == VariableFlag.StoryBoard.WaveType)
            {
                int selectIndex = SelectMonsterIndex(this._currentWaveStruct, ref waveUnitPTable);
                SpawnQueueMonster(this._currentWaveStruct, selectIndex);
                Debug.Log("Spawn Update, Index " + selectIndex);

                waveStepTime = TimeSystem.time + spawnStageFrequncy;
            }
        }
    }
}

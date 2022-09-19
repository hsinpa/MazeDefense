using Hsinpa.Utility;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TD.AI;
using TD.Database;
using TD.Map;
using TD.Unit;
using UnityEngine;


namespace TD.Storyboard {
    public class StoryBoardManager : MonoBehaviour
    {
        private LevelDirector _levelDirector;

        private GameUnitManager _gameUnitManager;
        private MapBlockManager _mapHolder;
        private MapGrid _mapGrid;
        private GameStrategyMapper _strategyMapper;

        private BlockComponent _entranceComponent;
        private List<MonsterStats> _monsterUnits;
        private List<PlayerModel> _players;

        private Queue<MonsterStats> _spawnQueue;

        public void Init(GameUnitManager gameUnitManager, MapBlockManager mapHolder, MapGrid mapGrid, List<MonsterStats> allMonsterUnits)
        {
            _spawnQueue = new Queue<MonsterStats>();
            _gameUnitManager = gameUnitManager;
            _mapHolder = mapHolder;
            _mapGrid = mapGrid;
            _monsterUnits = allMonsterUnits;

            _strategyMapper = new GameStrategyMapper();

            ConvertJSON();
        }

        public void ConvertJSON() {
            string json_raw = IOUtility.ConvertCsvFileToJsonObject(Application.streamingAssetsPath + "/CSV/database - skill.csv");
            Debug.Log(json_raw);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Unit;
using TD.Database;
using TD.AI;

public class DummyMonsterGeneator : MonoBehaviour
{
    [SerializeField, Range(0, 1)]
    float spawnSpeed = 0.2f;

    [SerializeField]
    MapGrid _mapGrid;

    [SerializeField]
    MapBlockManager _mapHolder;

    [SerializeField]
    GameUnitManager _gameUnitManager;

    [SerializeField]
    LevelDesignManager _levelDesignManager;

    [SerializeField]
    StatsHolder _statsHolder;

    List<MonsterStats> _monsterStats;

    Camera _camera;

    GameStrategyMapper _strategyMapper;

    private float recordSpawnTime;

    private void Start()
    {
        _camera = Camera.main;
        
        _strategyMapper = new GameStrategyMapper();

        if (_statsHolder != null)
            _monsterStats = _statsHolder.FindObjectByType<MonsterStats>();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetMouseButton(1) && LevelDesignManager.Time > recordSpawnTime) {
            recordSpawnTime = LevelDesignManager.Time + spawnSpeed;

            var worldPos = (_camera.ScreenToWorldPoint(Input.mousePosition));
            worldPos.Set(worldPos.x, worldPos.y, 0);

            var tile = _mapGrid.GetTileNodeByWorldPos(worldPos);

            if (tile.TileBase != null && (tile.towerUnit == null || !tile.towerUnit.isActive) && tile.IsWalkable)
            {
                GenerateMonster(tile);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && _levelDesignManager != null) {
            _levelDesignManager.CallEveryoneReady();
        }
    }
#endif

    private void GenerateMonster(TileNode tileNode) {

        if (_monsterStats == null || _monsterStats.Count <= 0)
            return;

        MonsterStats monsterStats = _monsterStats[0];

        GameObject monsterObject = Pooling.PoolManager.instance.ReuseObject(VariableFlag.Pooling.MonsterID);
        if (monsterObject != null) {
            monsterObject.transform.position = tileNode.WorldSpace;
            BaseStrategy strategy = _strategyMapper.GetStrategy(monsterStats.strategy);

            MonsterUnit dummyUnit = monsterObject.GetComponent<MonsterUnit>();
            dummyUnit.SetUp(monsterStats, strategy, _mapGrid, _mapHolder, _gameUnitManager.gameDamageManager);

            _gameUnitManager.AddUnit(dummyUnit);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Unit;
using TD.Database;

public class DummyMonsterGeneator : MonoBehaviour
{
    [SerializeField]
    MapGrid _mapGrid;

    [SerializeField]
    MapBlockManager _mapHolder;

    [SerializeField]
    GameUnitManager _gameUnitManager;

    [SerializeField]
    MonsterStats[] _spawnMonster;

    Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) {

            var worldPos = (_camera.ScreenToWorldPoint(Input.mousePosition));
            worldPos.Set(worldPos.x, worldPos.y, 0);

            var tile = _mapGrid.GetTileNodeByWorldPos(worldPos);

            if (tile.TileBase != null)
            {
                GenerateMonster(tile);
            }
        }
    }
#endif

    private void GenerateMonster(TileNode tileNode) {

        if (_spawnMonster == null || _spawnMonster.Length <= 0)
            return;

        MonsterStats monsterStats = _spawnMonster[Random.Range(0, _spawnMonster.Length)];

        GameObject monsterObject = Pooling.PoolManager.instance.ReuseObject(VariableFlag.Pooling.MonsterID);
        if (monsterObject != null) {
            monsterObject.transform.position = tileNode.WorldSpace;

            MonsterUnit dummyUnit = monsterObject.GetComponent<MonsterUnit>();
            dummyUnit.SetUp(monsterStats, _mapGrid, _mapHolder);

            _gameUnitManager.AddUnit(dummyUnit);

        }
    }

}

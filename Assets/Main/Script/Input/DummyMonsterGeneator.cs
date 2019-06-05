using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Unit;

public class DummyMonsterGeneator : MonoBehaviour
{
    [SerializeField]
    MapGrid _mapGrid;

    [SerializeField]
    MapBlockManager _mapHolder;

    [SerializeField]
    GameUnitManager _gameUnitManager;


    [SerializeField]
    string _monsterKey;

    Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

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

    private void GenerateMonster(TileNode tileNode) {
        GameObject monsterObject = Pooling.PoolManager.instance.ReuseObject(_monsterKey);
        if (monsterObject != null) {
            monsterObject.transform.position = tileNode.WorldSpace;

            MonsterUnit dummyUnit = monsterObject.GetComponent<MonsterUnit>();
            dummyUnit.SetUp(_mapGrid);

            _gameUnitManager.AddUnit(dummyUnit);

        }
    }

}

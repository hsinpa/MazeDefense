using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;

public class DummyMonsterGeneator : MonoBehaviour
{
    [SerializeField]
    MapGrid _mapGrid;

    [SerializeField]
    MapHolder _mapHolder;

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
                Debug.Log("Right click " + tile.GridIndex);

                GenerateMonster(tile);
            }
        }
    }

    private void GenerateMonster(TileNode tileNode) {
        GameObject monsterObject = Pooling.PoolManager.instance.ReuseObject(_monsterKey);
        if (monsterObject != null) {
            monsterObject.transform.position = tileNode.WorldSpace;

            DummyUnit dummyUnit = monsterObject.GetComponent<DummyUnit>();
            dummyUnit.SetUp(_mapGrid);
        }
    }

}

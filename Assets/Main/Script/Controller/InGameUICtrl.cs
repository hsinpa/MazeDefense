using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using Utility;
using Pooling;
using TD.Unit;

public class InGameUICtrl : MonoBehaviour
{
    GameInputManager _gameInputManager;
    GameUnitManager _gameUnitManager;
    MapGrid _mapGrid;

    TileNode currentSelectedNode;


    [SerializeField]
    ConstructionView ConstructionUI;

    [SerializeField, Range(0, 4)]
    float IgnoreInputRange = 2;

    public void SetUp(GameInputManager gameInputManager, GameUnitManager gameUnitManager, MapGrid mapGrid) {
        _gameInputManager = gameInputManager;
        _gameInputManager.OnSelectTileNode += SelectTileListener;

        _gameUnitManager = gameUnitManager;
        _mapGrid = mapGrid;

        if (ConstructionUI != null)
        {
            ConstructionUI.TowerClickEvent += SelectTowerToBuild;
        }
    }

    private void SelectTowerToBuild(string tower_id) {

        if (currentSelectedNode.TileMapPlace != null) {
            var tower = PoolManager.instance.ReuseObject(tower_id);

            if (tower != null) {
                tower.transform.position = currentSelectedNode.WorldSpace;

                _gameUnitManager.AddUnit(tower.GetComponent<BaseUnit>());
            }
        }

        StartCoroutine(GeneralUtility.DoDelayWork(0.1f, () =>
        {
            Reset();
        }));

    }

    private void SelectTileListener(TileNode tileNode) {
        if (!tileNode.IsWalkable)
            return;

        if (currentSelectedNode.TilemapMember != null) {
            float dist = Vector3.Distance(currentSelectedNode.WorldSpace, tileNode.WorldSpace);
            if (dist > IgnoreInputRange)
            {
                Reset();
                return;
            }
            else {
                return;
            }
        }

        currentSelectedNode = tileNode;

        ConstructionUI.transform.position = currentSelectedNode.WorldSpace;
        ConstructionUI.Show(true);
    }

    private void Reset()
    {
        currentSelectedNode.TilemapMember = null;
        ConstructionUI.Show(false);
    }

    private void OnDestroy()
    {
        if (_gameInputManager != null)
            _gameInputManager.OnSelectTileNode -= SelectTileListener;
    }
}

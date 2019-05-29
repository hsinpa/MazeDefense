using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using Utility;

public class InGameUICtrl : MonoBehaviour
{
    GameInputManager _gameInputManager;
    TileNode currentSelectedNode;

    [SerializeField]
    GameObject tempTowerPrefab;

    [SerializeField]
    ConstructionView ConstructionUI;

    [SerializeField, Range(0, 4)]
    float IgnoreInputRange = 2;

    public void SetUp(GameInputManager gameInputManager) {
        _gameInputManager = gameInputManager;
        _gameInputManager.OnSelectTileNode += SelectTileListener;

        if (ConstructionUI != null)
        {
            ConstructionUI.TowerClickEvent += SelectTowerToBuild;
        }
    }

    private void SelectTowerToBuild(int tower_id) {

        if (tempTowerPrefab != null && currentSelectedNode.TileMapPlace != null) {
            var tower = Instantiate(tempTowerPrefab);
            tower.transform.position = currentSelectedNode.WorldSpace;
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

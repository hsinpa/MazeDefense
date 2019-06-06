using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using Utility;
using Pooling;
using TD.Unit;
using System.Threading.Tasks;

public class InGameUICtrl : MonoBehaviour
{
    GameInputManager _gameInputManager;
    GameUnitManager _gameUnitManager;
    MapGrid _mapGrid;
    MapBlockManager _mapBlockManager;

    TileNode currentSelectedNode;
    STPTheme _stpTheme;

    [SerializeField]
    ConstructionView ConstructionUI;

    [SerializeField, Range(0, 4)]
    float IgnoreInputRange = 2;

    public void SetUp(GameInputManager gameInputManager, GameUnitManager gameUnitManager, MapGrid mapGrid, MapBlockManager mapBlockManager, STPTheme stpTheme) {
        _gameInputManager = gameInputManager;
        _gameInputManager.OnSelectTileNode += SelectTileListener;

        _gameUnitManager = gameUnitManager;
        _mapGrid = mapGrid;
        _mapBlockManager = mapBlockManager;
        _stpTheme = stpTheme;

        if (ConstructionUI != null)
        {
            ConstructionUI.TowerClickEvent += SelectTowerToBuild;
        }
    }

    private async void SelectTowerToBuild(string tower_id) {

        if (currentSelectedNode.TileMapPlace != null) {
            var tower = PoolManager.instance.ReuseObject(tower_id);

            if (tower != null) {
                tower.transform.position = currentSelectedNode.WorldSpace;

                MapComponent mapBlock = _mapBlockManager.GetMapComponentByPos(currentSelectedNode.WorldSpace);
                STPTower stpTower = _stpTheme.FindObject<STPTower>(tower_id);
                TowerUnit towerUnit = tower.GetComponent<TowerUnit>();

                if (stpTower != null && towerUnit != null && mapBlock != null) {
                    tower.transform.SetParent(mapBlock.unitHolder);
                    towerUnit.SetUp(stpTower, _mapGrid, (UnitInterface projectile, GameDamageManager.DMGRegistry dmgRistry) => {
                        _gameUnitManager.AddUnit(projectile);
                        _gameUnitManager.gameDamageManager.AddRequest(dmgRistry);
                    });

                    _gameUnitManager.AddUnit(towerUnit);
                }
            }
        }

        await (GeneralUtility.DoDelayWork(0.1f, () =>
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

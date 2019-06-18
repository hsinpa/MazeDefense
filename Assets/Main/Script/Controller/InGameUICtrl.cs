using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using Utility;
using Pooling;
using TD.Unit;
using TD.Database;
using System.Threading.Tasks;

public class InGameUICtrl : MonoBehaviour
{
    GameInputManager _gameInputManager;
    GameUnitManager _gameUnitManager;
    MapGrid _mapGrid;
    MapBlockManager _mapBlockManager;

    TileNode currentSelectedNode;
    STPTheme _stpTheme;
    StatsHolder _statHolder;

    [SerializeField]
    ConstructionView ConstructionUI;

    [SerializeField]
    Sprite tempSprite1;

    [SerializeField]
    Sprite tempSprite2;

    [SerializeField, Range(0, 4)]
    float IgnoreInputRange = 2;

    private List<TowerStats> FirstLevelTowers;

    public void SetUp(GameInputManager gameInputManager, GameUnitManager gameUnitManager, MapGrid mapGrid, MapBlockManager mapBlockManager, 
                    STPTheme stpTheme, StatsHolder statHolder) {
        _gameInputManager = gameInputManager;
        _gameInputManager.OnSelectTileNode += SelectTileListener;

        _gameUnitManager = gameUnitManager;
        _mapGrid = mapGrid;
        _mapBlockManager = mapBlockManager;
        _stpTheme = stpTheme;
        _statHolder = statHolder;

        if (ConstructionUI != null)
        {
            ConstructionUI.TowerClickEvent += SelectTowerToBuild;
        }
    }

    private async void SelectTowerToBuild(string tower_id) {

        if (currentSelectedNode.TileMapPlace != null) {
            var tower = PoolManager.instance.ReuseObject(VariableFlag.Pooling.TowerID);

            if (tower != null) {
                tower.transform.position = currentSelectedNode.WorldSpace;

                BlockComponent mapBlock = _mapBlockManager.GetMapComponentByPos(currentSelectedNode.WorldSpace);
                STPTower stpTower = _stpTheme.FindObject<STPTower>(VariableFlag.Pooling.TowerID);
                TowerUnit towerUnit = tower.GetComponent<TowerUnit>();
                TowerStats towerStats = _statHolder.FindObject<TowerStats>(tower_id);

                if (stpTower != null && towerUnit != null && mapBlock != null && towerStats != null) {
                    tower.transform.SetParent(mapBlock.unitHolder);
                    towerUnit.SetUp(towerStats, stpTower,
                        (tower_id == "tower_01") ? tempSprite1 : tempSprite2,
                        _mapGrid, (UnitInterface projectile, GameDamageManager.DMGRegistry dmgRistry) => {
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

        UnityEngine.UI.Button[] displayBTObjects = ConstructionUI.SetTowerToDisplay(GetInitialTowerPlacement());
        ConstructionUI.SetLayoutUI(displayBTObjects, currentSelectedNode.GridIndex, _mapBlockManager.blockSize);
        ConstructionUI.transform.position = currentSelectedNode.WorldSpace;
        ConstructionUI.Show(true);
    }

    private ConstructionView.DisplayUIComp[] GetInitialTowerPlacement() {
        List<TowerStats> firstLevelTowers = _statHolder.FindObjectByType<TowerStats>();
        firstLevelTowers = firstLevelTowers.FindAll(x => x.level == 1);

        int towerLength = firstLevelTowers.Count;

        ConstructionView.DisplayUIComp[] uiCompArray = new ConstructionView.DisplayUIComp[towerLength];

        for (int i = 0; i < towerLength; i++) {
            ConstructionView.DisplayUIComp uiComp = new ConstructionView.DisplayUIComp();
            uiComp.label = "$" + firstLevelTowers[i].cost;
            uiComp._id = firstLevelTowers[i].id;

            if (i == 0)
                uiComp.sprite = tempSprite2;

            else if (i == 1)
                uiComp.sprite = tempSprite1;
            else
                uiComp.sprite = tempSprite1;

            uiCompArray[i] = uiComp;
        }

        return uiCompArray;
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

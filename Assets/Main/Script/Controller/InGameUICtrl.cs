using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using Utility;
using Pooling;
using TD.Unit;
using TD.Database;
using TD.AI;
using System.Threading.Tasks;

namespace TD.UI
{
    public class InGameUICtrl : MonoBehaviour
    {
        GameInputManager _gameInputManager;
        GameUnitManager _gameUnitManager;
        MapGrid _mapGrid;
        MapBlockManager _mapBlockManager;
        LevelDesignManager _levelDesignManager;

        TileNode currentSelectedNode;
        STPTheme _stpTheme;
        StatsHolder _statHolder;

        [SerializeField]
        ConstructionView ConstructionUI;

        [SerializeField]
        Sprite[] spriteSheet;

        [SerializeField, Range(0, 4)]
        float IgnoreInputRange = 2;

        bool isTaskDone = false;

        private List<TowerStats> FirstLevelTowers;

        public void SetUp(GameInputManager gameInputManager, GameUnitManager gameUnitManager, LevelDesignManager levelDesign, MapGrid mapGrid, MapBlockManager mapBlockManager,
                        STPTheme stpTheme, StatsHolder statHolder)
        {
            _gameInputManager = gameInputManager;
            _gameInputManager.OnSelectTileNode += SelectTileListener;

            _gameUnitManager = gameUnitManager;
            _levelDesignManager = levelDesign;
            _mapGrid = mapGrid;
            _mapBlockManager = mapBlockManager;
            _stpTheme = stpTheme;
            _statHolder = statHolder;

            if (ConstructionUI != null)
            {
                ConstructionUI.TowerClickEvent += SelectTowerToBuild;
            }
        }

        private async void SelectTowerToBuild(string tower_id)
        {

            if (IsTileNodeValid(currentSelectedNode))
            {
                TowerStats towerStats = _statHolder.FindObject<TowerStats>(tower_id);

                //No money
                if (_levelDesignManager.selfPlayer.capital < towerStats.cost)
                    return;


                var tower = PoolManager.instance.ReuseObject(VariableFlag.Pooling.TowerID);

                if (tower != null)
                {
                    tower.transform.position = currentSelectedNode.WorldSpace;

                    BlockComponent mapBlock = _mapBlockManager.GetMapComponentByPos(currentSelectedNode.WorldSpace);
                    STPTower stpTower = _stpTheme.FindObject<STPTower>(VariableFlag.Pooling.TowerID);
                    TowerUnit towerUnit = tower.GetComponent<TowerUnit>();

                    if (stpTower != null && towerUnit != null && mapBlock != null && towerStats != null)
                    {
                        tower.transform.SetParent(mapBlock.unitHolder);
                        towerUnit.SetUp(towerStats, stpTower,
                            _mapGrid, _levelDesignManager.selfPlayer,
                       (UnitInterface projectile, GameDamageManager.DMGRegistry dmgRistry) =>
                       {
                           _gameUnitManager.AddUnit(projectile);
                           _gameUnitManager.gameDamageManager.AddRequest(dmgRistry);
                       });

                        _gameUnitManager.AddUnit(towerUnit);
                        isTaskDone = true;
                    }
                }
            }
        }

        private void SelectTowerToUpgrade(TowerUnit towerUnit, TowerStats upgradeStats)
        {
            towerUnit.SetTowerStats(upgradeStats);
        }

        private void SelectTowerToInfo(TileNode tileNode, TowerStats upgradeStats)
        {
            Debug.Log("Mouse click SelectTowerToInfo");
        }

        private void SelectTowerToSale(TileNode tileNode, TowerStats upgradeStats)
        {
            Debug.Log("Mouse click SelectTowerToSale");
        }

        private void SelectTileListener(TileNode tileNode)
        {
            if (!IsTileNodeValid(tileNode)) {
                Reset();
                return;
            }

            UnityEngine.UI.Button[] displayBTObjects = null;

            if (currentSelectedNode.TilemapMember != null)
            {
                float dist = Vector3.Distance(currentSelectedNode.WorldSpace, tileNode.WorldSpace);
                if (dist > IgnoreInputRange)
                {
                    Reset();
                }

                if (isTaskDone)
                    Reset();

                return;
            }

            if (tileNode.towerUnit != null)
                displayBTObjects = ConstructionUI.SetTowerToDisplay(GetTowerUpgradePath(tileNode.towerUnit));

            currentSelectedNode = tileNode;

            if (displayBTObjects == null)
                displayBTObjects = ConstructionUI.SetTowerToDisplay(GetInitialTowerPlacement());

            ConstructionUI.SetLayoutUI(displayBTObjects, currentSelectedNode.GridIndex, _mapBlockManager.blockSize);
            ConstructionUI.transform.position = currentSelectedNode.WorldSpace;
            ConstructionUI.Show(true);
        }

        #region Build Tower UI
        private ConstructionView.DisplayUIComp[] GetInitialTowerPlacement()
        {
            List<TowerStats> firstLevelTowers = _statHolder.FindObjectByType<TowerStats>();
            firstLevelTowers = firstLevelTowers.FindAll(x => x.level == 1);

            int towerLength = firstLevelTowers.Count;

            ConstructionView.DisplayUIComp[] uiCompArray = new ConstructionView.DisplayUIComp[towerLength];

            for (int i = 0; i < towerLength; i++)
            {
                int index = i;
                ConstructionView.DisplayUIComp uiComp = FormTowerUIComp("$" + firstLevelTowers[i].cost, firstLevelTowers[i].id, firstLevelTowers[i].sprite,
                () =>
                {
                    SelectTowerToBuild(firstLevelTowers[index].id);
                });

                uiCompArray[i] = uiComp;
            }

            return uiCompArray;
        }
        #endregion

        #region Upgrade Tower UI
        private ConstructionView.DisplayUIComp[] GetTowerUpgradePath(TowerUnit towerUnit)
        {
            if (towerUnit.unitStats == null)
                return null;

            TowerStats towerStats = (TowerStats)towerUnit.unitStats;
            List<ConstructionView.DisplayUIComp> uiComps = new List<ConstructionView.DisplayUIComp>();

            if (towerStats.upgrade_path != null && towerStats.upgrade_path.Length > 0)
            {
                int upgradePathLength = towerStats.upgrade_path.Length;

                for (int i = 0; i < upgradePathLength; i++)
                    uiComps.Add(GetTowerUIComp(towerUnit, towerStats.upgrade_path[i]));
            }

            uiComps.Add(GetTowerInfoComp(towerStats));
            uiComps.Add(GetTowerSaleComp(towerStats));

            return uiComps.ToArray();
        }

        private ConstructionView.DisplayUIComp GetTowerUIComp(TowerUnit towerUnit, TowerStats towerStats)
        {
            return FormTowerUIComp("$" + towerStats.cost, towerStats.id, towerStats.sprite,
                () =>
                {
                    SelectTowerToUpgrade(towerUnit, towerStats);
                });
        }

        private ConstructionView.DisplayUIComp GetTowerInfoComp(TowerStats towerStats)
        {
            return FormTowerUIComp("Info", towerStats.id, UtilityMethod.LoadSpriteFromMulti(spriteSheet, VariableFlag.TowerSpriteID.TowerInfo),
                () =>
                {
                    SelectTowerToInfo(currentSelectedNode, towerStats);
                });
        }

        private ConstructionView.DisplayUIComp GetTowerSaleComp(TowerStats towerStats)
        {
            return FormTowerUIComp("Sale", towerStats.id, UtilityMethod.LoadSpriteFromMulti(spriteSheet, VariableFlag.TowerSpriteID.TowerSale),
                () =>
                {
                    SelectTowerToSale(currentSelectedNode, towerStats);
                });
        }

        private ConstructionView.DisplayUIComp FormTowerUIComp(string label, string id, Sprite sprite, System.Action clickCallback)
        {
            ConstructionView.DisplayUIComp uiComp = new ConstructionView.DisplayUIComp();

            uiComp.label = label;

            uiComp._id = id;

            uiComp.sprite = sprite;

            uiComp.ClickEvent = () =>
            {
                clickCallback();
            };

            return uiComp;
        }
        #endregion

        public async void DelayReset()
        {
            await (GeneralUtility.DoDelayWork(0.01f, () =>
             {
                 Reset();
             }));
        }

        private bool IsTileNodeValid(TileNode tileNode) {
            return !(currentSelectedNode.TileMapPlace == null || !tileNode.IsWalkable || (tileNode.monsterUnit != null && tileNode.monsterUnit.Count > 0));
        }

        private void Reset()
        {
            currentSelectedNode.TilemapMember = null;
            ConstructionUI.Show(false);
            isTaskDone = false;
            
        }

        private void OnDestroy()
        {
            if (_gameInputManager != null)
                _gameInputManager.OnSelectTileNode -= SelectTileListener;
        }
    }
}
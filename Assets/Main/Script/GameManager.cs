﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using TD.Map;
using TD.Unit;
using TD.AI;
using TD.Database;
using TD.UI;
using Hsinpa.StateEntity;
using TD.Storyboard;

public class GameManager : MonoBehaviour
{
    #region Core Class
    private MapBlockManager _blockManager;
    private MapGrid _mapGrid;
    private InGameUICtrl _gameInteractorCtrl;
    private GameInputManager _gameInputManager;
    private PoolManager _poolManager;
    private GameUnitManager _gameUnitManager;
    private StoryBoardManager _storyBoardManager;

    #endregion

    #region UI Class
    private HeaderView _headView;
    private MapBlockBottomView _mapBlockBottomView;
    #endregion

    [SerializeField]
    STPTheme poolingTheme;

    [SerializeField]
    StatsHolder statsHolder;

    private void Awake()
    {
        StateEntityManager.Dispose();

        var monsterPools = statsHolder.FindObjectByType<MonsterStats>();

        _blockManager = GetComponentInChildren<MapBlockManager>();

        _mapGrid = GetComponentInChildren<MapGrid>();

        _gameInputManager = GetComponentInChildren<GameInputManager>();
        _gameInteractorCtrl = GetComponentInChildren<InGameUICtrl>();
        _poolManager = GetComponentInChildren<PoolManager>();
        _gameUnitManager = GetComponentInChildren<GameUnitManager>();
        //_levelDesignManager = GetComponentInChildren<LevelDesignManager>();
        _storyBoardManager = GetComponentInChildren<StoryBoardManager>();

        _headView = GetComponentInChildren<HeaderView>();
        _mapBlockBottomView = GetComponentInChildren<MapBlockBottomView>();

        _mapGrid.SetUp();
        _gameUnitManager.SetUp(_blockManager, _mapGrid, poolingTheme.total);
        _gameInputManager.SetUp(_mapGrid, _blockManager, _gameUnitManager);
        //_levelDesignManager.Init(_gameUnitManager, _blockManager, _mapGrid, monsterPools);
        _storyBoardManager.Init(_gameUnitManager, _blockManager, _mapGrid, monsterPools);

        _gameInteractorCtrl.SetUp(_gameInputManager, _gameUnitManager, _mapGrid, _blockManager, poolingTheme, statsHolder);

    }

    public void Start() {
        if (_poolManager != null && poolingTheme != null)
            PreparePoolingObject(poolingTheme);

        Init();
    }

    private void Init()
    {
        _gameUnitManager.Reset();
        _blockManager.ReadTilemap();

        PlayerModel mainPlayer = PlayerModel.CreatePlayer();

        //_levelDesignManager.SetLevel(new List<PlayerModel> { mainPlayer }, mainPlayer);
        _headView.SetUp(mainPlayer);
        _mapBlockBottomView.SetUp(_blockManager);

        _storyBoardManager.StartStoryBoard("stroy_group_01");
    }

    private void PreparePoolingObject(STPTheme poolingTheme) {
        if (poolingTheme.stpObjectHolder != null) {
            for (int i = 0; i < poolingTheme.stpObjectHolder.Count; i++) {
                _poolManager.CreatePool(poolingTheme.stpObjectHolder[i].prefab, poolingTheme.stpObjectHolder[i]._id, poolingTheme.stpObjectHolder[i].poolingNum);
            }
        }
    }

}

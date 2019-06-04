﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using TD.Map;
using TD.Unit;
using TD.AI;

public class GameManager : MonoBehaviour
{
    private MapHolder _mapHolder;
    private MapGrid _mapGrid;
    private InGameUICtrl _gameInteractorCtrl;
    private GameInputManager _gameInputManager;
    private PoolManager _poolManager;
    private GameUnitManager _gameUnitManager;
    private LevelDesignManager _levelDesignManager;

    [SerializeField]
    STPTheme poolingTheme;

    private void Awake()
    {
        _mapHolder = GetComponentInChildren<MapHolder>();

        _mapGrid = GetComponentInChildren<MapGrid>();

        _gameInputManager = GetComponentInChildren<GameInputManager>();
        _gameInteractorCtrl = GetComponentInChildren<InGameUICtrl>();
        _poolManager = GetComponentInChildren<PoolManager>();
        _gameUnitManager = GetComponentInChildren<GameUnitManager>();
        _levelDesignManager = GetComponentInChildren<LevelDesignManager>();

        _mapGrid.SetUp();
        _gameInputManager.SetUp(_mapGrid, _mapHolder);
        _gameInteractorCtrl.SetUp(_gameInputManager, _gameUnitManager, _mapGrid, poolingTheme);

        var monsters = poolingTheme.FindObjectByType<STPMonster>();
        _levelDesignManager.SetUp(_gameUnitManager, _mapHolder, _mapGrid, monsters);
    }

    public void Start() {
        if (_poolManager != null && poolingTheme != null)
            PreparePoolingObject(poolingTheme);

        Init();
    }

    private void Init()
    {
        _gameUnitManager.Reset();
        _mapHolder.ReadTilemap();

        _levelDesignManager.CallEveryoneReady();
    }

    private void PreparePoolingObject(STPTheme poolingTheme) {
        if (poolingTheme.stpObjectHolder != null) {
            for (int i = 0; i < poolingTheme.stpObjectHolder.Count; i++) {
                _poolManager.CreatePool(poolingTheme.stpObjectHolder[i].prefab, poolingTheme.stpObjectHolder[i]._id, poolingTheme.stpObjectHolder[i].poolingNum);
            }
        }
    }

}

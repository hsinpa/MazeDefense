using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using TD.Map;
using TD.Unit;

public class GameManager : MonoBehaviour
{
    private MapHolder _mapHolder;
    private MapGrid _mapGrid;
    private InGameUICtrl _gameInteractorCtrl;
    private GameInputManager _gameInputManager;
    private PoolManager _poolManager;
    private GameUnitManager _gameUnitManager;

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

        _mapGrid.SetUp();
        _gameInputManager.SetUp(_mapGrid, _mapHolder);

        _gameInteractorCtrl.SetUp(_gameInputManager, _gameUnitManager, _mapGrid, poolingTheme);

    }

    public void Start() {
        Init();

        if (_poolManager != null && poolingTheme != null)
            PreparePoolingObject(poolingTheme);
    }

    private void Init()
    {
        _gameUnitManager.Reset();
        _mapHolder.ReadTilemap();
    }

    private void PreparePoolingObject(STPTheme poolingTheme) {
        if (poolingTheme.stpObjectHolder != null) {
            for (int i = 0; i < poolingTheme.stpObjectHolder.Count; i++) {
                _poolManager.CreatePool(poolingTheme.stpObjectHolder[i].prefab, poolingTheme.stpObjectHolder[i]._id, poolingTheme.stpObjectHolder[i].poolingNum);
            }
        }
    }

}

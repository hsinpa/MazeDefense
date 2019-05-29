using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
public class GameManager : MonoBehaviour
{
    private MapHolder _mapHolder;
    private MapGrid _mapGrid;
    private InGameUICtrl _gameInteractorCtrl;
    private GameInputManager _gameInputManager;
    private PoolManager _poolManager;

    [SerializeField]
    STPTheme poolingTheme;

    private void Awake()
    {
        _mapHolder = GetComponentInChildren<MapHolder>();

        _mapGrid = GetComponentInChildren<MapGrid>();

        _gameInputManager = GetComponentInChildren<GameInputManager>();
        _gameInteractorCtrl = GetComponentInChildren<InGameUICtrl>();
        _poolManager = GetComponentInChildren<PoolManager>();

        _mapGrid.SetUp();
        _gameInputManager.SetUp(_mapGrid);
        _gameInteractorCtrl.SetUp(_gameInputManager);

    }

    public void Start() {
        Init();
    }

    private void Init()
    {
        _mapHolder.ReadTilemap();
    }

    private void PreparePoolingObject(STPTheme poolingTheme) {
        if (poolingTheme.stpObjectHolder != null) {
            for (int i = 0; i < poolingTheme.stpObjectHolder.Count; i++) {
                _poolManager.CreatePool(poolingTheme.stpObjectHolder[0].prefab, poolingTheme.stpObjectHolder[0]._id, poolingTheme.stpObjectHolder[0].poolingNum);
            }
        }
    }

}

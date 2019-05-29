using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MapHolder _mapHolder;
    private MapGrid _mapGrid;
    private InGameUICtrl _gameInteractorCtrl;
    private GameInputManager _gameInputManager;


    private void Awake()
    {
        _mapHolder = GetComponentInChildren<MapHolder>();

        _mapGrid = GetComponentInChildren<MapGrid>();

        _gameInputManager = GetComponentInChildren<GameInputManager>();
        _gameInteractorCtrl = GetComponentInChildren<InGameUICtrl>();

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


}

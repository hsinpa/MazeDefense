using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MapHolder _mapHolder;
    private MapGrid _mapGrid;


    private void Awake()
    {
        _mapHolder = GetComponentInChildren<MapHolder>();

        _mapGrid = GetComponentInChildren<MapGrid>();
        _mapGrid.SetUp();
    }

    public void Start() {
        Init();
    }

    private void Init()
    {
        _mapHolder.ReadTilemap();
    }


}

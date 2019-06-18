using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using Pooling;

public class DummyUnit : MonoBehaviour
{
    MapGrid _mapGrid;

    Vector3 moveDelta;

    public void SetUp(MapGrid mapGrid) {
        _mapGrid = mapGrid;
    }

    private void Update()
    {

        if (_mapGrid == null) return;

        var unitPosition = transform.position;
        var currentTile = _mapGrid.GetTileNodeByWorldPos(transform.position);

        if (currentTile.TilemapMember != null) {

            moveDelta.Set((currentTile.GetFlowFieldPath(VariableFlag.Path.CastleFirst).x), 
                        (currentTile.GetFlowFieldPath(VariableFlag.Path.CastleFirst).y), 0);

            moveDelta *= Time.deltaTime; 

            transform.position += moveDelta;
        }
    }
}

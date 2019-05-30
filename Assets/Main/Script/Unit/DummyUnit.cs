using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log(transform.position);

        var currentTile = _mapGrid.GetTileNodeByWorldPos(transform.position);

        if (currentTile.TilemapMember != null) {

            moveDelta.Set(transform.position.x + (currentTile.FlowFieldDirection.x * Time.deltaTime),
                transform.position.y + (currentTile.FlowFieldDirection.y * Time.deltaTime),
                transform.position.z);

            transform.position = moveDelta;
        }
    }
}

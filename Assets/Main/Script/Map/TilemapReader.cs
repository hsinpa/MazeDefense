using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TD.Map;

public class TilemapReader : MonoBehaviour
{

    private MapComponent mapComponent;
    private TileNode[,] _nodes;

    private void Start()
    {
        mapComponent = this.GetComponent<MapComponent>();
        _nodes = new TileNode[mapComponent.gridSize.x, mapComponent.gridSize.y];

        ReadGrid(this.transform);
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);
    //        Node _node;

    //        if (_nodes.TryGetValue(worldPoint, out _node))
    //        {
    //            print("Tile worldPoint " + _node.worldPosition + ", " + _node.walkable);
    //        }
    //    }
    //}

    public void ReadGrid(Transform p_grid)
    {
        for (int i = 0; i < p_grid.childCount; i++)
        {
            Tilemap tile = p_grid.GetChild(i).GetComponent<Tilemap>();

            if (tile == null) continue;

            foreach (Vector3Int pos in tile.cellBounds.allPositionsWithin)
            {

                if (!tile.HasTile(pos)) continue;
                Vector3Int localSpace = new Vector3Int(Mathf.RoundToInt(mapComponent.size.x + pos.x), Mathf.RoundToInt(mapComponent.size.y + pos.y), pos.z);

                var key = tile.CellToWorld(pos);
                var tileBase = tile.GetTile(pos);
                var node = new TileNode();

                node.IsWalkable = true;
                node.Cost = 1;
                node.TilemapMember = tile;
                node.TileBase = tileBase;
                node.LocalPlace = localSpace;
                _nodes[localSpace.x, localSpace.y] = node;
            }
        }
    }
}

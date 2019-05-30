using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TD.Map;

public class TilemapReader
{
    private MapComponent mapComponent;

    public TileNode[,] nodes {
        get {
            return _nodes;
        }
    }
    private TileNode[,] _nodes;

    public TilemapReader(MapComponent mapComponent)
    {
        this.mapComponent = mapComponent;
        _nodes = new TileNode[mapComponent.fullSize.x, mapComponent.fullSize.y];

        ReadGrid();
    }

    public void ReadGrid()
    {
        int length = mapComponent.tilemaps.Length;

        for (int i = 0; i < length; i++)
        {
            Tilemap tile = mapComponent.tilemaps[i];

            if (tile == null) continue;
            foreach (Vector3Int pos in tile.cellBounds.allPositionsWithin)
            {

                if (!tile.HasTile(pos)) {
                    continue;
                };

                Vector3Int localSpace = new Vector3Int(Mathf.RoundToInt(mapComponent.radiusSize.x + pos.x),
                                                        Mathf.FloorToInt(mapComponent.offsetAnchor.y + mapComponent.radiusSize.y + pos.y), 
                                                        pos.z);

                var key = tile.CellToWorld(pos);
                var tileBase = tile.GetTile(pos);
                var node = new TileNode();

                node.IsWalkable = true;
                node.Cost = 1;
                node.TilemapMember = tile;
                node.TileBase = tileBase;
                node.LocalPlace = localSpace;
                node.TileMapPlace = pos;
                _nodes[localSpace.x, localSpace.y] = node;


            }
        }
    }
}

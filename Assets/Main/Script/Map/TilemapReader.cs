using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TD.Map;

public class TilemapReader
{
    private BlockComponent mapComponent;

    public TileNode[,] nodes {
        get {
            return _nodes;
        }
    }
    private TileNode[,] _nodes;

    public TilemapReader(BlockComponent mapComponent)
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

                var node = ParseTileInfo(tile, pos);
                    node.LocalPlace = localSpace;

                _nodes[localSpace.x, localSpace.y] = node;
            }
        }
    }

    private TileNode ParseTileInfo(Tilemap tile, Vector3Int TileMapPlace) {
        var node = new TileNode();
        var key = tile.CellToWorld(TileMapPlace);
        var tileBase = tile.GetTile(TileMapPlace);

        node.TilemapMember = tile;
        node.TileBase = tileBase;
        node.TileMapPlace = TileMapPlace;
        node.IsWalkable = true;
        node.Cost = 1;
        node.FlowFieldDirectionSet = new Dictionary<VariableFlag.Path, Vector2>();

        if (tileBase.GetType() == typeof(CustomTile))
        {
            CustomTile customTile = (CustomTile)tileBase;
            node.IsWalkable = customTile.walkable;
            node.Cost = customTile.cost;
        }

        return node;
    }
}

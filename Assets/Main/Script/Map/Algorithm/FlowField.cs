using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TD.Map;
using UnityEngine;

public class FlowField
{

    private Vector3 zeroVector3 = new Vector3(0, 0, 0);
    private Queue<TileNode> frontier = new Queue<TileNode>();
    private HashSet<Vector2Int> came_from = new HashSet<Vector2Int>();
    private Vector2Int[] directionSet;
    private int directionSetLength;

    public FlowField() {
        directionSet = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
        directionSetLength = directionSet.Length;
    }

    public async Task<TileNode[,]> Execute(TileNode[,] tileNodes, TileNode[] targetNodes, Vector2Int nodeSize, VariableFlag.Strategy pathTag) {
        frontier.Clear();
        came_from.Clear();

        int targetNodeLength = targetNodes.Length;
        for (int t = 0; t < targetNodeLength; t++) {
            frontier.Enqueue(tileNodes[targetNodes[t].GridIndex.x, targetNodes[t].GridIndex.y]);
            came_from.Add(targetNodes[t].GridIndex);
        }

        var length = frontier.Count;

        //Hard code exit points
        //frontier.Enqueue(tileNodes[4, 0]);
        //frontier.Enqueue(tileNodes[5, 0]);
        while (length > 0) {
            TileNode current = frontier.Dequeue();
            length--;

            TileNode[] neighbours = GetNeighbours(tileNodes, nodeSize, current);
            for (int n = 0; n < directionSetLength; n++) {
                var neighbour = neighbours[n];

                if (neighbour.TileBase == null || !neighbour.IsWalkable || neighbour.towerUnit != null)
                    continue;

                if (!came_from.Contains(neighbour.GridIndex))
                {
                    frontier.Enqueue(neighbour);
                    came_from.Add(neighbour.GridIndex);


                    //tileNodes[neighbour.GridIndex.x, neighbour.GridIndex.y].FlowFieldDirection = current.GridIndex - neighbour.GridIndex;
                    tileNodes[neighbour.GridIndex.x, neighbour.GridIndex.y].AddFlowField(pathTag, current.GridIndex - neighbour.GridIndex);
                    length++;
                }
            }
        }

        return tileNodes;
    }

    public async Task<TileNode[,]> ClearTileNodePath(TileNode[,] tileNodes, Vector2Int nodeSize) {
        for (int x = 0; x < nodeSize.x; x++)
        {
            for (int y = 0; y < nodeSize.y; y++)
            {
                tileNodes[x, y].FlowFieldDirectionSet.Clear();
            }
        }

        return tileNodes;
     }

    public TileNode[] GetNeighbours(TileNode[,] tileNodes, Vector2Int nodeSize, TileNode node)
    {
        TileNode[] neighbours = new TileNode[directionSetLength];

        //for (int x = -1; x <= 1; x++)
        //{
        //    for (int y = -1; y <= 1; y++)
        //    {
        //        if (x == 0 && y == 0)
        //            continue;

        //        int checkX = node.GridIndex.x + x;
        //        int checkY = node.GridIndex.y + y;

        //        if (checkX >= 0 && checkX < nodeSize.x && checkY >= 0 && checkY < nodeSize.y)
        //        {
        //            neighbours.Add(tileNodes[checkX, checkY]);
        //        }
        //    }
        //}


        Vector2 NeighbourPos = Vector2.zero;

        for (int i = 0; i < directionSetLength; i++) {
            Vector2Int dirSet = directionSet[i];

            int checkX = node.GridIndex.x + dirSet.x;
            int checkY = node.GridIndex.y + dirSet.y;

            if (checkX >= 0 && checkX < nodeSize.x && checkY >= 0 && checkY < nodeSize.y)
            {
                neighbours[i] = (tileNodes[checkX, checkY]);
            }
        }

        return neighbours;
    }

    private TileNode[,] EraseTileNextPath(TileNode[,] tileNodes, Vector2Int nodeSize) {
        for (int x = 0; x < nodeSize.x; x++)
        {
            for (int y = 0; y < nodeSize.y; y++)
            {
                //tileNodes[x, y].FlowFieldDirection = zeroVector3;

                tileNodes[x, y].FlowFieldDirectionSet.Clear();
            }
        }

        return tileNodes;

    }


}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TD.Map;
using UnityEngine;

public class FlowField
{

    private Vector3 zeroVector3 = new Vector3(0, 0, 0);

    public void ExecuteAsyn(TileNode[,] tileNodes, TileNode[] targetNodes, Vector2Int nodeSize, VariableFlag.Path pathTag, System.Action<TileNode[,]> callback) {
        Thread t = new Thread(new ThreadStart(() => {

            tileNodes = EraseTileNextPath(tileNodes, nodeSize);
            tileNodes = Execute(tileNodes, targetNodes, nodeSize, pathTag);

            if (callback != null)
                callback(tileNodes);
        }));
        t.Start();
    }

    public TileNode[,] Execute(TileNode[,] tileNodes, TileNode[] targetNodes, Vector2Int nodeSize, VariableFlag.Path pathTag) {

        var frontier = new Queue<TileNode>();

        var came_from = new List<Vector2Int>();

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


            List<TileNode> neighbours = GetNeighbours(tileNodes, nodeSize, current);
            for (int n = 0; n < neighbours.Count; n++) {
                var neighbour = neighbours[n];

                if (!neighbour.IsWalkable || neighbour.towerUnit != null)
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

    public List<TileNode> GetNeighbours(TileNode[,] tileNodes, Vector2Int nodeSize, TileNode node)
    {
        List<TileNode> neighbours = new List<TileNode>();

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

        Vector2Int[] directionSet = new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0) };
        foreach (Vector2Int dirSet in directionSet)
        {

            int checkX = node.GridIndex.x + dirSet.x;
            int checkY = node.GridIndex.y + dirSet.y;

            if (checkX >= 0 && checkX < nodeSize.x && checkY >= 0 && checkY < nodeSize.y)
            {
                neighbours.Add(tileNodes[checkX, checkY]);
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

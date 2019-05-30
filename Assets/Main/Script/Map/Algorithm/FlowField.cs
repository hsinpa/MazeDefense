using System.Collections;
using System.Collections.Generic;
using TD.Map;
using UnityEngine;

public class FlowField
{
    private TileNode[,] tileNodes;
    private Vector2Int nodeSize;
    Vector2Int handStartPoint = new Vector2Int(0, 5);

    public FlowField(TileNode[,] blocks, Vector2Int nodeSize) {

        this.tileNodes = blocks;
        this.nodeSize = nodeSize;
    }


    public TileNode[,] Execute() {

        var length = 1;
        var frontier = new Queue<TileNode>();
        frontier.Enqueue(tileNodes[handStartPoint.x, handStartPoint.y]);

        var came_from = new Dictionary<TileNode, TileNode>();

        while (length > 0) {
            TileNode current = frontier.Dequeue();
            length--;



        }


        return tileNodes;
    }

}

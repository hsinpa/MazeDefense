using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TD.Map {

    public class MapGrid : MonoBehaviour
    {
        private MapHolder mapHolder;
        private List<TileNode[,]> tilenode;

        private TileNode[,] tilenodes;

        private FlowField _flowField;

        public int gridHeight, gridWidth;

        public void SetUp()
        {
            tilenode = new List<TileNode[,]>();
            mapHolder = this.GetComponent<MapHolder>();
            mapHolder.OnAddMapComponent += OnAddBlock;
            _flowField = new FlowField();
        }

        public void ReformMap()
        {
            tilenode.Clear();
            int mLength = mapHolder.mapComponents.Count;

            gridHeight = Mathf.RoundToInt(mLength * mapHolder.sampleSize.y * 2);
            gridWidth = Mathf.RoundToInt(mapHolder.sampleSize.x * 2);

            for (int i = mLength - 1; i >= 0; i--)
            {
                tilenode.Add(mapHolder.mapComponents[i].tilemapReader.nodes);
            }

            tilenodes = ReorganizedTileNode(tilenode, new Vector2Int(gridWidth, Mathf.RoundToInt(mapHolder.sampleSize.y * 2)));

            tilenodes = _flowField.Execute(tilenodes, new Vector2Int(gridWidth, gridHeight));


        }


        private TileNode[,] ReorganizedTileNode(List<TileNode[,]> p_tileBlocks, Vector2Int blockSize)
        {

            int blockLength = p_tileBlocks.Count;
            TileNode[,] tileNode = new TileNode[blockSize.x, blockSize.y * blockLength];
            int tOffset = blockLength - 1;

            for (int t = 0; t < blockLength; t++)
            {

                for (int x = 0; x < blockSize.x; x++)
                {
                    for (int y = 0; y < blockSize.y; y++)
                    {
                        p_tileBlocks[t][x, y].GridIndex = new Vector2Int(x, y + ((t) * blockSize.y));
                        tileNode[x, y + ((t) * blockSize.y)] = p_tileBlocks[t][x, y];
                    }
                }
            }

            return tileNode;
        }
        
        public TileNode GetTileNodeByWorldPos(Vector3 point)
        {

            //Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int yOffset = Mathf.CeilToInt(mapHolder.minPos.y - mapHolder.cameraTop);
            var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x + (mapHolder.sampleSize.x)), Mathf.FloorToInt(point.y + yOffset), 0);

            //float fullHeight = mapHolder.sampleSize.y * 2;
            //int componentIndex = Mathf.FloorToInt(worldPoint.y / (fullHeight));
            //int tileNodeIndex = Mathf.RoundToInt(worldPoint.y % (fullHeight));

            //if (componentIndex >= 0 && componentIndex < tilenode.Count &&
            //    worldPoint.x >= 0 && worldPoint.x < mapHolder.sampleSize.x * 2 &&
            //    tileNodeIndex >= 0 && tileNodeIndex < fullHeight
            //    )
            //{
            //    var selectedNode = tilenode[componentIndex][worldPoint.x, tileNodeIndex];

            //    return (selectedNode);
            //}

            if (worldPoint.x >= 0 && worldPoint.x < mapHolder.sampleSize.x * 2 &&
                worldPoint.y >= 0 && worldPoint.y < mapHolder.sampleSize.y * 2 * tilenode.Count
                )
            {


                var selectedNode = tilenodes[worldPoint.x, worldPoint.y];
                //Debug.Log(selectedNode.GridIndex +", Move Direction " + selectedNode.FlowFieldDirection);

                return (selectedNode);
            }

            return default(TileNode);
        }

        private void OnAddBlock(MapComponent mapComponent)
        {
            ReformMap();
        }

        private void OnDestroy()
        {
            if (mapHolder != null)
                mapHolder.OnAddMapComponent -= OnAddBlock;
        }
    }

}

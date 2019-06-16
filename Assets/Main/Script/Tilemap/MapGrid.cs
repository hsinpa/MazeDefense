using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Unit;

namespace TD.Map {

    public class MapGrid : MonoBehaviour
    {
        private MapBlockManager mapHolder;
        private List<TileNode[,]> raw_tileData;

        private TileNode[,] tilenodes;

        private FlowField _flowField;

        public int gridHeight, gridWidth;

        public System.Action OnMapReform;

        public void SetUp()
        {
            raw_tileData = new List<TileNode[,]>();
            mapHolder = this.GetComponent<MapBlockManager>();
            mapHolder.OnAddMapComponent += OnAddBlock;
            _flowField = new FlowField();
        }

        public void ReformMap()
        {
            raw_tileData.Clear();
            int mLength = mapHolder.mapComponents.Count;

            gridHeight = Mathf.RoundToInt(mLength * mapHolder.blockRadius.y * 2);
            gridWidth = Mathf.RoundToInt(mapHolder.blockRadius.x * 2);

            for (int i = mLength - 1; i >= 0; i--)
            {
                raw_tileData.Add(mapHolder.mapComponents[i].tilemapReader.nodes);
            }

            tilenodes = ReorganizedTileNode(raw_tileData, new Vector2Int(gridWidth, Mathf.RoundToInt(mapHolder.blockRadius.y * 2)));

            if (OnMapReform != null)
                OnMapReform();

            RefreshMonsterFlowFieldMap();
        }

        public void EditUnitState(Vector2Int index, UnitInterface unit, bool isAdd) {
            if (ValidateNodeIndex(index.x, index.y)) {

                if (unit.GetType() == typeof(TowerUnit)) {
                    if (isAdd)
                        tilenodes[index.x, index.y].towerUnit = (TowerUnit)unit;
                    else {
                        if (tilenodes[index.x, index.y].towerUnit == (TowerUnit)unit)
                            tilenodes[index.x, index.y].towerUnit = null;
                    }
                }
                else {

                    if (isAdd)
                        tilenodes[index.x, index.y].AddMonsterUnit((MonsterUnit)unit);
                    else
                        tilenodes[index.x, index.y].RemoveMonsterUnit((MonsterUnit)unit);
                }
            }
        }

        public List<T> FindUnitsFromRange<T>(TileNode centerNode, float range) where T : class
        {
            int intRange = Mathf.RoundToInt(range);
            List<T> unitList = new List<T>(); 

            if (ValidateNodeIndex(centerNode.GridIndex.x, centerNode.GridIndex.y)) {

                int startPointX = centerNode.GridIndex.x - intRange,
                    startPointY = centerNode.GridIndex.y - intRange,
                    endPointX = centerNode.GridIndex.x + intRange,
                    endPointY = centerNode.GridIndex.y + intRange;

                for (int x = startPointX; x <= endPointX; x++) {
                    for (int y = startPointY; y <= endPointY; y++)
                    {

                        if (x == centerNode.GridIndex.x && y == centerNode.GridIndex.y)
                            continue;

                        if (ValidateNodeIndex(x, y)) {

                            if (typeof(T) == typeof(MonsterUnit) && tilenodes[x, y].monsterUnit != null) {
                                unitList.AddRange(tilenodes[x,y].monsterUnit as List<T>);
                            } else if (typeof(T) == typeof(TowerUnit) && tilenodes[x, y].towerUnit != null)
                            {
                                unitList.Add(tilenodes[x, y].towerUnit as T);
                            }
                        }
                    }
                }
            }

            return unitList;
        }

        public void RefreshMonsterFlowFieldMap() {
            _flowField.ExecuteAsyn(tilenodes, new Vector2Int(gridWidth, gridHeight), (TileNode[,] resultNodes) => {
                tilenodes = resultNodes;
            });
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
            var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x + (mapHolder.blockRadius.x)), Mathf.FloorToInt(point.y + yOffset), 0);

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

            if (ValidateNodeIndex(worldPoint.x, worldPoint.y))
            {
                var selectedNode = tilenodes[worldPoint.x, worldPoint.y];

                //Debug.Log(selectedNode.GridIndex +", Move Direction " + selectedNode.FlowFieldDirection);

                return (selectedNode);
            }

            return default(TileNode);
        }

        private void OnAddBlock(BlockComponent mapComponent)
        {
            ReformMap();
        }

        private bool ValidateNodeIndex(int x, int y) {
            return (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight);
        }

        private void OnDestroy()
        {
            if (mapHolder != null)
                mapHolder.OnAddMapComponent -= OnAddBlock;
        }

    }
}

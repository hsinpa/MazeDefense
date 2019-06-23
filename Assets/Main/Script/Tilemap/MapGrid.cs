using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Unit;
using System.Threading.Tasks;

namespace TD.Map {

    public class MapGrid : MonoBehaviour
    {
        private MapBlockManager mapHolder;
        private List<TileNode[,]> raw_tileData;

        private List<TowerUnit> allTowerUnit = new List<TowerUnit>();

        private TileNode[,] tilenodes;

        private FlowField _flowField;

        public int gridHeight, gridWidth;

        public System.Action OnMapReform;

        public TileNode[] DestinationNode { get { return _DestinationNode; } }
        private TileNode[] _DestinationNode;
        
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
            allTowerUnit.Clear();
            int mLength = mapHolder.mapComponents.Count;

            gridHeight = Mathf.RoundToInt(mLength * mapHolder.blockRadius.y * 2);
            gridWidth = Mathf.RoundToInt(mapHolder.blockRadius.x * 2);

            for (int i = mLength - 1; i >= 0; i--)
            {
                TilemapReader tilemapReader = mapHolder.mapComponents[i].tilemapReader;
                raw_tileData.Add(tilemapReader.nodes);
            }

            //Clear tile node
            tilenodes = ReorganizedTileNode(raw_tileData, new Vector2Int(gridWidth, Mathf.RoundToInt(mapHolder.blockRadius.y * 2)));

            if (OnMapReform != null)
                OnMapReform();

            RefreshMonsterFlowFieldMap();
        }

        public void EditUnitState(Vector2Int index, UnitInterface unit, bool isAdd) {
            if (ValidateNodeIndex(index.x, index.y)) {

                if (unit.GetType() == typeof(TowerUnit)) {
                    if (isAdd) {
                        allTowerUnit.Add((TowerUnit)unit);
                        tilenodes[index.x, index.y].towerUnit = (TowerUnit)unit;
                    }
                    else {
                        if (tilenodes[index.x, index.y].towerUnit == (TowerUnit)unit) {
                            tilenodes[index.x, index.y].towerUnit = null;
                            allTowerUnit.Remove((TowerUnit)unit);
                        }
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
                                unitList.AddRange(tilenodes[x, y].monsterUnit as List<T>);
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

        public async void RefreshMonsterFlowFieldMap() {

            Vector2Int fullSize = new Vector2Int(gridWidth, gridHeight);

            //Remove all path sign
            TileNode[,] resultNode = await _flowField.ClearTileNodePath(tilenodes, fullSize);

            resultNode = await _flowField.Execute(resultNode, DestinationNode, fullSize, VariableFlag.Strategy.CastleFirst);

            TileNode[] towerTileNode = new TileNode[allTowerUnit.Count];
            for (int t = 0; t < towerTileNode.Length; t++)
                towerTileNode[t] = allTowerUnit[t].currentTile;

            resultNode = await _flowField.Execute(resultNode, towerTileNode, new Vector2Int(gridWidth, gridHeight), VariableFlag.Strategy.TowersFirst);

            lock (tilenodes) {
                tilenodes = resultNode;
            }
        }

        private TileNode[,] ReorganizedTileNode(List<TileNode[,]> p_tileBlocks, Vector2Int blockSize)
        {
            int blockLength = p_tileBlocks.Count;
            TileNode[,] tileNode = new TileNode[blockSize.x, blockSize.y * blockLength];
            List<TileNode> DestinationList = new List<TileNode>();

            for (int t = 0; t < blockLength; t++)
            {

                for (int x = 0; x < blockSize.x; x++)
                {
                    for (int y = 0; y < blockSize.y; y++)
                    {
                        p_tileBlocks[t][x, y].GridIndex = new Vector2Int(x, y + ((t) * blockSize.y));
                        tileNode[x, y + ((t) * blockSize.y)] = p_tileBlocks[t][x, y];

                        //Update Destination node
                        if (p_tileBlocks[t][x, y].customTileType == VariableFlag.CustomTileType.Destination)
                            DestinationList.Add(p_tileBlocks[t][x, y]);
                    }
                }
            }

            _DestinationNode = DestinationList.ToArray();

            return tileNode;
        }

        public TileNode GetTileNodeByWorldPos(Vector3 point)
        {

            //Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int yOffset = Mathf.CeilToInt(mapHolder.minPos.y - mapHolder.cameraTop);
            var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x + (mapHolder.blockRadius.x)), Mathf.FloorToInt(point.y + yOffset), 0);

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

        public TileNode FindTileNodeByIndex(Vector2Int gridIndex) {
            if (!ValidateNodeIndex(gridIndex.x, gridIndex.y))
                return default(TileNode);

            return tilenodes[gridIndex.x, gridIndex.y];
        }

        private void OnDestroy()
        {
            if (mapHolder != null)
                mapHolder.OnAddMapComponent -= OnAddBlock;
        }

    }
}

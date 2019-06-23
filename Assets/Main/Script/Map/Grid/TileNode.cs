using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using TD.Unit;

namespace TD.Map {
    public struct TileNode
    {
        public Vector2Int GridIndex { get; set; }

        public Vector3Int LocalPlace { get; set; }

        public Vector3Int TileMapPlace { get; set; }

        public Vector3 WorldSpace { get {
                //return TilemapMember.CellToWorld(TileMapPlace);
                return TilemapMember.CellToWorld(TileMapPlace) + TilemapMember.tileAnchor;
            }
        }

        public TileBase TileBase { get; set; }

        public Tilemap TilemapMember { get; set; }

        public bool IsWalkable { get; set; }

        public bool IsValidNode { get {
                return TileBase != null;
            }
        }

        public int Cost { get; set; }

        public Dictionary<VariableFlag.Strategy, Vector2> FlowFieldDirectionSet { get; set; }

        public TowerUnit towerUnit { get; set; }

        public List<MonsterUnit> monsterUnit { get; set; }

        public VariableFlag.CustomTileType customTileType { get; set; }

        public BlockComponent BlockComponent { get; set; }

        public void AddFlowField(VariableFlag.Strategy pathTag, Vector2 direction) {
            if (FlowFieldDirectionSet == null)
                FlowFieldDirectionSet = new Dictionary<VariableFlag.Strategy, Vector2>();

            if (!FlowFieldDirectionSet.ContainsKey(pathTag))
                FlowFieldDirectionSet.Add(pathTag, direction);
            else
                FlowFieldDirectionSet[pathTag] = direction;
        }

        public Vector2 GetFlowFieldPath(VariableFlag.Strategy pathTag) {

            if (pathTag == VariableFlag.Strategy.MoveStraight)
                return VariableFlag.Vector.Down;

            if (FlowFieldDirectionSet == null || !FlowFieldDirectionSet.ContainsKey(pathTag))
                return VariableFlag.Vector.Zero;
            
            return FlowFieldDirectionSet[pathTag];
        }

        public void AddMonsterUnit(TD.Unit.MonsterUnit unit)
        {
            if (monsterUnit == null)
                monsterUnit = new List<MonsterUnit>();

            monsterUnit.Add(unit);
        }

        public void RemoveMonsterUnit(TD.Unit.MonsterUnit unit)
        {
            if (monsterUnit != null)
            {
                int mIndex = monsterUnit.IndexOf(unit);

                if (mIndex >= 0)
                    monsterUnit.RemoveAt(mIndex);
            }
        }
    }
}
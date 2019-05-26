using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace TD.Map {
    public struct TileNode
    {
        public Vector3Int LocalPlace { get; set; }

        public Vector3 WorldSpace { get {
                return TilemapMember.CellToWorld(LocalPlace) + TilemapMember.tileAnchor;
            }
        }

        public TileBase TileBase { get; set; }

        public Tilemap TilemapMember { get; set; }

        public bool IsWalkable { get; set; }

        public int Cost { get; set; }
    }
}
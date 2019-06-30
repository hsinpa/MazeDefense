using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TD.Map
{
    public class BlockComponent : MonoBehaviour
    {
        [HideInInspector]
        public Vector2 radiusSize;

        [HideInInspector]
        public Vector2 offsetAnchor;

        public Tilemap[] tilemaps;

        public Transform unitHolder;

        public Vector2Int fullSize;

        public enum Type
        {
            Entrance,
            Exist,
            Free
        }

        public TilemapReader tilemapReader;

        public Type map_type = Type.Free;

        public bool isMoving = false;

        private SpriteRenderer spriteRenderer;

        private Tilemap sample_tilemap;

        public Vector2 targetPos
        {
            get { return _targetPos; }
        }


        private Vector2 _targetPos;

        public void SetTargetPosition(Vector2 p_targetPos)
        {
            p_targetPos.Set(p_targetPos.x, p_targetPos.y + offsetAnchor.y);
            _targetPos = p_targetPos;
        }

        public void SetUp()
        {
            tilemaps = transform.GetComponentsInChildren<Tilemap>();
            sample_tilemap = tilemaps[0];
            
            offsetAnchor = sample_tilemap.tileAnchor;
            radiusSize = sample_tilemap.localBounds.extents;
            fullSize = new Vector2Int(Mathf.RoundToInt(radiusSize.x * 2), Mathf.RoundToInt(radiusSize.y * 2));
            _targetPos = transform.position;

            tilemapReader = new TilemapReader(this);
        }
    }
}
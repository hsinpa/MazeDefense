using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapComponent : MonoBehaviour
{
    [HideInInspector]
    public Vector2 radiusSize;

    [HideInInspector]
    public Vector2 offsetAnchor;

    public Tilemap[] tilemaps;

    public Vector2Int fullSize {
        get {
            return new Vector2Int(Mathf.RoundToInt(radiusSize.x * 2), Mathf.RoundToInt(radiusSize.y * 2));
        }
    }

    private SpriteRenderer spriteRenderer;

    private Tilemap tilemap;

    public Vector2 targetPos {
        get { return _targetPos; }
    }
    private Vector2 _targetPos;

    public TilemapReader tilemapReader;

    public void SetTargetPosition(Vector2 p_targetPos) {
        p_targetPos.Set(p_targetPos.x, p_targetPos.y + offsetAnchor.y);
        _targetPos = p_targetPos;
    }

    public void SetUp()
    {
        //spriteRenderer = this.GetComponent<SpriteRenderer>();
        //bounds = spriteRenderer.bounds;
        tilemaps = transform.GetComponentsInChildren<Tilemap>();
        tilemap = tilemaps[0];

        offsetAnchor = tilemap.tileAnchor;
        radiusSize = tilemap.localBounds.extents;

        //tileOffset = tilemap.tileAnchor.y;
        //bounds.extents.Set(bounds.extents.x, bounds.extents.y - (tileOffset / 2f), bounds.extents.z);
        _targetPos = transform.position;

        tilemapReader = new TilemapReader(this);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapComponent : MonoBehaviour
{
    [HideInInspector]
    public Vector2 size;

    public Vector2Int gridSize {
        get {
            return new Vector2Int(Mathf.RoundToInt(size.x * 2), Mathf.RoundToInt(size.y * 2));
        }
    }

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;

    public Vector2 targetPos {
        get { return _targetPos; }
    }

    private Vector2 _targetPos;

    [HideInInspector]
    public Vector2 offsetAnchor;

    public void SetTargetPosition(Vector2 p_targetPos) {
        p_targetPos.Set(p_targetPos.x, p_targetPos.y + offsetAnchor.y);
        _targetPos = p_targetPos;
    }

    private void Awake()
    {
        //spriteRenderer = this.GetComponent<SpriteRenderer>();
        //bounds = spriteRenderer.bounds;

        tilemap = transform.GetChild(0).GetComponent<Tilemap>();

        offsetAnchor = tilemap.tileAnchor;
        size = tilemap.localBounds.extents;

        //tileOffset = tilemap.tileAnchor.y;
        //bounds.extents.Set(bounds.extents.x, bounds.extents.y - (tileOffset / 2f), bounds.extents.z);
        _targetPos = transform.position;
    }

}

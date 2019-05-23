using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapComponent : MonoBehaviour
{

    public Bounds bounds;

    private SpriteRenderer spriteRenderer;

    public Vector2 targetPos {
        get { return _targetPos; }
    }

    private Vector2 _targetPos;

    public void SetTargetPosition(Vector2 p_targetPos) {
        _targetPos = p_targetPos;
    }

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        bounds = spriteRenderer.bounds;
        _targetPos = transform.position;
    }

}

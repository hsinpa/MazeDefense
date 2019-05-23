using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapComponent : MonoBehaviour
{

    public Bounds bounds;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        bounds = spriteRenderer.sprite.bounds;
        Debug.Log(spriteRenderer.bounds);
    }

}

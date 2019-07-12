using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapBlockView : MonoBehaviour
{

    [SerializeField]
    private TilemapRenderer tilemapRenderer;

    private void Start()
    {
        if (tilemapRenderer != null) {
            Texture tex = tilemapRenderer.material.mainTexture;

            Debug.Log("Is tex work " + (tex != null));
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class CustomTile : RuleTile
{

    public int cost;
    public bool walkable;

    public CustomData[] customData;

    [System.Serializable]
    public struct CustomData
    {
        public float effect;
        public float id;

        public CustomData(float effect, float id)
        {
            this.effect = effect;
            this.id = id;
        }
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/CustomTile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Custom Tile", "New Custom Tile", "Asset", "Save Custom Tile", "Assets/Tilemap/Custom");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomTile>(), path);
    }
#endif
}

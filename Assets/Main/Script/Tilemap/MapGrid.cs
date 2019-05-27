using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;

public class MapGrid : MonoBehaviour
{
    private MapHolder mapHolder;
    private TileNode[,] tilenode;

    public void SetUp()
    {
        mapHolder = this.GetComponent<MapHolder>();
        mapHolder.OnAddMapComponent += OnAddBlock;
    }

    public void ReformMap() {
        int mLength = mapHolder.mapComponents.Count;

        int nodeHeight = Mathf.RoundToInt(mLength * mapHolder.sampleSize.y * 2);
        int nodeWidth = Mathf.RoundToInt(mapHolder.sampleSize.x * 2);

        tilenode = new TileNode[nodeWidth, nodeHeight];

        for (int i = 0; i < mLength; i++) {
           // mapHolder.mapComponents[i].tilemapReader.nodes
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x + (mapHolder.sampleSize.x)), Mathf.FloorToInt(point.y), 0);

            Debug.Log(point +", " + worldPoint );
        }
    }

    private void OnAddBlock(MapComponent mapComponent)
    {
        ReformMap();
    }

    private void OnDestroy()
    {
        if (mapHolder != null)
            mapHolder.OnAddMapComponent -= OnAddBlock;
    }
}

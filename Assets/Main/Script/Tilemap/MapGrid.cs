using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;

public class MapGrid : MonoBehaviour
{
    private MapHolder mapHolder;
    private List<TileNode[,]> tilenode;

    public void SetUp()
    {
        tilenode = new List<TileNode[,]>();
        mapHolder = this.GetComponent<MapHolder>();
        mapHolder.OnAddMapComponent += OnAddBlock;
    }

    public void ReformMap() {
        tilenode.Clear();
        int mLength = mapHolder.mapComponents.Count;

        int nodeHeight = Mathf.RoundToInt(mLength * mapHolder.sampleSize.y * 2);
        int nodeWidth = Mathf.RoundToInt(mapHolder.sampleSize.x * 2);

        //for (int i = 0; i < mLength; i++) {
        //    tilenode.Add(mapHolder.mapComponents[i].tilemapReader.nodes);
        //}
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int yOffset = Mathf.CeilToInt(mapHolder.minPos.y - mapHolder.cameraTop);
            var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x + (mapHolder.sampleSize.x)), Mathf.FloorToInt(point.y + yOffset), 0);

            Debug.Log(point +", " + worldPoint );
            Debug.Log(Mathf.CeilToInt (mapHolder.minPos.y - mapHolder.cameraTop));

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

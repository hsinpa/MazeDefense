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

        for (int i = mLength - 1; i >= 0; i--) {
            tilenode.Add(mapHolder.mapComponents[i].tilemapReader.nodes);
        }
    }

    public TileNode GetTileNodeByWorldPos(Vector3 point) {

        //Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int yOffset = Mathf.CeilToInt(mapHolder.minPos.y - mapHolder.cameraTop);
        var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x + (mapHolder.sampleSize.x)), Mathf.FloorToInt(point.y + yOffset), 0);

        float fullHeight = mapHolder.sampleSize.y * 2;
        int componentIndex = Mathf.FloorToInt(worldPoint.y / (fullHeight));
        int tileNodeIndex = Mathf.RoundToInt(worldPoint.y % (fullHeight));

        if (componentIndex >= 0 && componentIndex < tilenode.Count &&
            worldPoint.x >= 0 && worldPoint.x < mapHolder.sampleSize.x * 2 &&
            tileNodeIndex >= 0 && tileNodeIndex < fullHeight
            )
        {
            var selectedNode = tilenode[componentIndex][worldPoint.x, tileNodeIndex];
            Debug.Log(point + ", " + worldPoint);
            Debug.Log("LocalPlace " + selectedNode.LocalPlace);

            return (selectedNode);
        }

        return default(TileNode);
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

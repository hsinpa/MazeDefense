using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{

    private MapHolder mapHolder;

    public void Start()
    {
        mapHolder = this.GetComponent<MapHolder>();
        mapHolder.OnAddMapComponent += OnAddBlock;
    }

    private void OnAddBlock(MapComponent mapComponent) {
        ReformMap();
    }

    public void ReformMap() {

    }

}

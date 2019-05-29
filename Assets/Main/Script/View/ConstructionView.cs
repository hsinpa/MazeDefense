using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionView : InGameUIBase
{

    public System.Action<int> TowerClickEvent;

    private void Start()
    {
        base.Init();
    }

    public void OnTowerConstructClick(int tower_id) {
        if (TowerClickEvent != null)
            TowerClickEvent(tower_id);
    }

}

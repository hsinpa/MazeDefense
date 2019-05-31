using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionView : InGameUIBase
{

    public System.Action<string> TowerClickEvent;

    private void Start()
    {
        base.Init();
    }

    public void OnTowerConstructClick(string tower_id) {
        if (TowerClickEvent != null)
            TowerClickEvent(tower_id);
    }

}

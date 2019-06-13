using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class ConstructionView : InGameUIBase
{
    public System.Action<string> TowerClickEvent;

    [SerializeField]
    private GameObject ShopObject;

    private enum UILayout {
        EqualSpace,
        SpecificSpace
    }

    private void Start()
    {
        base.Init();
    }

    public void OnTowerConstructClick(string tower_id) {
        if (TowerClickEvent != null)
            TowerClickEvent(tower_id);
    }

    public override void Show(bool isOn)
    {
        base.Show(isOn);

        if (isOn) {
            ReLayoutUIComponent(UILayout.EqualSpace);
        }
    }

    private void ReLayoutUIComponent(UILayout ui_layout) {
        if (ShopObject != null) {
            int componentLength = ShopObject.transform.childCount;
            float space = 360f / componentLength;
            int radius = 1;

            for (int i = 0; i < componentLength; i++) {
                Transform child = ShopObject.transform.GetChild(i);

                float angle = space * i;

                if (componentLength % 2 == 0)
                    angle += space / 2;

                Vector3 rotatePos = MathUtility.AngleToVector3(angle);
                child.position = ShopObject.transform.position + (rotatePos * radius);
            }
        }
    }

}

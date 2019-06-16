using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class ConstructionView : InGameUIBase
{
    public System.Action<string> TowerClickEvent;

    [SerializeField]
    private GameObject ShopObject;

    [SerializeField, Range(0,90)]
    private float item_space = 15f;

    [SerializeField, Range(0, 2)]
    private float item_radius = 1.2f;

    [SerializeField, Range(0, 3)]
    private int padding = 0;


    public enum UILayout {
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

    public void SetEnablePosition(Vector3 position, Vector2 mapIndex, Vector2 mapSize)
    {
        transform.position = position;
        Show(true);


        UILayout uiLayout = GetUiLayout(mapIndex, mapSize, padding);
        int faceDir = GetFaceDirection(mapIndex, mapSize);

        ReLayoutUIComponent(uiLayout, faceDir);
    }

    public override void Show(bool isOn)
    {
        base.Show(isOn);

    }

    private void ReLayoutUIComponent(UILayout ui_layout, int faceDir) {
        if (ShopObject != null) {

            switch (ui_layout) {
                case UILayout.EqualSpace:
                    ToEqualSpaceLayout(ShopObject.transform, item_radius);
                break;

                case UILayout.SpecificSpace:
                    ToSpecificLayout(ShopObject.transform, faceDir, item_radius, item_space);
                break;
            }

        }
    }

    private void ToEqualSpaceLayout(Transform centerObject, float radius) {
        int componentLength = centerObject.childCount;
        float space = 360f / componentLength;

        for (int i = 0; i < componentLength; i++)
        {
            Transform child = centerObject.GetChild(i);

            float angle = space * i;

            if (componentLength % 2 == 0)
                angle += space / 2;

            Vector3 rotatePos = MathUtility.AngleToVector3(angle);
            child.position = centerObject.position + (rotatePos * radius);
        }
    }

    private void ToSpecificLayout(Transform centerObject, float angle, float radius, float space) {
        int componentLength = centerObject.childCount;


        float totalSpace = (componentLength - 1) * space;

        float startAngle = angle - (totalSpace / 2);
        
        for (int i = 0; i < componentLength; i++)
        {
            Transform child = centerObject.GetChild(i);

            Vector3 rotatePos = MathUtility.AngleToVector3(startAngle + (space * i));
            child.position = centerObject.position + (rotatePos * radius);
        }
    }

    private UILayout GetUiLayout(Vector2 mapIndex, Vector2 mapSize, int padding) {
        return (mapIndex.x <= (0 + padding) || mapIndex.x >= (mapSize.x - padding - 1)) ? UILayout.SpecificSpace : UILayout.EqualSpace;
    }

    private int GetFaceDirection(Vector2 mapIndex, Vector2 mapSize)
    {
        return (mapIndex.x < (mapSize.x / 2)) ? 90 : 270;
    }

}

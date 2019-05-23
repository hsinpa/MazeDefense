using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MapHolder : MonoBehaviour
{
    Camera _camera;

    List<MapComponent> _mapComponents;

    float width, height;

    private void Start()
    {
        _camera = Camera.main;
        _mapComponents = GetComponentsInChildren<MapComponent>().ToList();

        height = Camera.main.orthographicSize * 2.0f;
        width = height * Screen.width / Screen.height;

        CalculateMapTargetPos();
    }

    public void AddMapComp(MapComponent mapComponent) {
        int insertIndex = GetComponentIndexByPos(mapComponent.transform.position.y);

        if (insertIndex >= 0) {
            _mapComponents.Insert(insertIndex, mapComponent);
            CalculateMapTargetPos();
        }

    }

    public void RemoveMapComp(MapComponent mapComponent) {
        if (mapComponent == null) return;
        int cIndex = _mapComponents.IndexOf(mapComponent);
        if (cIndex >= 0)
        {
            _mapComponents.RemoveAt(cIndex);
            CalculateMapTargetPos();
        }
    }

    private int GetComponentIndexByPos(float yPos) {
        float heightOffset = height / 2f;

        if (_mapComponents.Count <= 0)
            return 0;

        if (yPos > heightOffset)
            return 0;

        Bounds bounds = _mapComponents[0].bounds;
        float lowestPoint = heightOffset - (_mapComponents[0].bounds.extents.y * 2 * _mapComponents.Count);

        if (yPos < lowestPoint)
            return _mapComponents.Count;

        int index = Mathf.RoundToInt((heightOffset - yPos) / (_mapComponents[0].bounds.extents.y * 2));

        return index;
    }

    void CalculateMapTargetPos()
    {
        float heightOffset = height / 2f;

        for (int i = 0; i < _mapComponents.Count; i++)
        {
            float diff = heightOffset - _mapComponents[i].bounds.extents.y - ((_mapComponents[i].bounds.extents.y * 2) * (i));

            _mapComponents[i].SetTargetPosition(new Vector2(0, diff));
        }
    }

    private void Update()
    {
        int mapLength = _mapComponents.Count;
        for (int i = 0; i < mapLength; i++) {
            if (_mapComponents == null)
                continue;
            
            var transitionPos = Vector2.Lerp(_mapComponents[i].transform.position, _mapComponents[i].targetPos, 0.1f);

            if (Vector2.Distance(transitionPos, _mapComponents[i].targetPos) < 0.01f)
                continue;

            _mapComponents[i].transform.position = transitionPos;
        }
    }

}

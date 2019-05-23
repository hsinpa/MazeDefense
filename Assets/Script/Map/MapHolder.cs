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

        GetFirstObjectSize();
    }

    void GetFirstObjectSize() {
        float heightOffset = height / 2f;

        for (int i = 0; i < _mapComponents.Count; i++) {

            if (i > 0) {
                heightOffset = _mapComponents[i-1].transform.position.y - _mapComponents[i-1].bounds.extents.y;
            }

            float diff = heightOffset - (_mapComponents[i].transform.position.y - _mapComponents[i].bounds.extents.y);
            _mapComponents[i].transform.position = new Vector3(0, diff, 0);
        }

    }



    private void Update()
    {
        
    }

}

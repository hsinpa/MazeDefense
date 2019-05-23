using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    Collider2D[] rayhitCache = new Collider2D[8];

    [SerializeField]
    LayerMask raycastLayer;

    [SerializeField]
    MapHolder mapHolder;

    MapComponent dragObject;

    Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var worldPos = (_camera.ScreenToWorldPoint(Input.mousePosition));
        worldPos.Set(worldPos.x, worldPos.y, 0);

        if (Input.GetMouseButtonDown(0)) {
            CheckDraggingDrag(worldPos);
        }

        Drag(dragObject, worldPos);

        if (Input.GetMouseButtonUp(0))
        {
            Release();
        }
    }

    void CheckDraggingDrag(Vector3 mousePosition) {
        float radius = 0.01f;

        int hits = Physics2D.OverlapCircleNonAlloc(mousePosition, radius, rayhitCache, raycastLayer);
        if (hits > 0) {
            MapComponent tMapComp = (rayhitCache[0].gameObject.transform.parent).GetComponent<MapComponent>();
            if (tMapComp != null) {
                dragObject = tMapComp;
                mapHolder.RemoveMapComp(dragObject);
            }
        }
    }

    void Drag(MapComponent dragObject, Vector3 mousePosition)
    {
        if (dragObject == null)
            return;

        mousePosition.Set(dragObject.transform.position.x, mousePosition.y, 0);

        dragObject.transform.position = mousePosition;
        //Debug.Log(dragObject.name);
    }

    void Release() {

        if (dragObject != null) {
            mapHolder.AddMapComp(dragObject);
        }


        dragObject = null;
    }
}


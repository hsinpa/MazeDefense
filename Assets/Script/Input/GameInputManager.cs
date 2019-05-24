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

    [SerializeField, Range(0, 1)]
    float dragRange;

    [SerializeField, Range(0, 3)]
    float maxDragSensitivity;

    MapComponent dragObject;

    Camera _camera;

    Vector2 initialMousePos;
    Vector2 lastMousePos;

    public enum InputState {
        Scroll,
        PreDragMap,
        DragMap,
        DragComp,
        Click,
        Idle
    }

    private InputState inputState;

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
            CheckMapComponentDrag(worldPos);
        }

        if (Input.GetMouseButton(0)) {
            CheckMapHolderDrag(worldPos);
        }

        Drag(dragObject, worldPos);

        if (Input.GetMouseButtonUp(0))
        {
            Release();
        }

        Scroll();

        mapHolder.ResumeToAppropriatePos();
        mapHolder.CalculateMapTargetPos();
    }

    void Scroll() {
        if (Input.mouseScrollDelta.y != 0)
        {
            mapHolder.transform.position += new Vector3(0, Input.mouseScrollDelta.y, 0) * Time.deltaTime * 10;
            inputState = InputState.Scroll;
        }
        else if (inputState == InputState.Scroll) {
            Release();
        }
    }

    void CheckMapHolderDrag(Vector2 mousePosition) {
        if (inputState == InputState.Idle)
        {
            initialMousePos = mousePosition;
            inputState = InputState.PreDragMap;
            return;
        }
        float delta = (mousePosition - initialMousePos).y;
              delta = Mathf.Clamp(delta, -maxDragSensitivity, maxDragSensitivity);
            
        if (inputState == InputState.PreDragMap && mapHolder.IsWithinMapSizeRange(mousePosition) &&
            Mathf.Abs(delta) > dragRange) {
            inputState = InputState.DragMap;
        }

        if (inputState == InputState.DragMap) {
            mapHolder.transform.position += new Vector3(0, delta, 0) * Time.deltaTime * 10;
        }
    }

    void CheckMapComponentDrag(Vector3 mousePosition) {
        float radius = 0.01f;

        int hits = Physics2D.OverlapCircleNonAlloc(mousePosition, radius, rayhitCache, raycastLayer);
        if (hits > 0) {
            MapComponent tMapComp = (rayhitCache[0].gameObject.transform.parent).GetComponent<MapComponent>();
            if (tMapComp != null) {
                dragObject = tMapComp;
                mapHolder.RemoveMapComp(dragObject);
                mapHolder.CalculateMapTargetPos();
            }
        }
    }

    void Drag(MapComponent dragObject, Vector3 mousePosition)
    {
        if (dragObject == null)
            return;
        inputState = InputState.DragComp;

        mousePosition.Set(dragObject.transform.position.x, mousePosition.y, 0);

        dragObject.transform.position = mousePosition;
        //Debug.Log(dragObject.name);
    }

    void Release() {

        if (dragObject != null) {
            mapHolder.AddMapComp(dragObject);
        }

        inputState = InputState.Idle;
        dragObject = null;
        initialMousePos = Vector2.zero; 
    }
}


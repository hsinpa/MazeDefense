using System.Collections;
using System.Collections.Generic;
using TD.Map;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    Collider2D[] rayhitCache = new Collider2D[8];

    [SerializeField]
    LayerMask raycastLayer;

    [SerializeField, Range(0, 1)]
    float dragRange;

    [SerializeField, Range(1, 30)]
    float dragSpeed = 10;

    [SerializeField, Range(0, 3)]
    float maxDragSensitivity;

    [SerializeField, Range(0, 1)]
    float maxDragVelocity;

    [SerializeField, Range(0, 1)]
    float dragDeprecation;

    float dragVelocity;

    BlockComponent dragObject;

    Camera _camera;

    Vector2 initialMousePos;
    Vector2 lastMousePos;

    private MapGrid mapGrid;
    MapBlockManager mapHolder;

    public enum InputState {
        Scroll,
        DragMap,
        DragComp,
        Click,
        UIFocus,
        Idle
    }

    public InputState inputState;

    #region Event
    public System.Action<TileNode> OnSelectTileNode;
    #endregion
    public void SetUp(MapGrid mapGrid, MapBlockManager mapHolder)
    {
        _camera = Camera.main;
        this.mapGrid = mapGrid;
        this.mapHolder = mapHolder;
    }

    // Update is called once per frame
    void Update()
    {
        if (_camera == null)
            return;

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
            if (inputState == InputState.Click)
                ClickOnMapMaterial(worldPos);

            Release();
        }

        bool outOfBoundary = IsOutOfBoundary();

        if (outOfBoundary)
            dragVelocity = 0;

        Scroll();
        HandleMapHolderVelocity();

        //mapHolder.ResumeToAppropriatePos();
        mapHolder.CalculateMapTargetPos();
    }

    void Scroll() {
        if (Input.mouseScrollDelta.y != 0)
        {
            transform.position += new Vector3(0, Input.mouseScrollDelta.y, 0) * Time.deltaTime * dragSpeed;
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
            inputState = InputState.Click;
            return;
        }
        float delta = (mousePosition - initialMousePos).y;
        delta = Mathf.Clamp(delta, -maxDragSensitivity, maxDragSensitivity);

        if (inputState == InputState.Click && mapHolder.IsWithinMapSizeRange(mousePosition) &&
            Mathf.Abs(delta) > dragRange) {
            inputState = InputState.DragMap;
        }

        if (inputState == InputState.DragMap) {

            bool outOfBoundary = IsOutOfBoundary();
            if (outOfBoundary)
            {
                dragVelocity = 0;
                return;
            }

            dragVelocity = delta * Time.deltaTime * dragSpeed;
            transform.position += new Vector3(0, dragVelocity, 0);
        }
    }

    void CheckMapComponentDrag(Vector3 mousePosition) {
        float radius = 0.01f;

        int hits = Physics2D.OverlapCircleNonAlloc(mousePosition, radius, rayhitCache, raycastLayer);
        if (hits > 0) {
            BlockComponent tMapComp = (rayhitCache[0].gameObject.transform.parent).GetComponent<BlockComponent>();
            if (tMapComp != null && tMapComp.map_type == BlockComponent.Type.Free) {
                dragObject = tMapComp;
                //mapHolder.RemoveMapComp(dragObject);
                //mapHolder.CalculateMapTargetPos();
            }
        }
    }

    bool IsOutOfBoundary() {

        if (mapGrid.gridHeight < mapHolder.cameraTop * 1.5f)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            return false;
        }


        if (inputState != InputState.DragComp)
        {
            if (transform.position.y > 0)
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                return true;
            }

            if (transform.position.y < (-mapHolder.minPos.y + mapHolder.cameraTop))
            {
                transform.position = new Vector3(transform.position.x, (-mapHolder.minPos.y + mapHolder.cameraTop), transform.position.z);
                return true;
            }
        }

        return false;
    }

    void Drag(BlockComponent dragObject, Vector3 mousePosition)
    {
        if (dragObject == null)
            return;
        inputState = InputState.DragComp;

        mousePosition.Set(dragObject.transform.position.x, mousePosition.y, 0);

        dragObject.transform.position = mousePosition;

        //mapHolder.CalculateMapTargetPos();
        dragObject.isMoving = true;

        mapHolder.AutoEditMapComp(dragObject);
        //Debug.Log(dragObject.name);
    }

    void HandleMapHolderVelocity()
    {

        if (inputState == InputState.Idle && dragVelocity != 0 ) {
            dragVelocity = Mathf.Clamp(dragVelocity, -maxDragVelocity, maxDragVelocity);
            transform.position += new Vector3(0, dragVelocity, 0);
            dragVelocity *= dragDeprecation;

            if (Mathf.Abs(dragVelocity) < 0.005f)
                dragVelocity = 0;
        }
    }


    void Release() {

        if (dragObject != null) {
            //mapHolder.AddMapComp(dragObject);
            //mapHolder.AutoEditMapComp(dragObject);
        }

        if (!mapHolder.IsWithinMapSizeRange(transform.position)) {
            dragVelocity = 0;
        }

        inputState = InputState.Idle;
        dragObject = null;
        initialMousePos = Vector2.zero; 
    }

    void ClickOnMapMaterial(Vector3 worldPos) {
        var tile = this.mapGrid.GetTileNodeByWorldPos(worldPos);

        if (tile.TileBase != null && OnSelectTileNode != null) {
            OnSelectTileNode(tile);
        }
    }
}


using Hsinpa.StateEntity;
using Hsinpa.Utility;
using System.Collections;
using System.Collections.Generic;
using TD.Map;
using TD.Unit;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [SerializeField, Range(0, 3)]
    float maxDragVelocity;

    [SerializeField, Range(0, 1)]
    float dragDeprecation;

    float dragVelocity;

    BlockComponent dragObject;

    Camera _camera;

    Vector2 initialMousePos;
    Vector2 lastMousePos;

    private MapGrid mapGrid;
    private MapBlockManager mapHolder;
    private GameUnitManager m_game_unit_manager;

    [SerializeField]
    public Vector3 mouseWorldPos;

    public enum InputState {
        Scroll,
        DragMap,
        DragComp,
        Click,
        UIFocus,
        Idle
    }

    public InputState inputState;
    public bool mainMouseActionFlag = false;

    private float ignoreTimePeriod;

    private static GameInputManager _instance;

    public static GameInputManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameInputManager>();
            }
            return _instance;
        }
    }

    private NewControls m_new_controller;
    private Vector2 m_mouse_pos;
    public Vector2 MousePos => this.m_mouse_pos;

    #region Event
    public System.Action<TileNode> OnSelectTileNode;
    #endregion

    public void AppendIgnoreTime() {
        ignoreTimePeriod = TimeSystem.time + 0.2f;
    }

    public void SetUp(MapGrid mapGrid, MapBlockManager mapHolder, GameUnitManager gameUnitManager)
    {
        _camera = Camera.main;
        this.mapGrid = mapGrid;
        this.mapHolder = mapHolder;
        this.m_game_unit_manager = gameUnitManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (_camera == null)
            return;
        m_mouse_pos = Mouse.current.position.ReadValue();

        mouseWorldPos = (_camera.ScreenToWorldPoint(m_mouse_pos));
        mouseWorldPos.Set(mouseWorldPos.x, mouseWorldPos.y, 0);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            bool object_detected = CheckMapComponentDrag(mouseWorldPos);
            mainMouseActionFlag = true;
        }

        if (mainMouseActionFlag)
        {
            CheckMapHolderDrag(mouseWorldPos);
        }

        Drag(dragObject, mouseWorldPos);

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (inputState == InputState.Click && TimeSystem.time > ignoreTimePeriod)
                ClickOnMapMaterial(mouseWorldPos);

            Release();
            mainMouseActionFlag = false;
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

        Vector2 scrll_delta = Mouse.current.scroll.ReadValue();

        if (scrll_delta.y != 0)
        {
            transform.position += new Vector3(0, scrll_delta.y, 0) * Time.deltaTime * dragSpeed;
            inputState = InputState.Scroll;

            IsOutOfBoundary();
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

        if (Mathf.Abs(delta) > dragRange) {
            initialMousePos = mousePosition;
        }

        if (inputState == InputState.Click && mapHolder.IsWithinMapSizeRange(mousePosition) &&
            Mathf.Abs(delta) > dragRange) {

            initialMousePos = mousePosition;

            inputState = InputState.DragMap;
        }

        if (inputState == InputState.DragMap) {

            bool outOfBoundary = IsOutOfBoundary();
            if (outOfBoundary)
            {
                dragVelocity = 0;
                return;
            }

            dragVelocity += delta * Time.deltaTime * dragSpeed;
            transform.position += new Vector3(0, dragVelocity, 0);
        }
    }

    bool CheckMapComponentDrag(Vector3 mousePosition) {
        float radius = 0.01f;

        int hits = Physics2D.OverlapCircleNonAlloc(mousePosition, radius, rayhitCache, raycastLayer);
        if (hits > 0) {
            BlockComponent tMapComp = (rayhitCache[0].gameObject.transform.parent).GetComponent<BlockComponent>();
            if (tMapComp != null && tMapComp.map_type == BlockComponent.Type.Free) {
                dragObject = tMapComp;
                //mapHolder.RemoveMapComp(dragObject);
                //mapHolder.CalculateMapTargetPos();

                TimeSystem.Pause();
                StateEntityManager.PushEntity(EntityData.Tag.MapBlockDrag);

                return true;
            }
        }
        return false;
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
        TimeSystem.Flow();

        StateEntityManager.RemoveEntity(EntityData.Tag.MapBlockDrag);
    }

    void ClickOnMapMaterial(Vector3 worldPos) {
        var tile = this.mapGrid.GetTileNodeByWorldPos(worldPos);

        if (tile.TileBase != null && OnSelectTileNode != null) {
            OnSelectTileNode(tile);
        }
    }

    private void OnDestroy()
    {
        this.m_new_controller?.Dispose();
    }

    #region Input Event

    #endregion

}


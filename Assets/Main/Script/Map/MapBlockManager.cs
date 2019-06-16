using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TD.Map {

    public class MapBlockManager : MonoBehaviour
    {
        Camera _camera;

        public List<BlockComponent> mapComponents {
            get {
                return _mapComponents;
            }
        }

        [SerializeField]
        private List<BlockComponent> _mapComponents;

        int _mapLength;

        [HideInInspector]
        public float width, height;

        [HideInInspector]
        public float cameraTop, cameraBottom;

        public System.Action<BlockComponent> OnAddMapComponent;

        [HideInInspector]
        public Vector2 blockRadius;

        [HideInInspector]
        public Vector2 blockSize;

        public Vector2 minPos {
            get {
                return new Vector2(0, blockRadius.y * 2 * _mapComponents.Count - 1);
            }
        }

        public void ReadTilemap() {
            _mapComponents = GetComponentsInChildren<BlockComponent>().ToList();
            _mapLength = _mapComponents.Count;

            for (int i = 0; i < _mapLength; i++) {
                _mapComponents[i].SetUp();

                if (blockRadius == Vector2.zero)
                    blockRadius = _mapComponents[i].radiusSize;

                if (blockSize == Vector2.zero)
                    blockSize = _mapComponents[i].fullSize;
            }

            if (OnAddMapComponent != null && _mapLength > 0)
                OnAddMapComponent(_mapComponents[_mapLength - 1]);

            CalculateMapTargetPos();
        }

        private void Awake()
        {
            _camera = Camera.main;

            height = Camera.main.orthographicSize * 2.0f;
            width = height * Screen.width / Screen.height;

            cameraTop = height / 2f;
            cameraBottom = -cameraTop;

            AlignHolderToTop();
        }

        public void AlignHolderToTop() {
            transform.position = new Vector2(0, cameraTop);
        }

        public bool IsWithinMapSizeRange(Vector2 mousePosition) {
            return (_mapLength > 0 && _mapComponents[_mapLength - 1].transform.position.y - blockRadius.y <= mousePosition.y &&
                mousePosition.y < cameraTop
                );
        }

        public void ResumeToAppropriatePos() {
            Vector2 tarPos = transform.position;


            if (transform.position.y < cameraTop)
            {
                tarPos.Set(tarPos.x, cameraTop);
            }
            else if (_mapLength > 0 && (_mapComponents[_mapLength - 1].transform.position.y - blockRadius.y) > 0) 
            {
                tarPos.Set(tarPos.x, minPos.y);
            }

            var transitionPos = Vector2.Lerp(transform.position, tarPos, 0.1f);

            if (Vector2.Distance(transitionPos, transform.position) < 0.01f)
                transform.position = tarPos;

            transform.position = transitionPos;
        }

        public BlockComponent GetMapComponentByPos(Vector2 worldPos) {

            int insertIndex = _mapLength - Mathf.CeilToInt(worldPos.y / _mapComponents[0].fullSize.y) - 1;

            if (insertIndex >= 0 && insertIndex < _mapLength) {
                return _mapComponents[insertIndex];
            }

            return null;
        }

        public void AddMapComp(BlockComponent mapComponent) {
            int insertIndex = GetComponentIndexByPos(mapComponent.transform.position.y + mapComponent.offsetAnchor.y);

            if (insertIndex >= 0) {
                _mapComponents.Insert(insertIndex, mapComponent);
                _mapLength++;
                CalculateMapTargetPos();

                if (OnAddMapComponent != null)
                    OnAddMapComponent(mapComponent);
            }
        }

        public void RemoveMapComp(BlockComponent mapComponent) {
            if (mapComponent == null) return;
            int cIndex = _mapComponents.IndexOf(mapComponent);
            if (cIndex >= 0)
            {
                _mapComponents.RemoveAt(cIndex);
                _mapLength--;
                CalculateMapTargetPos();
            }
        }

        public void AutoEditMapComp(BlockComponent mapComponent) {
            if (mapComponent == null) return;

            int originalIndex = _mapComponents.IndexOf(mapComponent);
            int insertIndex = GetComponentIndexByPos(mapComponent.transform.position.y - mapComponent.offsetAnchor.y);

            if (originalIndex != insertIndex) {

                _mapComponents.RemoveAt(originalIndex);

                //int insertIndex = GetComponentIndexByPos(mapComponent.transform.position.y - mapComponent.offsetAnchor.y);
                _mapComponents.Insert(insertIndex, mapComponent);

                CalculateMapTargetPos();

                if (OnAddMapComponent != null)
                    OnAddMapComponent(mapComponent);
            }
        }

        private int GetComponentIndexByPos(float yPos) {
            if (_mapComponents.Count <= 0)
                return 0;

            if (yPos > transform.position.y)
                return 0;

            float lowestPoint = transform.position.y - (_mapComponents[0].fullSize.y * _mapComponents.Count);

            if (yPos < lowestPoint)
                return _mapComponents.Count - 1;

            int index = Mathf.FloorToInt((transform.position.y - yPos) / (_mapComponents[0].fullSize.y));

            return index;
        }

        public void CalculateMapTargetPos()
        {
            for (int i = 0; i < _mapComponents.Count; i++)
            {
                float diff = transform.position.y - _mapComponents[i].radiusSize.y - ((_mapComponents[i].radiusSize.y * 2) * (i));

                _mapComponents[i].SetTargetPosition(new Vector2(0, diff));
            }
        }

        private void Update()
        {
            for (int i = 0; i < _mapLength; i++) {
                if (_mapComponents == null)
                    continue;

                BlockComponent blockComponent = _mapComponents[i];
                var transitionPos = Vector2.Lerp(blockComponent.transform.position, blockComponent.targetPos, 0.2f);

                if (Vector2.Distance(transitionPos, blockComponent.targetPos) < 0.1f) {
                    blockComponent.transform.position = blockComponent.targetPos;
                    blockComponent.isMoving = false;
                    continue;
                }

                blockComponent.isMoving = true;
                blockComponent.transform.position = transitionPos;
            }
        }

    }
}
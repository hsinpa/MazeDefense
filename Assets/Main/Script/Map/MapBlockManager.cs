using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TD.Map {

    public class MapBlockManager : MonoBehaviour
    {
        Camera _camera;

        public List<MapComponent> mapComponents {
            get {
                return _mapComponents;
            }
        }

        List<MapComponent> _mapComponents;
        int _mapLength;

        float width, height;
        public float cameraTop, cameraBottom;

        public System.Action<MapComponent> OnAddMapComponent;

        [HideInInspector]
        public Vector2 sampleSize;

        public Vector2 minPos {
            get {
                return new Vector2(0, sampleSize.y * 2 * _mapComponents.Count - 1);
            }
        }

        public void ReadTilemap() {
            _mapComponents = GetComponentsInChildren<MapComponent>().ToList();
            _mapLength = _mapComponents.Count;

            for (int i = 0; i < _mapLength; i++) {
                _mapComponents[i].SetUp();

                if (sampleSize == Vector2.zero)
                    sampleSize = _mapComponents[i].radiusSize;
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
            return (_mapLength > 0 && _mapComponents[_mapLength - 1].transform.position.y - sampleSize.y <= mousePosition.y &&
                mousePosition.y < cameraTop
                );
        }

        public void ResumeToAppropriatePos() {
            Vector2 tarPos = transform.position;


            if (transform.position.y < cameraTop)
            {
                tarPos.Set(tarPos.x, cameraTop);
            }
            else if (_mapLength > 0 && (_mapComponents[_mapLength - 1].transform.position.y - sampleSize.y) > 0) 
            {
                tarPos.Set(tarPos.x, minPos.y);
            }

            var transitionPos = Vector2.Lerp(transform.position, tarPos, 0.1f);

            if (Vector2.Distance(transitionPos, transform.position) < 0.01f)
                transform.position = tarPos;

            transform.position = transitionPos;
        }

        public void AddMapComp(MapComponent mapComponent) {
            int insertIndex = GetComponentIndexByPos(mapComponent.transform.position.y);

            if (insertIndex >= 0) {
                _mapComponents.Insert(insertIndex, mapComponent);
                _mapLength++;
                CalculateMapTargetPos();

                if (OnAddMapComponent != null)
                    OnAddMapComponent(mapComponent);
            }
        }

        public void RemoveMapComp(MapComponent mapComponent) {
            if (mapComponent == null) return;
            int cIndex = _mapComponents.IndexOf(mapComponent);
            if (cIndex >= 0)
            {
                _mapComponents.RemoveAt(cIndex);
                _mapLength--;
                CalculateMapTargetPos();
            }
        }

        private int GetComponentIndexByPos(float yPos) {
            if (_mapComponents.Count <= 0)
                return 0;

            if (yPos > transform.position.y)
                return 0;

            float lowestPoint = transform.position.y - (_mapComponents[0].radiusSize.y * 2 * _mapComponents.Count);

            if (yPos < lowestPoint)
                return _mapComponents.Count;

            int index = Mathf.RoundToInt((transform.position.y - yPos) / (_mapComponents[0].radiusSize.y * 2));

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
            
                var transitionPos = Vector2.Lerp(_mapComponents[i].transform.position, _mapComponents[i].targetPos, 0.2f);

                if (Vector2.Distance(transitionPos, _mapComponents[i].targetPos) < 0.1f) {
                    _mapComponents[i].transform.position = _mapComponents[i].targetPos;
                    continue;
                }

                _mapComponents[i].transform.position = transitionPos;
            }
        }

    }
}
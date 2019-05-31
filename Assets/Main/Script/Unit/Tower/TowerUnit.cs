using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using TD.Map;

namespace TD.Unit {
    public class TowerUnit : MonoBehaviour, BaseUnit
    {
        STPTower _stpTower;
        MapGrid _mapGrid;
        System.Action<BaseUnit> OnDestroyCallback;

        [SerializeField]
        private Transform stationObject;

        [SerializeField]
        private Transform gunBodyObject;

        [SerializeField]
        private SpriteRenderer RangeIndicator;

        public GameObject unitObject { get => this.gameObject; }

        public void SetUp(STPTower stpTower, MapGrid mapGrid) {
            _stpTower = stpTower;
            _mapGrid = mapGrid;
        }

        public void ReadyToAction(System.Action<BaseUnit> OnDestroyCallback)
        {
            this.OnDestroyCallback = OnDestroyCallback;
        }

        public void OnUpdate()
        {

        }

        public void Destroy()
        {
            if (this.OnDestroyCallback != null)
                OnDestroyCallback(this);

            this.OnDestroyCallback = null;
            this._stpTower = null;
        }


    }
}

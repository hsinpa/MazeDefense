using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TD.Unit {
    public interface BaseUnit
    {
        GameObject unitObject { get; }

        void ReadyToAction(System.Action<BaseUnit> OnDestroyCallback);
        void OnUpdate();
        void Destroy();
    }
}

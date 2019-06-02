using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TD.Unit {
    public interface UnitInterface
    {
        GameObject unitObject { get; }

        void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback);
        void OnUpdate();
        void Destroy();
    }
}

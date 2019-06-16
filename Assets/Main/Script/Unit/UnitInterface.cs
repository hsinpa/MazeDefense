using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TD.Unit {
    public interface UnitInterface
    {
        GameObject unitObject { get; }
        bool isActive { get; }

        void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback);
        void OnUpdate();

        void OnAttack(float dmg);
        void Destroy();
    }
}

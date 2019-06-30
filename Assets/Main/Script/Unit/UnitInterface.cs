using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using TD.Database;

namespace TD.Unit {
    public interface UnitInterface
    {
        GameObject unitObject { get; }
        TileNode currentTile { get; }
        UnitStats unitStats { get; }

        bool isActive { get; }

        void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback);
        void OnUpdate();

        void OnAttack(float dmg);
        void Destroy();
    }
}

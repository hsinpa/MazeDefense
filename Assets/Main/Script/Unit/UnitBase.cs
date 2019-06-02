using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using TD.Map;

namespace TD.Unit
{
    public abstract class UnitBase : MonoBehaviour, UnitInterface
    {
        public GameObject unitObject { get => this.gameObject; }
        public System.Action<UnitInterface> OnDestroyCallback;

        public void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback)
        {
            this.OnDestroyCallback = OnDestroyCallback;
        }

        public virtual void OnUpdate()
        {

        }

        public void Destroy()
        {

        }

    }
}
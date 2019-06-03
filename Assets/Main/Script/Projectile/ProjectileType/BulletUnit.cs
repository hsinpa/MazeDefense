using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;

namespace TD.Unit
{
    public class BulletUnit : MonoBehaviour, UnitInterface
    {
        public GameObject unitObject { get { return gameObject; } }

        private System.Action<UnitInterface> OnDestroyCallback;

        public bool isActive { get { return OnDestroyCallback != null; } }

        private Vector3 lastPosition;
        private MonsterUnit _monsterUnit;
        private Vector3 moveDelta;
        private float minDist;

        public void SetUp(MonsterUnit monsterUnit)
        {
            _monsterUnit = monsterUnit;
            lastPosition = _monsterUnit.transform.position;
        }

        public void ReadyToAction(System.Action<UnitInterface> OnDestroyCallback)
        {
            this.OnDestroyCallback = OnDestroyCallback;
        }

        public void OnUpdate()
        {
            if (OnDestroyCallback == null) return;

            float tSpeed = 10;

            Vector3 targetPos = (_monsterUnit.isActive) ? _monsterUnit.transform.position : lastPosition;
            Vector3 distance = (targetPos - transform.position);
            Vector3 direction = distance.normalized;

            moveDelta.Set((direction.x), (direction.y), 0);
            moveDelta *= Time.deltaTime * tSpeed;

            lastPosition += moveDelta;

            if (distance.magnitude < minDist) {
                Destroy();
            }
        }

        public void Destroy()
        {
            if (this.OnDestroyCallback != null)
                OnDestroyCallback(this);

            this.OnDestroyCallback = null;
        }

    }
}
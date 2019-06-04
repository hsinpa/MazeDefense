using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TD.Map;
using Pooling;

namespace TD.Unit
{
    public class BulletUnit : MonoBehaviour, UnitInterface
    {
        public GameObject unitObject { get { return gameObject; } }

        private System.Action<UnitInterface> OnDestroyCallback;

        public bool isActive { get { return OnDestroyCallback != null; } }

        private Vector3 lastPosition;
        private Vector3 moveDelta;
        private Vector3 eulerRotation;

        private MonsterUnit _monsterUnit;
        private float minDist = 0.4f;
        private STPBullet _sTPBullet;

        public void SetUp(STPBullet stpBullet, MonsterUnit monsterUnit)
        {
            _sTPBullet = stpBullet;
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

            Vector3 targetPos = (_monsterUnit.isActive) ? _monsterUnit.transform.position : lastPosition;
            Vector3 distance = (targetPos - transform.position);
            Vector3 direction = distance.normalized;

            moveDelta.Set((direction.x), (direction.y), 0);
            moveDelta *= Time.deltaTime * _sTPBullet.moveSpeed;



            transform.position += moveDelta;

            eulerRotation.Set(0, 0, Utility.MathUtility.VectorToAngle(direction)-90);
            transform.eulerAngles = eulerRotation;

            lastPosition = targetPos;

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
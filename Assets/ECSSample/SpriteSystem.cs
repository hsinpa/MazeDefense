using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System;
using Hsinpa.ECS.System;
using Unity.Transforms;
using Unity.Mathematics;
using Hsinpa.ECS.Component;

namespace Hsinpa.ECS.JobSystem {
    [DisableAutoCreation]
    public partial class SpriteSystem : SystemBase
    {
        private CustomInputSystem _customInputSystem;
        private const float DELTA_TIME = 0.02f;

        public void SetUp(CustomInputSystem customInputSystem) {
            _customInputSystem = customInputSystem;
        }

        protected override void OnUpdate()
        {
            float2 axis = _customInputSystem.InputDataComponent.Axis;

            Entities.ForEach((ref Translation translation) =>
            {
                float3 position = translation.Value;
                position.x += axis.x * DELTA_TIME;
                position.y += axis.y * DELTA_TIME;

                translation.Value = position;
            })
            .Schedule();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Hsinpa.ECS.Component {
    public struct InputComponent
    {
        public int mouse_x;
        public int mouse_y;
        public bool mouse_onclick;

        public float2 Axis;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public class MathUtility
    {
        public static float VectorToAngle(Vector2 p_vector) {
            float angle = (Mathf.Atan2(p_vector.y, p_vector.x) * 180 / Mathf.PI);

            return angle;
        }

        public static Vector3 AngleToVector3(float p_angle) {

            return new Vector3( Mathf.Sin(p_angle * Mathf.Deg2Rad), Mathf.Cos(p_angle * Mathf.Deg2Rad), 0);
        }

        public static float NormalizeAngle(float angle)
        {
            float result = angle / 360;

            result = result - Mathf.FloorToInt(result);
            result = result * 360;
            if (result < 0)
            {
                result = 360 - result;
            }
            return result;
        }

    }
}

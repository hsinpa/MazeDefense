using System.Collections;
using System.Collections.Generic;

namespace UtilityBase {
    public class Graph
    {

        public static float Normalized(float value, float max) {
            return value / max;
        }

        public static float QuadraticCurve(float value, float max, float k)
        {
            return (float)System.Math.Pow((value / max), k);
        }

        public static float QuadraticFunction(float value, float a, float b, float c)
        {
            return (float)((a * System.Math.Pow(value, 2)) + (b * value) + c);
        }

        public static float Linear(float value, float m, float b)
        {
            return (value * m) + b;
        }

        public static float Logistic(float value)
        {
            return  (float)(1 / (1 + System.Math.Exp(-value)));
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Utility {

    public class GeneralUtility
    {

        public static async Task DoDelayWork(float p_delay, System.Action p_action) {
            await Task.Delay(System.TimeSpan.FromSeconds(p_delay));

            if (p_action != null)
                p_action();
        }

        public static Vector2 PixelPerfectClamp(Vector2 moveVector, float pixelsPerUnit) {
            Vector2 vectorInPixels = new Vector2(
                Mathf.RoundToInt(moveVector.x * pixelsPerUnit),
                Mathf.RoundToInt(moveVector.y * pixelsPerUnit)
                );

            return vectorInPixels / pixelsPerUnit;
        }


    }
}

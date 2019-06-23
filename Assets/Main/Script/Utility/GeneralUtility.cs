﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TD.Database;
using TD.Unit; 

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

        public static float GetReachTimeGivenInfo(Vector3 targetPos, Vector3 selfPos, float bulletSpeed, float constDeltaTime) {
            float dist = Vector3.Distance(targetPos, selfPos);

            return dist / (bulletSpeed * constDeltaTime * 60);
        }

        public static GameDamageManager.DMGRegistry GetDMGRegisterCard(UnitInterface target, UnitStats unitStats, float fireTime, float timeToDst)
        {
            GameDamageManager.DMGRegistry damageRequest = new GameDamageManager.DMGRegistry();

            damageRequest.fireTime = fireTime;

            damageRequest.timeToDest = timeToDst;

            damageRequest.unitStats = unitStats;

            damageRequest.target = target;

            return damageRequest;
        }


    }
}

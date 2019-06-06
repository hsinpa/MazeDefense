using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;

namespace TD.Unit {
    public class GameDamageManager
    {
        private int requestBatchNum = 20;
        private List<DMGRegistry> dmgRegisterList = new List<DMGRegistry>();
        private int dmgRegisterLength = 0;

        #region Public Method
        public void AddRequest(DMGRegistry request) {
            dmgRegisterList.Add(request);
            dmgRegisterLength++;
        }

        public void OnUpdate()
        {
            float time = Time.time;
            int removeNum = 0;
            for (int i = 0; i < dmgRegisterLength; i++)
            {
                if (i >= requestBatchNum)
                    break;
                
                //If dmgRequest haven't reach its dmg time point, then break loop
                if (time < dmgRegisterList[i].timeToDest + dmgRegisterList[i].fireTime)
                    break;

                for (int m = 0; m < dmgRegisterList[i].targetNum; m++)
                {
                    var monster = dmgRegisterList[i].targets[m];
                    if (monster.isActive)
                        monster.OnAttack(dmgRegisterList[i].towerInfo.damage);
                }
                removeNum++;
            }

            if (removeNum > 0) {
                dmgRegisterLength -= removeNum;
                dmgRegisterList.RemoveRange(0, removeNum);
            }
        }

        public void Reset()
        {
            dmgRegisterLength = 0;
            dmgRegisterList.Clear();
        }
        #endregion

        #region Private Method

        #endregion
        public struct DMGRegistry {
            public STPTower towerInfo;
            public MonsterUnit[] targets;
            public int targetNum;

            public float fireTime;
            public float timeToDest;
        }
    }
}
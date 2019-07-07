using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using TD.Skill;
using TD.Map;

namespace TD.Unit {
    public class GameDamageManager
    {
        private int requestBatchNum = 20;
        private List<DMGRegistry> dmgRegisterList = new List<DMGRegistry>();
        private List<UnitInterface> possibleTargetList = new List<UnitInterface>();

        private int dmgRegisterLength = 0;
        private GameSkillMapper _gameSkillMapper;
        private MapBlockManager _mapBlockerManager;
        private MapGrid _mapGrid;

        #region Public Method
        public GameDamageManager(GameSkillMapper gameSkillMapper, MapBlockManager mapBlockManager, MapGrid mapGrid) {
            _gameSkillMapper = gameSkillMapper;
            _mapBlockerManager = mapBlockManager;
            _mapGrid = mapGrid;
        }

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


                List<UnitInterface> targetList = ApplySkillEffect(dmgRegisterList[i]);
                int targetListLength = targetList.Count;
                for (int targetIndex = 0; targetIndex < targetListLength; targetIndex++) {
                    //var monster = dmgRegisterList[i].target;
                    if (targetList[targetIndex] != null && targetList[targetIndex].isActive)
                        targetList[targetIndex].OnAttack(dmgRegisterList[i]);
                }

                if (dmgRegisterList[i].target != null) {
                    //var monster = dmgRegisterList[i].target;
                    //if (monster.isActive)
                    //    monster.OnAttack(dmgRegisterList[i].towerStats.atk);
                }

                removeNum++;
            }

            if (removeNum > 0) {
                dmgRegisterLength -= removeNum;
                dmgRegisterList.RemoveRange(0, removeNum);
            }
        }

        private List<UnitInterface> ApplySkillEffect(DMGRegistry dmgRegistry) {
            possibleTargetList.Clear();
            possibleTargetList.Add(dmgRegistry.target);

            int skillLength = dmgRegistry.unitStats.skills.Length;
            if (dmgRegistry.unitStats.skills != null && dmgRegistry.unitStats.skills.Length > 0) {
                for (int s = 0; s < skillLength; s++) {
                    BaseSkill skillWorker = _gameSkillMapper.GetSkill(dmgRegistry.unitStats.skills[s].id);
                    if (skillWorker != null) {

                        possibleTargetList.AddRange(skillWorker.Execute(dmgRegistry.unitStats.skills[s], possibleTargetList, dmgRegistry, _mapBlockerManager, _mapGrid));

                    }
                }
            }

            return possibleTargetList;
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
            public TD.Database.UnitStats unitStats;
            public UnitInterface target;
            public UnitInterface fromUnit;

            public float fireTime;
            public float timeToDest;
        }
    }
}
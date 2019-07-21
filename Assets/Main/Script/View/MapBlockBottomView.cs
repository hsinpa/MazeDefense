using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using TD.Map;

namespace TD.UI
{
    public class MapBlockBottomView : InGameUIBase
    {

        #region Inspector Parameter;

        [SerializeField]
        private Animator mapBlockAnimator;

        [SerializeField]
        private Button mapBlockExpandBt;

        [SerializeField]
        private Button[] mapBlockViewBts;

        [SerializeField]
        private BlockComponent testPrefabBlockComponent;

        private MapBlockManager _mapBlockManager;
        #endregion

        void FindReference() {
            if (mapBlockAnimator == null)
                mapBlockAnimator = transform.Find("MapBlock").GetComponent<Animator>();

            if (mapBlockExpandBt == null)
                mapBlockExpandBt = transform.Find("MapBlock/expand_icon").GetComponent<Button>();

            if (mapBlockViewBts == null || (mapBlockViewBts != null && mapBlockViewBts.Length <= 0))
                mapBlockViewBts = transform.Find("MapBlock/MapBlockItems").GetComponentsInChildren<Button>();
        }

        public void SetUp(MapBlockManager mapBlockManager) {
            _mapBlockManager = mapBlockManager;

            RegisterUIEvent();
        }

        private void RegisterUIEvent() {
            if (mapBlockExpandBt != null) {
                mapBlockExpandBt.onClick.RemoveAllListeners();
                mapBlockExpandBt.onClick.AddListener(() =>
                {
                    if (mapBlockAnimator != null)
                        mapBlockAnimator.SetBool(VariableFlag.UIAnimator.Expand, !mapBlockAnimator.GetBool(VariableFlag.UIAnimator.Expand) );
                });
            }

            if (mapBlockViewBts != null && mapBlockViewBts.Length > 0) {
                for (int i = 0; i < mapBlockViewBts.Length; i++) {

                    mapBlockViewBts[i].onClick.RemoveAllListeners();
                    mapBlockViewBts[i].onClick.AddListener(() =>
                    {
                        if (_mapBlockManager != null && testPrefabBlockComponent != null) {
                            var prefabBlockComponent = Instantiate(testPrefabBlockComponent);
                            _mapBlockManager.InsertNewMapComponent(prefabBlockComponent);
                            mapBlockAnimator.SetBool(VariableFlag.UIAnimator.Expand, false);

                            GameInputManager.instance.AppendIgnoreTime();
                        }
                    });
                }
            }

        }

        private void OnValidate()
        {
            FindReference();
        }
    }
}
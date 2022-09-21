using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TD.UI
{
    public class HeaderView : InGameUIBase
    {
        private PlayerModel mainPlayer;

        [SerializeField]
        private TMPro.TextMeshProUGUI capital_text;

        public void SetUp(PlayerModel mainPlayer)
        {
            this.mainPlayer = mainPlayer;

            if (mainPlayer != null) {
                capital_text.text = mainPlayer.capital.ToString();

                mainPlayer.OnCapitalChange += OnPlayerCapitalChange;
            }
        }

        private void Start()
        {
            base.Init();
        }

        private void OnPlayerCapitalChange(int capital_value)
        {
            if (capital_text != null)
                capital_text.text = capital_value.ToString();
        }

        private void OnDestroy()
        {
            if (this.mainPlayer != null)
                this.mainPlayer.OnCapitalChange -= OnPlayerCapitalChange;
        }
    }
}
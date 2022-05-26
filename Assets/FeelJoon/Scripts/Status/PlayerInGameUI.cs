using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;
using UnityEngine.UI;

namespace ETeam.FeelJoon
{
    public class PlayerInGameUI : MonoBehaviour
    {
        #region Variables
        public StatsObject playerStats;

        public Text levelText;
        public Slider healthSlider;
        public Slider manaSlider;

        #endregion Variables

        void Start()
        {
            levelText.text = playerStats.level.ToString("n0");

            healthSlider.value = playerStats.HealthPercentage;
            manaSlider.value = playerStats.ManaPercentage;
        }

        void OnEnable()
        {
            playerStats.OnChangedStats += OnChangedStats;
        }

        void OnDisable()
        {
            playerStats.OnChangedStats -= OnChangedStats;
        }

        private void OnChangedStats(StatsObject statsObject)
        {
            levelText.text = statsObject.level.ToString("n0");

            healthSlider.value = statsObject.HealthPercentage;
            manaSlider.value = statsObject.ManaPercentage;
        }
    }
}
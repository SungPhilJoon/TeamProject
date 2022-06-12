using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityChanAdventure.FeelJoon
{
    public class NPCBattleUI : MonoBehaviour
    {
        #region Variables
        private Slider hpSlider;
        [SerializeField] private GameObject damageTextPrefab;

        #endregion Variables

        #region Properties
        public float MaximumValue
        {
            get => hpSlider.maxValue;

            set => hpSlider.maxValue = value;
        }

        public float MinimumValue
        {
            get => hpSlider.minValue;

            set => hpSlider.minValue = value;
        }

        public float Value
        {
            get => hpSlider.value;

            set => hpSlider.value = value;
        }

        #endregion Properties

        #region Unity Methods
        void Awake()
        {
            hpSlider = GetComponentInChildren<Slider>();
        }

        void OnEnable()
        {
            GetComponent<Canvas>().enabled = true;
        }

        void OnDisable()
        {
            GetComponent<Canvas>().enabled = false;
        }

        #endregion Unity Methods

        #region Helper Methods
        public void CreateDamageText(int damage)
        {
            if (damageTextPrefab != null)
            {
                GameObject damageTextGO = Instantiate(damageTextPrefab, transform);
                DamageText damageText = damageTextGO.GetComponent<DamageText>();
                if (damageText == null)
                {
                    Destroy(damageTextGO);
                }

                damageText.Damage = damage;
            }
        }

        #endregion Helper Methods
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityChanAdventure.FeelJoon
{
    public class DamageText : MonoBehaviour
    {
        #region Variables
        public float delayTimeToDestroy = 1.0f;
        private int damage = 0;
        private TextMeshProUGUI text;

        #endregion Variables

        #region Properties
        public int Damage
        {
            get => damage;

            set
            {
                damage = value;
                text.text = damage.ToString();
            }
        }

        #endregion Properties

        #region Unity Methods
        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            Destroy(this.gameObject, delayTimeToDestroy);
        }

        #endregion Unity Methods
    }
}
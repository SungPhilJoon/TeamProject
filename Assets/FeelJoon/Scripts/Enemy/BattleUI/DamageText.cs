using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETeam.FeelJoon
{
    public class DamageText : MonoBehaviour
    {
        #region Variables
        public float delayTimeToDestroy = 1.0f;
        private int damage;
        private Text text;

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
        void Start()
        {
            Destroy(this.gameObject, delayTimeToDestroy);
            text = GetComponent<Text>();
        }

        #endregion Unity Methods
    }
}
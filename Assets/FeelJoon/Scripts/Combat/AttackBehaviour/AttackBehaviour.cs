using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public abstract class AttackBehaviour : MonoBehaviour
    {
        #region Variables
    
    #if UNITY_EDITOR
        [Multiline]
        public string developmentDescription = "";
    
    #endif
    
        public int animationIndex;
    
        public int priority;
    
        public int damage = 10;
        public float range = 3f;
    
        [SerializeField] protected float coolTime;
        protected float calcCoolTime = 0.0f;
    
        public GameObject effectPrefab;
    
        [HideInInspector]
        public LayerMask targetMAsk;
    
        [SerializeField] public bool IsAvailable => calcCoolTime >= coolTime;
    
        #endregion Variables
    
        #region Unity Methods
        void Start()
        {
            calcCoolTime = coolTime;
        }
    
        void Update()
        {
            if (calcCoolTime < coolTime)
            {
                calcCoolTime += Time.deltaTime;
            }
        }
    
        #endregion Unity Methods
    
        #region Helper Methods
        public abstract void ExecuteAttack(GameObject target = null, Transform startPosition = null);
    
        #endregion Helper Methods
    }
}

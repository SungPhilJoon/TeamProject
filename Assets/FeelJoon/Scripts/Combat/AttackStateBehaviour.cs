using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public class AttackStateBehaviour : MonoBehaviour
    {
        #region delegates
        public delegate void OnEnterAttackState();
        public delegate void OnExitAttackState();
    
        public OnEnterAttackState enterAttackStateHandler;
        public OnExitAttackState exitAttackStateHandler;
    
        #endregion delegates
    
        #region Properties
        public bool IsInAttackState
        {
            get;
            private set;
        }
    
        #endregion Properties
    
        #region Unity Methods
        void Start()
        {
            enterAttackStateHandler = new OnEnterAttackState(EnterAttackState);
            exitAttackStateHandler = new OnExitAttackState(ExitAttackState);
        }
    
        #endregion Unity Methods
    
        #region Helper Methods
        public void OnStartOfAttackState()
        {
            IsInAttackState = true;
            enterAttackStateHandler();
        }
    
        public void OnEndOfAttackState()
        {
            IsInAttackState = false;
            exitAttackStateHandler();
        }
    
        public void EnterAttackState()
        {
    
        }
    
        public void ExitAttackState()
        {
    
        }
    
        public void OnCheckAttackCollider(int attackIndex)
        {
            GetComponent<IAttackable>)()?.OnExecuteAttack(attackIndex);
        }
    
        #endregion Helper Methods
    }
}

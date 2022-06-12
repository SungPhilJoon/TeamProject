using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class EnemyDeadState : State<EnemyController>
    {
        #region Variables
        private Animator animator;

        protected readonly int hashAlive = Animator.StringToHash("IsAlive");
        protected readonly int hashOnDead = Animator.StringToHash("OnDead");

        #endregion Variables

        #region State
        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(hashAlive, context.IsAlive);
            animator?.SetTrigger(hashOnDead);
        }

        public override void Update(float deltaTime)
        {
            if (stateMachine.ElapsedTimeInState > 3.0f)
            {
                context.gameObject.SetActive(false);
            }
        }

        public override void OnExit()
        {
            
        }

        #endregion State
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public class EnemyDeadState : State<EnemyController>
    {
        #region Variables
        private Animator animator;

        protected readonly int hashAlive = Animator.StringToHash("Alive");

        #endregion Variables

        #region State
        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            //animator?.SetBool(hashAlive, context.IsAlive);
        }

        public override void OnEnter()
        {
            animator?.SetBool(hashAlive, context.IsAlive);
        }

        public override void Update(float deltaTime)
        {
            if (stateMachine.ElapsedTimeInState > 3.0f)
            {
                GameObject.Destroy(context.gameObject);
            }
        }

        public override void OnExit()
        {
            
        }

        #endregion State
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public class EnemyAttackState : State<EnemyController>
    {
        #region Variables
        private Animator animator;
        private IAttackable attackable;

        #endregion Variables

        #region State
        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            attackable = context.GetComponent<IAttackable>();
        }

        public override void OnEnter()
        {
            if (attackable == null)
            {
                stateMachine.ChangeState<EnemyIdleState>();
                return;
            }


        }

        public override void Update(float deltaTime)
        {
            
        }

        public override void OnExit()
        {
            stateMachine.ChangeState<EnemyIdleState>();
        }

        #endregion State
    }
}
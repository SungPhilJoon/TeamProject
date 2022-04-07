using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public class EnemyIdleState : State<EnemyController>
    {
        #region Variables
        private Animator animator;

        #endregion Variables

        #region State
        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();

        }

        public override void OnEnter()
        {
            
        }

        public override void Update(float deltaTime)
        {
            Transform enemy = context.SearchEnemy();
            if (enemy == null)
            {
                return;
            }

            if (context.IsAvailableAttack)
            {
                stateMachine.ChangeState<EnemyAttackState>();
            }
            else
            {
                stateMachine.ChangeState<EnemyMoveState>();
            }
        }

        public override void OnExit()
        {
            
        }

        #endregion State
    }
}
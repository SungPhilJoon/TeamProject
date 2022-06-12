using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class EnemyIdleState : State<EnemyController>
    {
        #region Variables
        private Animator animator;

        protected readonly int hashIsMove = Animator.StringToHash("IsMove");
        protected readonly int hashAttackIndex = Animator.StringToHash("AttackIndex");
        protected readonly int hashAttack = Animator.StringToHash("Attack");

        #endregion Variables

        #region State
        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            animator.SetBool(hashIsMove, false);
        }

        public override void Update(float deltaTime)
        {
            Transform enemy = context.Target;
            if (enemy == null || !enemy.GetComponent<IDamageable>().IsAlive)
            {
                return;
            }

            if (context.IsAvailableAttack)
            {
                animator.SetTrigger(hashAttack);
                animator.SetInteger(hashAttackIndex, Random.Range(0, 2));
                stateMachine.ChangeState<EnemyAttackState>();
            }
            else
            {
                animator.SetBool(hashIsMove, true);
                stateMachine.ChangeState<EnemyMoveState>();
            }
        }

        public override void OnExit()
        {
            
        }

        #endregion State
    }
}
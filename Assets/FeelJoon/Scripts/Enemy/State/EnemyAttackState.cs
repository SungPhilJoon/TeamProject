using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class EnemyAttackState : State<EnemyController>
    {
        #region Variables
        private Animator animator;
        private IAttackable attackable;

        private Transform target;

        protected readonly int hashAttackIndex = Animator.StringToHash("AttackIndex");
        protected readonly int hashAttack = Animator.StringToHash("Attack");

        #endregion Variables

        #region State
        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            attackable = context.GetComponent<IAttackable>();
        }

        public override void OnEnter()
        {
            context.transform.LookAt(context.Target);

            if (attackable == null)
            {
                stateMachine.ChangeState<EnemyIdleState>();
                return;
            }

            target = context.Target;
            if (target == null)
            {
                stateMachine.ChangeState<EnemyIdleState>();
                return;
            }

            if (context.enemyType == EnemyType.Melee)
            {
                animator.SetInteger(hashAttackIndex, Random.Range(0, 2));
            }
            animator.SetTrigger(hashAttack);
        }

        public override void Update(float deltaTime)
        {
            if (stateMachine.ElapsedTimeInState > context.coolTime)
            {
                stateMachine.ChangeState<EnemyIdleState>();
            }
        }

        public override void OnExit()
        {
        }

        #endregion State
    }
}
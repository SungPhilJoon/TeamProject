using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityChanAdventure.FeelJoon
{
    public class EnemyMoveState : State<EnemyController>
    {
        #region Variables
        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        protected readonly int hashIsMove = Animator.StringToHash("IsMove");

        #endregion Variables

        #region State
        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
            agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            agent?.SetDestination(context.Target.position);
        }

        public override void Update(float deltaTime)
        {
            Transform enemy = context.Target;
            if (enemy == null)
            {
                stateMachine.ChangeState<EnemyIdleState>();
                return;
            }

            agent.SetDestination(context.Target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                controller.Move(agent.velocity * Time.deltaTime);
                return;
            }

            animator.SetBool(hashIsMove, false);
            stateMachine.ChangeState<EnemyIdleState>();
        }

        public override void OnExit()
        {
            agent.ResetPath();
        }

        #endregion State
    }
}
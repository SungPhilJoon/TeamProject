using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ETeam.FeelJoon
{
    public class EnemyMoveState : State<EnemyController>
    {
        #region Variables
        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

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
            Transform enemy = context.SearchEnemy();
            if (enemy == null)
            {
                return;
            }

            agent.SetDestination(context.Target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                controller.Move(agent.velocity * Time.deltaTime);
                return;
            }

            stateMachine.ChangeState<EnemyIdleState>();
        }

        public override void OnExit()
        {
            agent.ResetPath();
        }

        #endregion State
    }
}
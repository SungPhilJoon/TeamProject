using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public class EnemyHitState : State<EnemyController>
    {
        #region Variables
        private CharacterController controller;
        private Animator animator;

        public readonly int hashTrigger = Animator.StringToHash("HitTrigger");

        #endregion Variables

        #region State
        public override void OnInitialized()
        {
            controller = context.GetComponent<CharacterController>();
            animator = context.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            controller.Move(Vector3.zero);

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
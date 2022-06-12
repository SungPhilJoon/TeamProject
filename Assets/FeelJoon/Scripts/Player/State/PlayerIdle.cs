using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class PlayerIdle : State<PlayerController>
    {
        #region Variables
        private Animator animator;
        private CharacterController controller;

        protected readonly int hashIsMove = Animator.StringToHash("IsMove");

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponentInChildren<Animator>();
            controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(hashIsMove, context.IsMove);
            controller?.Move(Vector3.zero);
        }

        public override void Update(float deltaTime)
        {
            if (context.IsMove)
            {
                stateMachine.ChangeState<PlayerMove>();
            }
        }

        public override void OnExit()
        {
            animator?.SetBool(hashIsMove, context.IsMove);
        }
    }
}
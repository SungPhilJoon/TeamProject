using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public class PlayerMove : State<PlayerController>
    {
        #region Variables
        private Animator animator;
        private CharacterController controller;

        private readonly int hashIsMove = Animator.StringToHash("IsMove");
        private readonly int hashDashTrigger = Animator.StringToHash("DashTrigger");

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponentInChildren<Animator>();
            controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator.SetBool(hashIsMove, context.IsMove);
        }

        public override void Update(float deltaTime)
        {
            
        }

        public override void OnExit()
        {

        }
    }
}

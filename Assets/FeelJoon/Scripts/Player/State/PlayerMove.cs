using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class PlayerMove : State<PlayerController>
    {
        #region Variables
        private Animator animator;

        private readonly int hashIsMove = Animator.StringToHash("IsMove");

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponentInChildren<Animator>();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ETeam.FeelJoon
{
    public class PlayerAttack : State<PlayerController>
    {
        #region Variables
        private Animator animator;
        private PlayerStance playerStance;

        protected readonly int hashOnNormalAttack = Animator.StringToHash("OnNormalAttack");
        protected readonly int hashIsComboAttack = Animator.StringToHash("IsComboAttack");

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponentInChildren<Animator>();
            playerStance = context.playerStance;
        }

        public override void OnEnter()
        {
            animator.SetTrigger(hashOnNormalAttack);
        }

        public override void Update(float deltaTime)
        {
            if (playerStance == PlayerStance.Sword)
            {
                
            }
            else if (playerStance == PlayerStance.Bow)
            {

            }
            else
            {
                
            }
        }

        public override void OnExit()
        {
            
        }
    }
}

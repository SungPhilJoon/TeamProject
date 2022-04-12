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
        private AttackStateController attackStateController;

        protected readonly int hashOnNormalAttack = Animator.StringToHash("OnNormalAttack");
        protected readonly int hashIsComboAttack = Animator.StringToHash("IsComboAttack");

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponentInChildren<Animator>();
            attackStateController = context.GetComponent<AttackStateController>();
        }

        public override void OnEnter()
        {
            playerStance = context.playerStance;

            switch (playerStance)
            {
                case PlayerStance.Sword:
                    attackStateController.OnEnterSwordAttackStateHandler();
                    break;
                case PlayerStance.Bow:
                    attackStateController.OnEnterBowAttackStateHandler();
                    break;
            }
        }

        public override void Update(float deltaTime)
        {
            
        }

        public override void OnExit()
        {
            switch (playerStance)
            {
                case PlayerStance.Sword:
                    attackStateController.OnExitSwordAttackStateHandler();
                    break;
                case PlayerStance.Bow:
                    attackStateController.OnExitBowAttackStateHandler();
                    break;
            }
        }
    }
}

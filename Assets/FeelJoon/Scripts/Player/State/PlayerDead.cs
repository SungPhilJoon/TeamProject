using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityChanAdventure.FeelJoon
{
    public class PlayerDead : State<PlayerController>
    {
        #region Variables
        private Animator animator;
        private CharacterController controller;
        private PlayerInput playerInput;

        private InputActionMap previousActionMap;

        private readonly int hashIsAlive = Animator.StringToHash("IsAlive");
        private readonly int hashOnDeadTrigger = Animator.StringToHash("OnDeadTrigger");

        private readonly string reviveMessage = "초 후에\n마을에서\n부활합니다.";

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponentInChildren<Animator>();
            controller = context.GetComponent<CharacterController>();
            playerInput = context.GetComponent<PlayerInput>();
        }

        public override void OnEnter()
        {
            previousActionMap = playerInput.currentActionMap;
            playerInput.SwitchCurrentActionMap("PlayerDead");

            animator.SetBool(hashIsAlive, context.IsAlive);
            animator.SetTrigger(hashOnDeadTrigger);
        }

        public override void Update(float deltaTime)
        {
            float reviveCount = 8 - stateMachine.ElapsedTimeInState;
            string countReviveMessage = reviveCount.ToString("n0") + reviveMessage;
            context.gameoverText.text = countReviveMessage;

            if (stateMachine.ElapsedTimeInState > 8.0f)
            {
                playerInput.currentActionMap = previousActionMap;
                GameManager.Instance.PlayerRespawn();
            }
        }

        public override void OnExit()
        {
            
        }
    }
}

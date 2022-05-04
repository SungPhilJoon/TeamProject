using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ETeam.FeelJoon
{
    public class PlayerDead : State<PlayerController>
    {
        #region Variables
        private Animator animator;
        private CharacterController controller;
        private PlayerInput playerInput;

        private InputActionMap currentActionMap;

        private readonly int hashIsAlive = Animator.StringToHash("IsAlive");
        private readonly int hashOnDeadTrigger = Animator.StringToHash("OnDeadTrigger");

        #endregion Variables

        public override void OnInitialized()
        {
            animator = context.GetComponentInChildren<Animator>();
            controller = context.GetComponent<CharacterController>();
            playerInput = context.GetComponent<PlayerInput>();
        }

        public override void OnEnter()
        {
            currentActionMap = playerInput.currentActionMap;
            playerInput.SwitchCurrentActionMap("PlayerDead");

            Debug.Log("히히 죽엇땅");

            animator.SetBool(hashIsAlive, context.IsAlive);
            animator.SetTrigger(hashOnDeadTrigger);
        }

        public override void Update(float deltaTime)
        {
            int reviveCount = Mathf.RoundToInt(5 - stateMachine.ElapsedTimeInState);
            context.gameoverText.text = $"{reviveCount}초 후에\n자동으로\n부활합니다.";

            if (stateMachine.ElapsedTimeInState > 5.0f)
            {
                Debug.Log("개같이 부활!");
                playerInput.currentActionMap = currentActionMap;
                GameManager.Instance.Revive();

                // context.gameObject.SetActive(false);
                // GameObject.Destroy(context.gameObject);
            }
        }

        public override void OnExit()
        {
            
        }
    }
}

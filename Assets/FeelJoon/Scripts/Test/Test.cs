using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityChanAdventure.FeelJoon
{
    [RequireComponent(typeof(CharacterController))]
    public class Test : MonoBehaviour
    {
        #region Variables
        private PlayerInput playerInput;
        private CharacterController controller;

        private Vector3 calcVelocity;

        public float moveSpeed;

        #endregion Variables

        #region Properties


        #endregion Properties

        #region Unity Methods
        void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            playerInput.actions["SwitchAttackAction"].performed += SwitchActionMap;
            playerInput.actions["Move"].performed += Move;
            playerInput.actions["Move"].canceled += Stop;
        }

        void OnDisable()
        {
            playerInput.actions["SwitchAttackAction"].performed -= SwitchActionMap;
            playerInput.actions["Move"].performed -= Move;
            playerInput.actions["Move"].canceled -= Stop;
        }

        void Update()
        {
            // Debug.Log(playerInput.currentActionMap.ToString());
            controller.Move(calcVelocity * Time.deltaTime * moveSpeed);
        }

        #endregion Unity Methods

        #region Helper Methods
        private void SwitchActionMap(InputAction.CallbackContext callbackContext)
        {
            playerInput.SwitchCurrentActionMap("SwordStand");
        }

        private void Move(InputAction.CallbackContext callbackContext)
        {
            calcVelocity = new Vector3(callbackContext.ReadValue<Vector2>().x,
                0,
                callbackContext.ReadValue<Vector2>().y);
        }

        private void Stop(InputAction.CallbackContext callbackContext)
        {
            calcVelocity = Vector3.zero;
        }

        #endregion Helper Methods
    }
}

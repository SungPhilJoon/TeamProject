using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ETeam.KyungSeo
{
    public class TestPlayerController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float moveSpeed;

        private Vector2 inputValue = Vector2.zero;
        private float inputZ; // z축으로 앞 뒤로 움직인다
        private float inputX; // x축으로 왼쪽 오른쪽으로 움직인다.

        #endregion

        #region Properties

        // 여기에 프로퍼티를 선언합니다.

        #endregion

        #region Unity Methods

        private void Update()
        {
            transform.Translate(inputValue * moveSpeed * Time.deltaTime);
        }

        #endregion

        #region Helper Methods

        public void Move(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                inputValue = callbackContext.ReadValue<Vector2>();
                inputZ = inputValue.x;
                inputX = inputValue.y;
            }

            if (callbackContext.canceled)
            {
                inputValue = Vector2.zero;
            }
        }
        

        #endregion

        /*
         여기에 주석을 작성합니다.
         */
    }
}
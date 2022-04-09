using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ETeam.FeelJoon;

namespace ETeam.KyungSeo
{
    [RequireComponent(typeof(CharacterController))]
    public partial class TestPlayerController : PlayerController
    {
        #region Variables
        [SerializeField] private float moveSpeed; // 이동 스피드
        [SerializeField] private float dashDistance = 5.0f; // 대쉬 거리 - PJ

        [SerializeField] private Image testUI; // 테스트로 UI(설정창)를 띄우고 끄게 해 보려고

        private Vector2 inputValue = Vector2.zero; // 입력 Vector
        private Vector3 movement = Vector3.zero; // 이동 방향 Vector
        private bool isUIOn = false; // 테스트 UI표시용

        private CharacterController controller; // 캐싱할 CharacterController - PJ

        public float gravity = -29.81f; // 중력 계수 - PJ // KS 상세 : rigidbody를 사용하지 않는 중력계수라고 하네요~
        public Vector3 drags; // 저항력 -PJ

        private bool isGround = false;

        private Vector3 calcVelocity; // 계산에 사용될 Vector3 레퍼런스 - PJ

        public PlayerInput playerInput;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();

            playerInput.SwitchCurrentActionMap("Default");
        }

        protected override void Update()
        {
            base.Update();
            isGround = controller.isGrounded;
            if (isGround && calcVelocity.y < 0)
            {
                calcVelocity.y = 0;
            }

            controller.Move(movement * Time.deltaTime * moveSpeed);
            if (movement != Vector3.zero)
            {
                transform.forward = movement;
            }

            calcVelocity.y += gravity * Time.deltaTime;

            calcVelocity.x /= 1 + drags.x * Time.deltaTime;
            calcVelocity.y /= 1 + drags.y * Time.deltaTime;
            calcVelocity.z /= 1 + drags.z * Time.deltaTime;
            
            controller.Move(calcVelocity * Time.deltaTime);
            
            // controller.Move(movement * Time.deltaTime * moveSpeed);
            // if (movement != Vector3.zero)
            // {
            //     transform.forward = movement;
            // }

            
            
            // // 업데이트 말고 다른곳에서 할 방법은?
            // transform.Translate(movement * moveSpeed * Time.deltaTime);
        }

        #endregion

        #region Input Methods : Movements

        public void Move(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed) // 키 누르고있으면 이동하도록 value 읽기
            {
                inputValue = callbackContext.ReadValue<Vector2>();
                movement.x = inputValue.x;
                movement.z = inputValue.y;
                isMove = true;
                stateMachine.ChangeState<PlayerMove>();
            }

            if (callbackContext.canceled) // 키를 떼면 정지
            {
                movement = Vector2.zero;
                isMove = false;
                stateMachine.ChangeState<PlayerIdle>();
            }

            // animator.SetBool(hashIsMove, movement.sqrMagnitude > 0);
        }

        public void Dash(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                Vector3 dashVelocity = Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1 / (drags.x * Time.deltaTime + 1)) / -Time.deltaTime),
                    0,
                    (Mathf.Log(1 / (drags.z * Time.deltaTime + 1)) / -Time.deltaTime)));
                calcVelocity += dashVelocity;
                stateMachine.ChangeState<PlayerDash>();
            }

            // 이동방향으로.. 밀어버리나? 개너무한 부분;
        }

        #endregion

        public void Interact()
        {
            // Interact가 가능한 오브젝트면
            // interact
        }

        #region Input Methods : Swap

        public void SwapToBow(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                // 무기 스왑 애니메이션
                animator.SetInteger(hashSwapIndex, 2);
                playerInput.SwitchCurrentActionMap("PlayerBow");
            }
        }
        
        public void SwapToSword(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                // 무기 스왑 애니메이션
                animator.SetInteger(hashSwapIndex, 1);
                playerInput.SwitchCurrentActionMap("PlayerSword");
            }
        }

        public void SwapDefault(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                animator.SetInteger(hashSwapIndex, 0);
                playerInput.SwitchCurrentActionMap("Default");
            }
        }

        #endregion

        #region Input Methods : Call UIs

        public void CallSettings(InputAction.CallbackContext callbackContext)
        {
            // 이게 맞나? 어쨌든 테스트 -> 이건 플레이어 말고 게임매니저 같은곳에 넣어야 하나?
            if (!isUIOn)
            {
                isUIOn = true;
                testUI.gameObject.SetActive(true);
            }
            else
            {
                isUIOn = false;
                testUI.gameObject.SetActive(false);
            }
        }

        public void CallInventory(InputAction.CallbackContext callbackContext)
        {

        }

        public void CallEquipment(InputAction.CallbackContext callbackContext)
        {

        }

        #endregion

        #region Helper Methods
        // 여기에 사용자 정의 함수를 선언합니다.
        #endregion
    }
}
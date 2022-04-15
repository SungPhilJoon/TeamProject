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

        [SerializeField] private Image settingsUI; // 테스트로 UI(설정창)를 띄우고 끄게 해 보려고
        [SerializeField] private Image inventoryUI;
        [SerializeField] private Image equipmentUI;

        private Vector2 inputValue = Vector2.zero; // 입력 Vector
        private Vector3 movement = Vector3.zero; // 이동 방향 Vector

        public bool isSettingOn = false; // 테스트 UI표시용
        public bool isInventoryOn = false;
        public bool isEquipmentOn = false;

        public GameObject weaponErrorText;

        private CharacterController controller; // 캐싱할 CharacterController - PJ

        public float gravity = -29.81f; // 중력 계수 - PJ // KS 상세 : rigidbody를 사용하지 않는 중력계수라고 하네요~
        public Vector3 drags; // 저항력 -PJ

        private bool isGround = false;

        private Vector3 calcVelocity; // 계산에 사용될 Vector3 레퍼런스 - PJ

        public PlayerInput playerInput;

        [Header("전투")]
        public AttackStateController attackStateController;
        [SerializeField] private GameObject defaultWeaponPrefab;
        private GameObject previousWeapon;
        private GameObject equipmentWeapon;

        // 인벤토리
        [Header("인벤토리")]
        public InventoryObject inventory;

        #endregion Variables

        #region Properties
        public override int Damage
        {
            get => damage;
            protected set => damage = value;
        }

        #endregion Properties

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            controller = GetComponent<CharacterController>();
            playerInput = GetComponent<PlayerInput>();
            attackStateController = GetComponent<AttackStateController>();
            objectPoolManager = new ObjectPoolManager<Arrow>(PooledObjectNameList.NameOfArrow, spawnPoint);

            playerInput.SwitchCurrentActionMap("Default");

            equipmentWeapon = defaultWeaponPrefab;
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
                transform.forward = movement; // Vector3.Lerp(transform.forward, new Vector3(focus.forward.x, 0f, focus.forward.z), 10f * Time.deltaTime);
            }

            calcVelocity.y += gravity * Time.deltaTime;

            calcVelocity.x /= 1 + drags.x * Time.deltaTime;
            calcVelocity.y /= 1 + drags.y * Time.deltaTime;
            calcVelocity.z /= 1 + drags.z * Time.deltaTime;

            controller.Move(calcVelocity * Time.deltaTime);
        }

        #endregion

        #region Helper Methods
        private void SetTarget(out Transform newTarget, LayerMask targetMask, float distance = 3.0f)
        {
            Collider[] targetColliders = Physics.OverlapSphere(transform.position, distance, targetMask);

            foreach (Collider targetCollider in targetColliders)
            {
                IInteractable interactable = targetCollider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    newTarget = targetCollider.transform;
                    return;
                }
            }

            newTarget = null;
        }

        #endregion Helper Methods

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

        #region Input Methods : Attack
        public void Attack(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                switch (playerStance)
                {
                    case PlayerStance.Sword:
                        AttackStanceToUsedEnter(true, EnterNormalSwordAttack, EnterNormalComboAttack);
                        AttackStanceToUsedExit(true, ExitNormalComboAttack);
                        break;
                    case PlayerStance.Bow:
                        AttackStanceToUsedEnter(true, EnterNormalBowAttack);
                        AttackStanceToUsedExit(true, ExitNormalBowAttack);
                        break;
                }
                stateMachine.ChangeState<PlayerAttack>();
            }
            else if (callbackContext.canceled)
            {
                stateMachine.ChangeState<PlayerIdle>();
                switch (playerStance)
                {
                    case PlayerStance.Sword:
                        AttackStanceToUsedEnter(false, EnterNormalSwordAttack, EnterNormalComboAttack);
                        AttackStanceToUsedExit(false, ExitNormalComboAttack);
                        break;
                    case PlayerStance.Bow:
                        AttackStanceToUsedEnter(false, EnterNormalBowAttack);
                        AttackStanceToUsedExit(false, ExitNormalBowAttack);
                        break;
                }
            }
        }

        public void Skill1(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                switch (playerStance)
                {
                    case PlayerStance.Sword:
                        AttackStanceToUsedEnter(true, EnterSkillSwordAttack);
                        AttackStanceToUsedExit(true, ExitSkillSwordAttack);
                        break;
                    case PlayerStance.Bow:
                        AttackStanceToUsedEnter(true, EnterSkillBowAttack);
                        AttackStanceToUsedExit(true, ExitSkillBowAttack);
                        break;
                }
                stateMachine.ChangeState<PlayerAttack>();
            }
            else if (callbackContext.canceled)
            {
                stateMachine.ChangeState<PlayerIdle>();
                switch (playerStance)
                {
                    case PlayerStance.Sword:
                        AttackStanceToUsedEnter(false, EnterSkillSwordAttack);
                        AttackStanceToUsedExit(false, ExitSkillSwordAttack);
                        break;
                    case PlayerStance.Bow:
                        AttackStanceToUsedEnter(false, EnterSkillBowAttack);
                        AttackStanceToUsedExit(false, ExitSkillBowAttack);
                        break;
                }
            }
        }

        #endregion Input Methods : Attack

        public void Interact(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                SetTarget(out target, targetMask);

                if (target != null)
                {
                    IInteractable interactable = target.GetComponent<IInteractable>();
                    interactable?.Interact(this.gameObject);
                    target = null;
                }
            }
        }

        #region Input Methods : Swap

        public void SwapToBow(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (bowPrefab != null)
                {
                    playerStance = PlayerStance.Bow;
                    animator.SetInteger(hashSwapIndex, (int)playerStance);
                    playerInput.SwitchCurrentActionMap("PlayerBow");
                }

                SwapWeapon(bowPrefab);
            }
        }

        public void SwapToSword(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                if (swordPrefab != null)
                {
                    playerStance = PlayerStance.Sword;
                    animator.SetInteger(hashSwapIndex, (int)playerStance);
                    playerInput.SwitchCurrentActionMap("PlayerSword");
                }

                SwapWeapon(swordPrefab);
            }
        }

        public void SwapDefault(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                playerStance = PlayerStance.Default;
                animator.SetInteger(hashSwapIndex, (int)playerStance);
                playerInput.SwitchCurrentActionMap("Default");
                SwapWeapon(defaultWeaponPrefab);
            }
        }

        #endregion

        #region Input Methods : Call UIs

        public void CallSettings(InputAction.CallbackContext callbackContext)
        {
            // 이게 맞나? 어쨌든 테스트 -> 이건 플레이어 말고 게임매니저 같은곳에 넣어야 하나?
            if (!isSettingOn)
            {
                isSettingOn = true;
                settingsUI.gameObject.SetActive(true);
            }
            else
            {
                if (isInventoryOn)
                {
                    isInventoryOn = false;
                    inventoryUI.gameObject.SetActive(false);
                }
                if (isEquipmentOn)
                {
                    isEquipmentOn = false;
                    equipmentUI.gameObject.SetActive(false);
                }

                isSettingOn = false;
                settingsUI.gameObject.SetActive(false);
            }
        }

        public void CallInventory(InputAction.CallbackContext callbackContext)
        {
            if (!isInventoryOn)
            {
                isInventoryOn = true;
                inventoryUI.gameObject.SetActive(true);
            }
            else
            {
                isInventoryOn = false;
                inventoryUI.gameObject.SetActive(false);
            }
        }

        public void CallEquipment(InputAction.CallbackContext callbackContext)
        {
            if (!isEquipmentOn)
            {
                isEquipmentOn = true;
                equipmentUI.gameObject.SetActive(true);
            }
            else
            {
                isEquipmentOn = false;
                equipmentUI.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Helper Methods
        // 여기에 사용자 정의 함수를 선언합니다.
        public bool PickupItem(PickupItem pickupItem, int amount = 1)
        {
            if (pickupItem.itemObject != null && inventory.AddItem(new Item(pickupItem.itemObject), amount))
            {
                Destroy(pickupItem.gameObject);
                return true;
            }

            return false;
        }

        private void SwapWeapon(GameObject weaponToSwap)
        {
            try
            {
                previousWeapon = equipmentWeapon;
                if (weaponToSwap == null)
                {
                    throw new NullReferenceException();
                }
                equipmentWeapon = weaponToSwap;
            }
            catch(NullReferenceException e)
            {
                Debug.Log("장착된 무기가 없습니다.");

                weaponErrorText.SetActive(true);

                animator.SetInteger(hashSwapIndex, (int)playerStance);
            }
            finally
            {
                previousWeapon.SetActive(false);
                equipmentWeapon.SetActive(true);
            }
            //previousWeapon = equipmentWeapon;
            //equipmentWeapon = weaponToSwap;
        }

        /// <summary>
        /// AttackState가 들어오면 발생하는 이벤트들을 더하거나 뺄 수 있는 함수 입니다.
        /// </summary>
        /// <param name="isAdded">이벤트를 더할건지 뺄건지 결정하는 매개변수 입니다.</param>
        /// <param name="attackStates"></param>
        public void AttackStanceToUsedEnter(bool isAdded, params Action[] attackStates)
        {
            if (isAdded)
            {
                for (int i = 0; i < attackStates.Length; i++)
                {
                    attackStateController.OnEnterAttackStateHandler += attackStates[i];
                }
            }
            else
            {
                for (int i = 0; i < attackStates.Length; i++)
                {
                    attackStateController.OnEnterAttackStateHandler -= attackStates[i];
                }
            }
        }

        /// <summary>
        /// AttackState가 나가면 발생하는 이벤트들을 더하거나 뺄 수 있는 함수 입니다.
        /// </summary>
        /// <param name="isAdded">이벤트를 더할건지 뺄건지 결정하는 매개변수 입니다.</param>
        /// <param name="attackStates"></param>
        public void AttackStanceToUsedExit(bool isAdded, params Action[] attackStates)
        {
            if (isAdded)
            {
                for (int i = 0; i < attackStates.Length; i++)
                {
                    attackStateController.OnExitAttackStateHandler += attackStates[i];
                }
            }
            else
            {
                for (int i = 0; i < attackStates.Length; i++)
                {
                    attackStateController.OnExitAttackStateHandler -= attackStates[i];
                }
            }
        }

        #endregion
    }
}
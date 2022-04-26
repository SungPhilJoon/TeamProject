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
        public float moveSpeed; // 이동 스피드
        [SerializeField] private float dashDistance = 5.0f; // 대쉬 거리 - PJ

        [SerializeField] private Image settingsUI; // 테스트로 UI(설정창)를 띄우고 끄게 해 보려고
        [SerializeField] private Image inventoryUI; // 캐릭터 인벤토리
        [SerializeField] private Image equipmentUI; // 캐릭터 장비창

        private Vector2 inputValue = Vector2.zero; // 입력 Vector
        private Vector3 movement = Vector3.zero; // 이동 방향 Vector

        public Transform focus;
        private Camera camera;

        [HideInInspector] public bool isSettingOn = false; // 테스트 UI표시용
        [HideInInspector] public bool isInventoryOn = false;
        [HideInInspector] public bool isEquipmentOn = false;

        private CharacterController controller; // 캐싱할 CharacterController - PJ

        public float gravity = -29.81f; // 중력 계수 - PJ // KS 상세 : rigidbody를 사용하지 않는 중력계수라고 하네요~
        public Vector3 drags; // 저항력 -PJ

        private bool isGround = false;

        private Vector3 calcVelocity; // 계산에 사용될 Vector3 레퍼런스 - PJ

        public PlayerInput playerInput;

        public GameObject weaponErrorText;

        [Header("전투")]
        public AttackStateController attackStateController;
        [SerializeField] private GameObject defaultWeaponPrefab;
        private GameObject previousWeapon;
        private GameObject equipmentWeapon;

        // 인벤토리
        [Header("인벤토리")]
        public InventoryObject inventory;
        public InventoryObject equipment;
        private PlayerEquipment playerEquipment;

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
            camera = Camera.main;
            objectPoolManager = new ObjectPoolManager<Arrow>(PooledObjectNameList.NameOfArrow, spawnPoint);
            playerEquipment = GetComponent<PlayerEquipment>();

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

            if (isMove)
            {
                Vector3 lookForward = new Vector3(focus.forward.x, 0f, focus.forward.z).normalized;
                Vector3 lookRight = new Vector3(focus.right.x, 0f, focus.right.z).normalized;
                Vector3 moveDir = lookForward * movement.z + lookRight * movement.x;

                transform.forward = Vector3.Lerp(transform.forward, moveDir, 10f * Time.deltaTime);
                controller.Move(moveDir * Time.deltaTime * moveSpeed);
            }

            calcVelocity.y += gravity * Time.deltaTime;

            calcVelocity.x /= 1 + drags.x * Time.deltaTime;
            calcVelocity.y /= 1 + drags.y * Time.deltaTime;
            calcVelocity.z /= 1 + drags.z * Time.deltaTime;

            controller.Move(calcVelocity * Time.deltaTime);
        }

        #endregion
         
        #region Input Methods : Movements

        public void Move(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed) // 키 누르고있으면 이동하도록 value 읽기
            {
                inputValue = callbackContext.ReadValue<Vector2>();
                movement.x = inputValue.x;
                movement.y = 0f;
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
        }

        #endregion Input Methods : Movements

        #region Input Methods : Attack
        public void Attack(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                switch (playerWeapon)
                {
                    case PlayerWeapon.Sword:
                        AttackStanceToUsedEnter(true, EnterNormalSwordAttack, EnterNormalComboAttack);
                        AttackStanceToUsedExit(true, ExitNormalComboAttack);
                        break;
                    case PlayerWeapon.Bow:
                        AttackStanceToUsedEnter(true, EnterNormalBowAttack);
                        AttackStanceToUsedExit(true, ExitNormalBowAttack);
                        break;
                }
                stateMachine.ChangeState<PlayerAttack>();
            }
            else if (callbackContext.canceled)
            {
                stateMachine.ChangeState<PlayerIdle>();
                switch (playerWeapon)
                {
                    case PlayerWeapon.Sword:
                        AttackStanceToUsedEnter(false, EnterNormalSwordAttack, EnterNormalComboAttack);
                        AttackStanceToUsedExit(false, ExitNormalComboAttack);
                        break;
                    case PlayerWeapon.Bow:
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
                switch (playerWeapon)
                {
                    case PlayerWeapon.Sword:
                        AttackStanceToUsedEnter(true, EnterSkillSwordAttack);
                        AttackStanceToUsedExit(true, ExitSkillSwordAttack);
                        break;
                    case PlayerWeapon.Bow:
                        AttackStanceToUsedEnter(true, EnterSkillBowAttack);
                        AttackStanceToUsedExit(true, ExitSkillBowAttack);
                        break;
                }
                stateMachine.ChangeState<PlayerAttack>();
            }
            else if (callbackContext.canceled)
            {
                stateMachine.ChangeState<PlayerIdle>();
                switch (playerWeapon)
                {
                    case PlayerWeapon.Sword:
                        AttackStanceToUsedEnter(false, EnterSkillSwordAttack);
                        AttackStanceToUsedExit(false, ExitSkillSwordAttack);
                        break;
                    case PlayerWeapon.Bow:
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
                bowPrefab = playerEquipment.itemInstances[(int)ItemType.Bow].itemTransforms[0].gameObject;
                bowPrefab.SetActive(true);

                if (equipment.Slots[(int)ItemType.Bow].SlotItemObject == null)
                {
                    bowPrefab = null;
                }

                if (bowPrefab != null)
                {
                    playerWeapon = PlayerWeapon.Bow;
                    animator.SetInteger(hashSwapIndex, (int)playerWeapon);
                    playerInput.SwitchCurrentActionMap("PlayerBow");
                }

                SwapWeapon(bowPrefab);
            }
        }

        public void SwapToSword(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                swordPrefab = playerEquipment.itemInstances[(int)ItemType.Sword].itemTransforms[0].gameObject;
                swordPrefab.SetActive(true);
                manualCollision = swordPrefab.GetComponent<BoxCollider>();

                if (equipment.Slots[(int)ItemType.Sword].SlotItemObject == null)
                {
                    swordPrefab = null;
                }

                if (swordPrefab != null)
                {
                    playerWeapon = PlayerWeapon.Sword;
                    animator.SetInteger(hashSwapIndex, (int)playerWeapon);
                    playerInput.SwitchCurrentActionMap("PlayerSword");
                }

                SwapWeapon(swordPrefab);
            }
        }

        public void SwapDefault(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                playerWeapon = PlayerWeapon.Default;
                animator.SetInteger(hashSwapIndex, (int)playerWeapon);
                playerInput.SwitchCurrentActionMap("Default");
                SwapWeapon(defaultWeaponPrefab);
            }
        }

        #endregion

        #region Input Methods : Call UIs

        public void CallSettings(InputAction.CallbackContext callbackContext)
        {
            if (!isSettingOn)
            {
                isSettingOn = true;
                Time.timeScale = 0;
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
                Time.timeScale = 1;
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
        public bool PickupItem(PickupItem pickupItem, int amount = 1)
        {
            if (pickupItem.itemObject != null && inventory.AddItem(new Item(pickupItem.itemObject), amount))
            {
                Destroy(pickupItem.gameObject);
                return true;
            }

            return false;
        }

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

                animator.SetInteger(hashSwapIndex, (int)playerWeapon);
            }
            finally
            {
                previousWeapon.SetActive(false);
                equipmentWeapon.SetActive(true);
            }
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
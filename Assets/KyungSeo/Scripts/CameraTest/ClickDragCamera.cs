using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityChanAdventure.FeelJoon;

namespace UnityChanAdventure.KyungSeo
{
    [ExecuteInEditMode]
    public class ClickDragCamera : MonoBehaviour
    {
        #region Variables
        [HideInInspector]
        public bool isRightButton = false; // 마우스 우클릭 감지용 bool변수
        [HideInInspector]
        public bool isCameraTurn = false; // 컨트롤 키를 누른 상태에서 마우스 우클릭을 하여 드래그 한다면 캐릭터를 기준으로 회전할 수 있게 해주는 변수
        
        public float cameraSensitivy = 3;
        public Transform target = null;
        public Transform focus;

        public float distance;
        public float height;
        public float lerpPercent;

        public float rotateSpeed = 15f;
        
        [Header("에임모드 카메라 관련")] [SerializeField] private Image imageAim;
        [SerializeField] private float _defaultModeFOV = 60; // 기본모드에서의 카메라 FOV
        [SerializeField] private float _aimModeFOV = 30; // Aim모드에서의 카메라 FOV
        [SerializeField] private Transform aimFocus; 
        internal bool _isAimOn; // 에임모드 체크용

        #endregion

        #region Unity Methods

        private void Awake()
        {
            //imageAim.enabled = false;
            //_isAimOn = false;
        }

        private void Start()
        {
            StartCoroutine(MoveCamera());
            StartCoroutine(Zoom());
        }

        private void LateUpdate()
        {
            if (!target)
            {
                return;
            }
            //calcVector += new Vector3(0, (z * Time.deltaTime) * -1f, z * Time.deltaTime);

            if (_isAimOn)
            {
                transform.LookAt(aimFocus);
                return;
            }
            
            focus.position = Vector3.Lerp(focus.position, target.position, cameraSensitivy * Time.deltaTime);
            //focus.position = Vector3.Lerp(focus.position, target.position + Vector3.up * 3, cameraSensitivy * Time.deltaTime);
            //calcVector = new Vector3(0, Mathf.Clamp(calcVector.y, -5, 5), Mathf.Clamp(calcVector.z, -5f, 5f));
            //focus.position = calcVector;

            if (!isCameraTurn)
            {
                focus.rotation = Quaternion.Slerp(Quaternion.Euler(focus.rotation.eulerAngles.x, focus.rotation.eulerAngles.y, 0), Quaternion.Euler(new Vector3(30, focus.eulerAngles.y, 0)), cameraSensitivy * Time.deltaTime);
            }
        }

        #endregion

        #region Helper Methods
        public void MouseDrag(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed) // 우클릭중이면 true 떼면 false
            {
                isRightButton = true;
            }
            if (callbackContext.canceled)
                isRightButton = false;
        }

        public void CameraTurn(InputAction.CallbackContext callbackContext)
        {
            if(callbackContext.performed)
            {
                isCameraTurn = true;
            }
            if(callbackContext.canceled)
            {
                isCameraTurn = false;
            }
        }

        private IEnumerator MoveCamera()
        {
            while (true)
            {
                yield return null;

                if (isCameraTurn && isRightButton) // 컨트롤 버튼 누르는 중 + 우클릭 드래그 중에만 
                {
                    focus.eulerAngles =
                        new Vector3(Mathf.Clamp(focus.eulerAngles.x + (-Mouse.current.delta.y.ReadValue()), 0, 80),
                            focus.eulerAngles.y + (Mouse.current.delta.x.ReadValue()), 0);
                    continue;
                }

                if(isRightButton) // 우클릭 드래그 중에는 캐릭터의 로테이션을 바꿈
                {
                    focus.eulerAngles =
                        new Vector3(Mathf.Clamp(focus.eulerAngles.x + (-Mouse.current.delta.y.ReadValue()), 0, 80),
                            focus.eulerAngles.y + (Mouse.current.delta.x.ReadValue()), 0);
                    target.forward = new Vector3((focus.forward.x), 0, (focus.forward.z));
                }
            }
        }

        private IEnumerator Zoom()
        {
            while (true)
            {
                float z = Mouse.current.scroll.y.ReadValue();

                transform.localPosition += new Vector3(0, 0, Mathf.Clamp(z * Time.deltaTime / 2f, -2, 2));

                yield return null;
            }
        }
        
        public IEnumerator AimCameraMove()
        {
            _isAimOn = !_isAimOn;
            imageAim.enabled = !imageAim.enabled;

            float current = 0;
            float percent = 0;
            float time = 0.35f;

            float start = Camera.main.fieldOfView;
            float end = _isAimOn ? _aimModeFOV : _defaultModeFOV;
            
            Camera.main.transform.parent = _isAimOn ? aimFocus : focus;
            Vector3 offset = _isAimOn ? aimFocus.position : focus.position;
            
            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / time;
                
                Camera.main.fieldOfView = Mathf.Lerp(start, end, percent); // mode에 따라 카메라의 시야각을 변경
                Camera.main.transform.position =
                    Vector3.Lerp(Camera.main.transform.position, offset, percent);

                yield return null;
            }
            yield return null;
        }
        #endregion
    }
}
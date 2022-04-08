using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace ETeam.KyungSeo
{
    public class ClickDragCamera : MonoBehaviour
    {
        #region Variables
        private bool _isRightButton = false; // 마우스 우클릭 감지용 bool변수
        
        [SerializeField] private float camareSensitivity = 3;
        public Transform target = null;
        private float limitMinX = -10; // 카메라 회전범위(최소)
        private float limitMaxX = 50; // 카메라 회전범위(최대)
        private float eulerAngleX;
        private float eulerAngleY;
        
        #endregion

        #region Unity Methods
        private void Start()
        {
            StartCoroutine(MoveCamera());
        }

        private void Update()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, camareSensitivity * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, target.position, camareSensitivity * Time.deltaTime);
        }

        #endregion

        #region Helper Methods
        public void MouseDrag(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed) // 우클릭중이면 true 떼면 false
            {
                _isRightButton = true;
            }
            if (callbackContext.canceled)
                _isRightButton = false;
        }
        
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;

            return Mathf.Clamp(angle, min, max);
        }

        private IEnumerator MoveCamera()
        {
            while (true)
            {
                if (_isRightButton) // 우클릭 드래그 중에만 
                {
                    eulerAngleY += Mouse.current.delta.x.ReadValue() * camareSensitivity; // 마우스 좌/우 이동으로 카메라 y축 회전
                    eulerAngleX += Mouse.current.delta.y.ReadValue() * camareSensitivity; // 마우스 위/아래 이동으로 카메라 x축 회전
        
                    // 카메라 x축 회전의 경우 회전 범위를 설정
                    eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

                    transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
                    yield return null;
                }

                yield return null;
            }
        }
        #endregion
    }
}
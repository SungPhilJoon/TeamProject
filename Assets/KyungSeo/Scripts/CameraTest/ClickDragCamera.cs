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
        
        [SerializeField] private float cameraSensitivy = 3;
        public Transform target = null;
        public Transform focus;

        private float limitMinX = -10; // 카메라 회전범위(최소)
        private float limitMaxX = 50; // 카메라 회전범위(최대)
        private float eulerAngleX;
        private float eulerAngleY;

        public float distance;
        public float height;
        public float lerpPercent;

        public float rotateSpeed = 15f;
        private float offsetHeight = 0.7f;
        
        #endregion

        #region Unity Methods
        private void Start()
        {
            StartCoroutine(MoveCamera());
        }

        private void LateUpdate()
        {
            // Vector3 cameraFocus = target.position;

            // cameraFocus.y += offsetHeight;
            // target.position = cameraFocus;

            // // transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, camareSensitivity * Time.deltaTime);
            // transform.position = Vector3.Lerp(transform.position, target.position, camareSensitivity * Time.deltaTime);
            // transform.LookAt(cameraFocus);

            if (!target)
            {
                return;
            }

            if (!_isRightButton)
            {
                Vector3 worldPosition = (Vector3.forward * -distance) + (Vector3.up * height);
                Vector3 flatTargetPosition = target.position;
                flatTargetPosition.y += offsetHeight;

                Vector3 finalPosition = flatTargetPosition + worldPosition;

                Vector3 lerpedPosition = Vector3.Lerp(transform.position, finalPosition, lerpPercent);
                transform.position = lerpedPosition;

                transform.LookAt(flatTargetPosition);
            }
        }

        #endregion

        #region Helper Methods
        public void MouseDrag(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed) // 우클릭중이면 true 떼면 false
            {
                Debug.Log("움직이는 중~~");
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
                    focus.position = target.position;

                    focus.eulerAngles =
                        new Vector3(Mathf.Clamp(focus.eulerAngles.x + Mouse.current.delta.y.ReadValue(), 10, 80),
                            focus.eulerAngles.y + (-Mouse.current.delta.x.ReadValue()), 0);

                    yield return null;
                }

                yield return null;
            }
        }
        #endregion
    }
}
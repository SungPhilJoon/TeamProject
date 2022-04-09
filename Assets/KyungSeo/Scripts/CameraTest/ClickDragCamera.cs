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
        public Transform cameraArm;

        private float limitMinX = -10; // 카메라 회전범위(최소)
        private float limitMaxX = 50; // 카메라 회전범위(최대)
        private float eulerAngleX;
        private float eulerAngleY;

        public float distance;
        public float height;
        public float lerpPercent;
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

            Vector3 worldPosition = (Vector3.forward * -distance) + (Vector3.up * height);
            Vector3 flatTargetPosition = target.position;
            flatTargetPosition.y += offsetHeight;

            Vector3 finalPosition = flatTargetPosition + worldPosition;

            Vector3 lerpedPosition = Vector3.Lerp(transform.position, finalPosition, lerpPercent);
            transform.position = lerpedPosition;

            transform.LookAt(flatTargetPosition);
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
                    eulerAngleY = Mouse.current.delta.x.ReadValue() * cameraSensitivy; // 마우스 좌/우 이동으로 카메라 y축 회전
                    eulerAngleX = Mouse.current.delta.y.ReadValue() * cameraSensitivy; // 마우스 위/아래 이동으로 카메라 x축 회전
                    
                    // Debug.Log(eulerAngleX.ToString());
                    // Debug.Log(eulerAngleY.ToString());

                    // eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);
        
                    // Vector3 Y = Quaternion.AngleAxis(eulerAngleY / 100f, target.up) * cameraArm.forward;
                    // Vector3 X = Quaternion.AngleAxis(eulerAngleX / 100f, target.right) * cameraArm.forward;

                    // // 카메라 x축 회전의 경우 회전 범위를 설정
                    // //eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

                    // // cameraArm.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
                    // cameraArm.rotation = Quaternion.Euler(X + Y);
                    
                    Vector3 cameraAngle = cameraArm.rotation.eulerAngles;

                    float x = cameraAngle.x - eulerAngleX;

                    if (x < 180)
                    {
                        x = Mathf.Clamp(x, -1f, 70f);
                    }
                    else
                    {
                        x = Mathf.Clamp(x, 355f, 361f);
                    }

                    cameraArm.rotation = Quaternion.Euler(x, cameraAngle.y + eulerAngleY, cameraAngle.z);
                    
                    yield return null;
                }

                yield return null;
            }
        }
        #endregion
    }
}
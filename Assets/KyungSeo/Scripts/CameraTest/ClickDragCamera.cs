using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace ETeam.KyungSeo
{
    [ExecuteInEditMode]
    public class ClickDragCamera : MonoBehaviour
    {
        #region Variables
        private bool _isRightButton = false; // 마우스 우클릭 감지용 bool변수
        
        [SerializeField] private float cameraSensitivy = 3;
        public Transform target = null;
        public Transform focus;

        public float distance;
        public float height;
        public float lerpPercent;

        public float rotateSpeed = 15f;

        private Vector3 calcVector;
        
        #endregion

        #region Unity Methods
        private void Start()
        {
            calcVector = Vector3.zero;
            StartCoroutine(MoveCamera());
        }

        private void LateUpdate()
        {
            if (!target)
            {
                return;
            }

            float z = Mouse.current.scroll.y.ReadValue();

            transform.localPosition += new Vector3(0, 0, Mathf.Clamp(z * Time.deltaTime / 2f, -2, 2));

            //calcVector += new Vector3(0, (z * Time.deltaTime) * -1f, z * Time.deltaTime);

            focus.position = Vector3.Lerp(focus.position, target.position, cameraSensitivy * Time.deltaTime);
            //calcVector = new Vector3(0, Mathf.Clamp(calcVector.y, -5, 5), Mathf.Clamp(calcVector.z, -5f, 5f));
            //focus.position = calcVector;

            if (!_isRightButton)
            {
                focus.rotation = Quaternion.Lerp(Quaternion.Euler(focus.rotation.eulerAngles.x,focus.rotation.eulerAngles.y,0), Quaternion.Euler(new Vector3(30, 0, 0)), cameraSensitivy * Time.deltaTime);
            }
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

        private IEnumerator MoveCamera()
        {
            while (true)
            {
                if (_isRightButton) // 우클릭 드래그 중에만 
                {
                    focus.eulerAngles =
                        new Vector3(Mathf.Clamp(focus.eulerAngles.x + (-Mouse.current.delta.y.ReadValue()), 10, 80),
                            focus.eulerAngles.y + (Mouse.current.delta.x.ReadValue()), 0);
                }

                yield return null;
            }
        }

        private IEnumerator Zoom()
        {
            while (true)
            {
                

                yield return null;
            }
        }
        #endregion
    }
}
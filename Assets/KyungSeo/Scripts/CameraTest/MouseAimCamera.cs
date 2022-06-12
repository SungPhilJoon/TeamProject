using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityChanAdventure.KyungSeo
{
    public class MouseAimCamera : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameObject target;

        [Header("카메라 민감도")] [SerializeField] 
        private float rotateSpeed = 5;

        private Vector3 offset;
        private float _minXRotation = 0.0f;
        private float _maxXRotation = 60.0f;

        #endregion

        #region Unity Methods
        void Start()
        {
            offset = target.transform.position - transform.position;
        }

        void LateUpdate()
        {
            float horizontal = Mouse.current.delta.x.ReadValue() * rotateSpeed;
            float vertical = Mouse.current.delta.y.ReadValue() * rotateSpeed;
            target.transform.Rotate(vertical, horizontal, 0);

            float desiredAngleX = target.transform.eulerAngles.x;
            float desiredAngleY = target.transform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(Mathf.Clamp(-desiredAngleX, _minXRotation, _maxXRotation),
                desiredAngleY, Mathf.Clamp(-desiredAngleX, _minXRotation, _maxXRotation));
            transform.position = target.transform.position - (rotation * offset);

            transform.LookAt(target.transform);
        }
        #endregion
    }
}

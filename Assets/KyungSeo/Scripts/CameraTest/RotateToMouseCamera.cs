using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityChanAdventure.KyungSeo
{
    public class RotateToMouseCamera : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float camareSensitivity = 3;
        private float limitMinX = 0; // 카메라 회전범위(최소)
        private float limitMaxX = 50; // 카메라 회전범위(최대)
        private float eulerAngleX;
        private float eulerAngleY;
        #endregion

        #region Unity Methods

        private void Update()
        {
            UpdateRotate();
        }

        #endregion

        #region Helper Methods

        public void UpdateRotate()
        {
            eulerAngleY += Mouse.current.delta.x.ReadValue() * camareSensitivity; // 마우스 좌/우 이동으로 카메라 y축 회전
            eulerAngleX += Mouse.current.delta.y.ReadValue() * camareSensitivity; // 마우스 위/아래 이동으로 카메라 x축 회전

            // 카메라 x축 회전의 경우 회전 범위를 설정
            eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

            transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;

            return Mathf.Clamp(angle, min, max);
        }

        #endregion
    }
}

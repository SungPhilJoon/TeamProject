using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace UnityChanAdventure.KyungSeo
{
    public class TestShootArrow : MonoBehaviour
    {
        public GameObject arrowPrefab;
        public Transform arrowSpawnPosition;
        
        [Header("에임모드 카메라 관련")] [SerializeField] private Image imageAim;
        [SerializeField] private float _defaultModeFOV = 60; // 기본모드에서의 카메라 FOV
        [SerializeField] private float _aimModeFOV = 30; // Aim모드에서의 카메라 FOV
        private bool _isAimOn = false; // 에임모드 체크용

        private RaycastHit hit;
        
        [Header("거리")] [SerializeField]
        private float arrowRange = 100;

        private void Awake()
        {
            imageAim.enabled = false;
        }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            transform.Translate(new Vector3(h, 0f, v) * Time.deltaTime * 15f);
        }

        public void Shoot(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.canceled)
            {
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                if (Physics.Raycast(ray, out hit, arrowRange))
                {
                    GameObject arrowObject = GameObject.Instantiate(arrowPrefab, arrowSpawnPosition.transform.position,
                        arrowSpawnPosition.transform.rotation) as GameObject;
                    arrowObject.GetComponent<TestArrow>().SetTarget(hit.point);
                }
                
                _isAimOn = false;
                imageAim.enabled = false;

                StartCoroutine(AimCameraMove());
            }
        }

        public void AimIn(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                _isAimOn = true;
                imageAim.enabled = true;

                StartCoroutine(AimCameraMove());
            }
        }

        private IEnumerator AimCameraMove()
        {
            float current = 0;
            float percent = 0;
            float time = 0.35f;
            
            float start = Camera.main.fieldOfView;
            float end = _isAimOn ? _aimModeFOV : _defaultModeFOV;
            
            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / time;
            
                // mode에 따라 카메라의 시야각을 변경
                Camera.main.fieldOfView = Mathf.Lerp(start, end, percent);

                yield return null;
            }
            yield return null;
        }
    }
}
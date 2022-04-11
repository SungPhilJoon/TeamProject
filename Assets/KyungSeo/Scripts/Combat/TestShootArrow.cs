using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ETeam.KyungSeo
{
    public class TestShootArrow : MonoBehaviour
    {
        public GameObject arrowPrefab;
        public Transform arrowSpawnPosition;

        private RaycastHit hit;
        
        [Header("거리")] [SerializeField]
        private float arrowRange = 100;

        public void Shoot(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.started)
            {
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                if (Physics.Raycast(ray, out hit, arrowRange))
                {
                    GameObject arrowObject = GameObject.Instantiate(arrowPrefab, arrowSpawnPosition.transform.position,
                        arrowSpawnPosition.transform.rotation) as GameObject;
                    arrowObject.GetComponent<TestArrow>().SetTarget(hit.point);
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class Skill1_PlaceAreaWithMouse : MonoBehaviour
    {
        #region Variables
        public float surfaceOffset = 1.5f;
        public Transform target = null;

        public MainPlayerController owner;

        public float duration;

        #endregion Variables

        #region Unity Methods
        void Update()
        {
            //if (target)
            //{
            //    transform.position = target.position + Vector3.up * surfaceOffset;
            //}

            Ray ray = GameManager.Instance.mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 80, owner.groundLayerMask))
            {
                if (owner.placeArea)
                {
                    owner.placeArea.SetPosition(hit);
                }
            }
        }

        #endregion Unity Methods

        #region Helper Methods
        public void SetPosition(RaycastHit hit)
        {
            target = null;
            transform.position = hit.point + hit.normal * surfaceOffset;
        }

        #endregion Helper Methods
    }
}
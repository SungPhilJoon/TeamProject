using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class Skill1_PlaceAreaWithMouse : MonoBehaviour
    {
        #region Variables
        public float surfaceOffset = 1.5f;
        public Transform target = null;
        public MainPlayerController owner;

        #endregion Variables

        #region Unity Methods
        void Update()
        {
            //if (target)
            //{
            //    transform.position = target.position + Vector3.up * surfaceOffset;
            //}
        }

        void OnTriggerStay(Collider other)
        {
            
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
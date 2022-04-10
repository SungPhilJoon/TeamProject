using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public class FieldOfView : MonoBehaviour
    {
        #region Variables
        public float viewRadius = 5f;
        //[Range(0, 360)]
        //public float viewAngle = 90f;
    
        public LayerMask targetMask;
        public LayerMask obstacleMask;
    
        public Transform target;
        private float distanceToNearestTarget = 0.0f;
    
        public float delay = 0.2f;
    
        #endregion Variables
    
        #region Unity Methods
        void Start()
        {
            StartCoroutine(FindTargetsWithDelay(delay));
        }
    
        #endregion Unity Methods
    
        #region Helper Methods
        private IEnumerator FindTargetsWithDelay(float delay)
        {
            while(true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTarget();
            }
        }
    
        private void FindVisibleTarget()
        {
            distanceToNearestTarget = 0.0f;
            this.target = null;
    
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            if (targetsInViewRadius.Length == 0)
            {
                return;
            }
            Transform target = targetsInViewRadius[0].transform;

            Vector3 dirToTarget = (target.position- transform.position);
            float dstToTarget = Vector3.Distance(transform.position, target.position);
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            {
                    this.target = target;
            }
        }
    
        #endregion Helper Methods
    }
}
